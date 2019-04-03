using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RadaGovUa.Parser.Parsers;

namespace RadaGovUa.Parser.Helpers
{
    public enum DeputyRequestStatus
    {
        All,
        Removed
    }

    public class DeputyParserHelper
    {    
        public async Task<string> Parse(DeputyRequestStatus deputyRequest)
        {
            var deputiesLinkParser = new DeputiesLinkParser();

            var deputyParser = new DeputyParser();

            switch (deputyRequest)
            {
                case DeputyRequestStatus.All:
                    {
                        var deputiesLinks = await deputiesLinkParser
                                .GetDeputiesLinks(DeputiesLinkParser.AllDeputiesUrl);

                        var deputies = await deputyParser.ParsePages(deputiesLinks, true);

                        return JsonConvert.SerializeObject(deputies);
                    }
                case DeputyRequestStatus.Removed:
                    {
                        var deputiesLinks = await deputiesLinkParser
                                .GetDeputiesLinks(DeputiesLinkParser.RemoveDeputiesUrl);

                        var deputies = await deputyParser.ParsePages(deputiesLinks, false);

                        return JsonConvert.SerializeObject(deputies);
                    }
            }

            return null;
        }
    }
}