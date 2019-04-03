using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RadaGovUa.Parser.Models;
using System.Text;
using Deputies.Parsing.Common;

namespace RadaGovUa.Parser.Parsers
{
    internal class DeputyRequestsParser
    {
        public async Task<List<DeputyRequest>> ParsePages(List<string> urls)
        {
            var httpClient = new HttpClient();

            var deputyRequests = new List<DeputyRequest>();

            int counter = 0;
            foreach (var url in urls)
            {
                deputyRequests.Add(await ParsePage(httpClient, url));

                if(++counter % 1000 == 0)
                {
                    Logger.LogInfo("Parsed " + counter + " inquries");
                }
            }

            return deputyRequests;
        }

        public async Task<DeputyRequest> ParsePage(HttpClient httpClient, string url)
        {
            var deputyRequest = new DeputyRequest();

            var deputyRequestHttpResponse = await httpClient.GetAsync(url);

            Debug.WriteLine(url);

            if (deputyRequestHttpResponse.StatusCode == HttpStatusCode.OK)
            {
                var deputyRequestContent = deputyRequestHttpResponse.Content;

                if (deputyRequestContent != null)
                {
                    var deputyRequestStream = await deputyRequestContent.ReadAsByteArrayAsync();
                    var html = Encoding.GetEncoding("Windows-1251").GetString(deputyRequestStream);

                    try
                    {
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);

                        var keyNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='information_block_ins']//td[@class='THEAD32']");
                        var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='information_block_ins']//td[@class='THEAD21']");

                        for (var i = 0; i < keyNodes.Count; i++)
                        {
                            var node = keyNodes[i];
                            var child = node.FirstChild;

                            var valueNode = nodes[i];
                            var valuechild = valueNode.FirstChild;

                            if (child != null)
                            {
                                var keytext = child.InnerText.ClearHtml();
                                var text = valuechild.InnerText.ClearHtml();

                                switch (keytext)
                                {
                                    case "№ запиту":
                                        deputyRequest.RequestId = text;
                                        break;
                                    case "№ сесії":
                                        deputyRequest.SessionId = text;
                                        break;
                                    case "Дата запиту":
                                        deputyRequest.RequestDate = text;
                                        break;
                                    case "Автор запиту":
                                        deputyRequest.RequestAuthor = text;
                                        break;
                                    case "Співавтор(и) запиту":
                                        deputyRequest.RequestCoAuthors = valueNode.ChildNodes[3].InnerText.ClearHtml();
                                        break;
                                    case "Проблематика":
                                        deputyRequest.Problem = text;
                                        break;
                                    case "Тип запиту":
                                        deputyRequest.RequestType = text;
                                        break;
                                    case "Термін виконання":
                                        deputyRequest.DeadlineDate = text;
                                        break;
                                    case "До кого запит":
                                        deputyRequest.Whom = text;
                                        break;
                                    case "Зміст запиту":
                                        deputyRequest.RequestBody = text;
                                        var href = valuechild.FirstChild?.Attributes["href"]?.Value;
                                        if (string.IsNullOrWhiteSpace(href))
                                        {
                                            deputyRequest.RequestBodyUrl = null;
                                        }
                                        else
                                        {
                                            deputyRequest.RequestBodyUrl = DeputyRequestsLinkParser.DeputyRequestsPrefixUrl + valuechild.FirstChild?.Attributes["href"]?.Value;
                                        }

                                        break;
                                }
                            }
                        }

                        var answers = htmlDocument.DocumentNode.SelectNodes("//div[@class='information_block_ins']//table[4]//tr");

                        for (var i = 1; i < answers.Count; i++)
                        {
                            var answerNode = answers[i];

                            var answer = new DeputyRequestAnswer
                            {
                                AnswerDate = answerNode.ChildNodes[1]?.InnerText.ClearHtml(),
                                FamiliarizationDate = answerNode.ChildNodes[5]?.InnerText.ClearHtml()
                            };

                            var bodyNode = answerNode.ChildNodes[3];

                            if (bodyNode != null)
                            {
                                answer.AnswerBody = bodyNode.InnerText.ClearHtml();

                                var answerUrl = bodyNode.FirstChild?.Attributes["href"]?.Value;

                                if (answerUrl != null)
                                {
                                    answer.AnswerBodyUrl = DeputyRequestsLinkParser.DeputyRequestsPrefixUrl
                                        + answerUrl;
                                }
                            }

                            deputyRequest.Answers.Add(answer);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Parsing error {e.Message}");
                    }
                }
            }

            return deputyRequest;
        }
    }
}