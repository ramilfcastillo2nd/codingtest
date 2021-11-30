using codingtest.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codingtest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Test2Controller : ControllerBase
    {
        private const string TextToSearchVal = "Peter told me (actually he slurrred) that peter the pickle piper piped a pitted pickle before he petered out. Phew!";

        [AllowAnonymous]
        [HttpGet("textToSearch")]
        public async Task<IActionResult> GetTextToSearch()
        {
            var response = new TextToSearchResponse {
                Text = TextToSearchVal
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("subTexts")]
        public async Task<IActionResult> GetSubTexts()
        {
            var response = new SubTextToSearch
            {
                SubTexts = new string[] 
                {
                    "Peter","peter","Pick","Pi","Z"
                }
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("submitResults")]
        public async Task<IActionResult> SubmitResults([FromBody] SubmitResultRequest request)
        {
            var lstResponse = new List<SubmitResultDetail>();
            var tempStringArray = new List<WordDetail>();
            var tempStringFiltered = new List<WordDetail>();
            var tempString = string.Empty;
            var beginIndex = 1; 
            for (int i = 0; i < TextToSearchVal.Length; i++)
            {
                if (TextToSearchVal[i].ToString() != " ")
                    tempString += TextToSearchVal[i];
                else
                {
                    tempStringArray.Add(new WordDetail { 
                        BeginIndex = beginIndex,
                        Word = tempString
                    });

                    beginIndex = i + 2;
                    tempString = string.Empty;
                }

                if (TextToSearchVal.Length - 1 == i)
                {
                    tempStringArray.Add(new WordDetail
                    {
                        BeginIndex = beginIndex,
                        Word = tempString
                    });
                    tempString = string.Empty;
                }
            }

            foreach (var text in request.SubTexts)
            {

                lstResponse.Add(new SubmitResultDetail { 
                    Result = string.Join(",", tempStringArray.Where(s => SearchString(s.Word.ToLower(), text.ToLower())).Select(s => s.BeginIndex).ToArray()),
                    SubText = text        
                });
            }

            lstResponse = lstResponse.Select(s =>
            {
                s.Result = string.IsNullOrEmpty(s.Result) ? "<No Output>" : s.Result;
                return s;
            }).ToList();

            return Ok(new SubmitResultResponse
            { 
                Candidate = request.Candidate == null? string.Empty: request.Candidate,
                Results = lstResponse
            });
        }

        private  bool SearchString(string stringToSearch, string stringToFind)
        {
            var maxIndex = stringToSearch.Length - stringToFind.Length;
            if (maxIndex < 0) return false;

            for (int i = 0; i <= maxIndex; i++)
            {
                int j;
                for (j = 0; j < stringToFind.Length; j++)
                    if (stringToSearch[i + j] != stringToFind[j]) break;
                if (j == stringToFind.Length) return true;
            }

            return false;
        }

    }
}
