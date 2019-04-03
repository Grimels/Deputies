using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RadaGovUa.Parser.Parsers
{
    internal class DeputyRequestsLinkParser
    {
        private const string DeputyRequestsUrl =
            @"http://w1.c1.rada.gov.ua/pls/zweb2/pkg_depzap.list?ses=10009&request_num_s=2&request_num=&date1=&date2=&request_body=&av_nd=&executive_type=";

        internal static string DeputyRequestsPrefixUrl = @"http://w1.c1.rada.gov.ua/pls/zweb2/";

        public async Task<List<string>> GetDeputyRequestsLinks()
        {
            var httpClient = new HttpClient();

            var links = new List<string>();

            try
            {

                var deputyRequestsHttpResponseMessage = await httpClient.GetAsync(DeputyRequestsUrl);

                if (deputyRequestsHttpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var deputyRequestsContent = deputyRequestsHttpResponseMessage.Content;

                    if (deputyRequestsContent != null)
                    {
                        var deputiesContentStream = await deputyRequestsContent.ReadAsStreamAsync();

                        var htmlDocument = new HtmlDocument();
                        htmlDocument.Load(deputiesContentStream);

                        foreach (var selectNode in htmlDocument.DocumentNode.SelectNodes("//table//td[@class='THEAD3'][@align='center']//a"))
                        {
                            var href = selectNode.Attributes["href"];

                            if (href != null && href.Value != null)
                            {
                                var link = DeputyRequestsPrefixUrl + href.Value.Trim();

                                if (! string.IsNullOrWhiteSpace(link))
                                {
                                    links.Add(link);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return links;
        }
    }
}
