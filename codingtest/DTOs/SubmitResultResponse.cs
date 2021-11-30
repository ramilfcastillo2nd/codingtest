using System.Collections.Generic;

namespace codingtest.DTOs
{
    public class SubmitResultResponse
    {
        public string Candidate { get; set; }
        public List<SubmitResultDetail> Results { get; set; }
    }
}
