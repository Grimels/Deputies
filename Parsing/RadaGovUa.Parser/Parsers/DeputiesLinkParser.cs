using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RadaGovUa.Parser.Parsers
{
    internal class DeputiesLinkParser
    {
        public const string AllDeputiesUrl = @"http://w1.c1.rada.gov.ua/pls/site2/fetch_mps?skl_id=9";
        public const string RemoveDeputiesUrl = @"http://w1.c1.rada.gov.ua/pls/site2/fetch_mps?skl_id=9&pid_id=-3";

        public async Task<List<string>> GetDeputiesLinks(string url)
        {
            var httpClient = new HttpClient();

            var links = new List<string>();

            try
            {
                var deputiesHttpResponseMessage = await httpClient.GetAsync(url);

                if (deputiesHttpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var deputiesContent = deputiesHttpResponseMessage.Content;

                    if (deputiesContent != null)
                    {
                        var deputiesContentStream = await deputiesContent.ReadAsStreamAsync();

                        var htmlDocument = new HtmlDocument();
                        htmlDocument.Load(deputiesContentStream);

                        foreach (var selectNode in htmlDocument.DocumentNode.SelectNodes("//a[@target=\"_blank\"]"))
                        {
                            var href = selectNode.Attributes["href"];

                            if (href != null && href.Value != null)
                            {
                                var link = href.Value.Trim();

                                if (!string.IsNullOrWhiteSpace(link))
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
