using System;
using System.Collections.Generic;

namespace RadaGovUa.Parser.Models
{
    public class DeputyRequest
    {
        public string RequestId { get; set; }
        public string SessionId { get; set; }
        public string RequestAuthor { get; set; }
        public string RequestCoAuthors { get; set; }
        public string Problem { get; set; }
        public string RequestType { get; set; }
        public string Whom { get; set; }
        public string RequestBody { get; set; }
        public string RequestBodyUrl { get; set; }
        public string RequestDate { get; set; }
        public string DeadlineDate { get; set; }
        public bool Status => Answers.Count > 0;
        public List<DeputyRequestAnswer> Answers { get; } = new List<DeputyRequestAnswer>();
    }

    public class DeputyRequestAnswer
    {
        public string AnswerDate { get; set; }
        public string AnswerBody { get; set; }
        public string AnswerBodyUrl { get; set; }
        public string FamiliarizationDate { get; set; }
    }
}