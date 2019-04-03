using System.Threading.Tasks;
using Newtonsoft.Json;
using RadaGovUa.Parser.Parsers;
using System.Collections.Generic;
using System;
using RadaGovUa.Parser.Models;
using Deputies.Parsing.Common;

namespace RadaGovUa.Parser.Helpers
{
    public class DeputyRequestsParserHelper
    {    
        public async Task<List<DeputyRequest>> Parse()
        {
            var deputyRequestsLinkParser = new DeputyRequestsLinkParser();

            var deputyRequestsParser = new DeputyRequestsParser();

            List<string> deputyRequestsLinks;
            do
            {
                deputyRequestsLinks = await deputyRequestsLinkParser
                                .GetDeputyRequestsLinks();

                Logger.Log("Found " + deputyRequestsLinks.Count + " inquries");
            }
            while (deputyRequestsLinks.Count == 0);
            

            return await deputyRequestsParser.ParsePages(deputyRequestsLinks);
        }
    }
}