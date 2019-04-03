using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RadaGovUa.Parser.Models;

namespace RadaGovUa.Parser.Parsers
{
    internal class DeputyParser
    {
        public async Task<List<Deputy>> ParsePages(List<string> urls, bool status)
        {
            var httpClient = new HttpClient();

            var deputies = new List<Deputy>();

            foreach (var url in urls)
            {
                deputies.Add(await ParsePage(httpClient, url, status));
            }

            return deputies;
        }

        public async Task<Deputy> ParsePage(HttpClient httpClient, string url, bool status)
        {
            var deputy = new Deputy();

            var deputyHttpResponse = await httpClient.GetAsync(url);

            Debug.WriteLine(url);

            if (deputyHttpResponse.StatusCode == HttpStatusCode.OK)
            {
                var deputyContent = deputyHttpResponse.Content;

                if (deputyContent != null)
                {
                    var deputyStream = await deputyContent.ReadAsStreamAsync();

                    try
                    {
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.Load(deputyStream, Encoding.UTF8);

                        // status
                        deputy.Status = status;

                        // site
                        deputy.Site = url;

                        var informationBlock = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='information_block']");

                        if (informationBlock != null)
                        {
                            // email
                            var emailNode = informationBlock.SelectSingleNode("//div[@class='topTitle']//a[starts-with(@href, 'mailto:')]");
                            if (emailNode != null)
                            {
                                deputy.Email = emailNode.InnerText.Trim(' ', '\n', '\r');
                            }

                            // name
                            var nameNode = informationBlock.SelectSingleNode("//h2");
                            if (nameNode != null)
                            {
                                deputy.Name = nameNode.InnerText.Trim(' ', '\n', '\r');
                            }

                            // image
                            var imageNode = informationBlock.SelectSingleNode("//td//img");
                            if (imageNode != null)
                            {
                                deputy.ImageUrl = imageNode.Attributes["src"]?.Value;
                            }

                            // group
                            if (status)
                            {
                                var groupNode = informationBlock.SelectSingleNode("//td[2]//text()[5]");
                                if (groupNode != null)
                                {
                                    deputy.Group = groupNode.InnerText.Trim(' ', '\n', '\r');
                                }
                            }
                            else
                            {
                                var generalInfo1 = informationBlock.SelectSingleNode("//div[@class='mp-general-info']");
                                if (generalInfo1 != null)
                                {
                                    // selected
                                    var selectedNode = generalInfo1.SelectSingleNode("//dd[2]");
                                    if (selectedNode != null)
                                    {
                                        deputy.Group = selectedNode.InnerText.Trim(' ', '\n', '\r');
                                    }
                                }

                            }

                            var generalInfo = informationBlock.SelectSingleNode("//div[@class='mp-general-info']");
                            if (generalInfo != null)
                            {
                                // selected
                                var selectedNode = generalInfo.SelectSingleNode("//dd[1]");
                                if (selectedNode != null)
                                {
                                    deputy.Selected = selectedNode.InnerText.Trim(' ', '\n', '\r');
                                }

                                // партия 
                                var partyLabelNode = generalInfo.SelectSingleNode("//dt[2]");
                                var partyLabelNodeText = partyLabelNode.InnerText.Trim(' ', '\n', '\r');
                                if (partyLabelNodeText == "Партія:")
                                {
                                    var partyNode = generalInfo.SelectSingleNode("//dd[2]");
                                    if (partyNode == null)
                                    {
                                        
                                    }
                                    else
                                    {
                                        deputy.Party = partyNode.InnerText.Trim(' ', '\n', '\r');
                                    }

                                }

                            }

                            // positions
                            var positionNode = informationBlock.SelectSingleNode("//ul[@class='level1']//li");
                            if (positionNode != null)
                            {
                                deputy.Position = positionNode.InnerText.Trim(' ', '\n', '\r');
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Parsing error");
                    }
                }
            }

            return deputy;
        }
    }
}