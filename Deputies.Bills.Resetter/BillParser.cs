using Deputies.Parsing.Common;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Bills.Resetter
{
    class BillParser
    {
        private const string ListUrl = "http://w1.c1.rada.gov.ua/pls/zweb2/webproc2_5_1_J?ses=10009&num_s=2&num=&date1=&date2=&name_zp=&out_type=&id=&page=1";

        private readonly HttpClient client = new HttpClient();

        public async Task<IEnumerable<BillParsingModel>> ParseAllBillsAsync()
        {
            var links = await ParseLinks();
            var allBills = new List<BillParsingModel>();
            var counter = 0;
            var count = links.Count();
            Logger.LogInfo("Parsed " + count + " links");
            foreach (var link in links)
            {
                var bill = await this.ParseBill(link);
                allBills.Add(bill);


                if (++counter % 100 == 0)
                {
                    Logger.LogInfo("Parsed " + counter + " bills");
                }
            }

            return allBills;
        }

        private async Task<byte[]> ParseListPage()
        {
            var formVariables = new List<KeyValuePair<string, string>>();
            formVariables.Add(new KeyValuePair<string, string>("zp_cnt", "-1"));
            var formContent = new FormUrlEncodedContent(formVariables);

            var response = await client.PostAsync(ListUrl, formContent);
            return await response.Content.ReadAsByteArrayAsync();
        }

        private async Task<byte[]> ParseBillPage(string url)
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<IEnumerable<string>> ParseLinks()
        {
            var listContent = await ParseListPage();
            var html = Encoding.GetEncoding("Windows-1251").GetString(listContent);
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var linkNodes = document
                .DocumentNode
                .SelectNodes("//*[@id='content-all']/div[2]/div/table/tr/td/a")
                .Select(x => "http://w1.c1.rada.gov.ua/pls/zweb2/" + x.GetAttributeValue("href", ""));

            return linkNodes.ToList();
        }

        private async Task<BillParsingModel> ParseBill(string url)
        {
            var billContent = await this.ParseBillPage(url);
            var html = Encoding.GetEncoding("Windows-1251").GetString(billContent);
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var bill = new BillParsingModel();

            bill.Url = url;

            bill.Title = document
                .DocumentNode
                .SelectSingleNode("//*[@id='content-all']/div[2]/div/h3")
                ?.InnerText
                ?.Trim()
                ?.Trim('\n');

            var numberDateValue = this.GetTabletValueByKey(document.DocumentNode, "Номер, дата реєстрації:");
            if (!string.IsNullOrWhiteSpace(numberDateValue))
            {
                var splittedNumberDateValue = numberDateValue.Split(new string[] { " від " }, StringSplitOptions.None);
                bill.Number = splittedNumberDateValue[0];
                bill.Date = splittedNumberDateValue[1];
            }

            bill.Session = int.Parse(this.GetTabletValueByKey(document.DocumentNode, "Сесія реєстрації:")?.Split(' ')[0]);

            bill.Included = this.GetTabletValueByKey(document.DocumentNode, "Включено до порядку денного:", 1);

            bill.Rubric = this.GetTabletValueByKey(document.DocumentNode, "Рубрика законопроекту:");

            bill.SubjectOfRight = this.GetTabletValueByKey(document.DocumentNode, "Суб'єкт права законодавчої ініціативи:");

            bill.Initiators = this.GetTabletValueByKey(document.DocumentNode, "Ініціатор(и) законопроекту:")
                .Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => string.Join(" ", x.Split().Take(3)))
                .ToList();

            bill.MainCommittee = this.GetTabletValueByKey(document.DocumentNode, "Головний комітет:");

            bill.OtherCommittees = this.GetTabletValueByKeyAsList(document.DocumentNode, "Інші комітети:")?.ToList();

            bill.Texts = this.GetTabletLinksByKeyAsList(document.DocumentNode, "Текст законопроекту та супровідні документи:")?.ToList();

            bill.DocumentsRelatedToWork = this.GetTabletLinksByKeyAsList(document.DocumentNode, "Документи, пов'язані із роботою:")?.ToList();

            return bill;
        }

        private string GetTabletValueByKey(HtmlNode node, string key, int valueOffset = 2)
        {
            var childNodes = node
                .SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[2]/div[1]/div[2]/dl[1]")
                .ChildNodes;

            var keyNode = childNodes.FirstOrDefault(x => x.InnerText.Contains(key));

            if (keyNode != null)
            {
                var index = childNodes.GetNodeIndex(keyNode);
                var valueNode = childNodes[index + valueOffset];
                return valueNode.InnerText.Trim().Trim('\n');
            }

            return null;
        }

        private IEnumerable<string> GetTabletValueByKeyAsList(HtmlNode node, string key, int valueOffset = 2)
        {
            var childNodes = node
                .SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[2]/div[1]/div[2]/dl[1]")
                .ChildNodes;

            var keyNode = childNodes.FirstOrDefault(x => x.InnerText.Contains(key));

            if (keyNode != null)
            {
                var index = childNodes.GetNodeIndex(keyNode);
                var valueNode = childNodes[index + valueOffset];
                return valueNode.ChildNodes.Select(x => x.InnerText.Trim().Trim('\n'));
            }

            return null;
        }

        private IEnumerable<LinkParsingModel> GetTabletLinksByKeyAsList(HtmlNode node, string key, int valueOffset = 2)
        {
            var childNodes = node
                .SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[2]/div[1]/div[2]/dl[1]")
                .ChildNodes;

            var keyNode = childNodes.FirstOrDefault(x => x.InnerText.Contains(key));

            if (keyNode != null)
            {
                var index = childNodes.GetNodeIndex(keyNode);
                var valueNode = childNodes[index + valueOffset];
                return valueNode
                    .ChildNodes
                    .Where(x => x.Name == "li")
                    .Select(x =>
                    {
                        var a = x.ChildNodes.FirstOrDefault(y => y.Name == "a");
                        if (a != null)
                        {
                            return new LinkParsingModel() { Title = x.InnerText.Trim().Trim('\n'), URL = "http://w1.c1.rada.gov.ua/pls/zweb2/" + a.GetAttributeValue("href", "") };
                        }

                        return new LinkParsingModel() { Title = x.InnerText.Trim().Trim('\n') };
                    });
            }

            return null;
        }
    }
}
