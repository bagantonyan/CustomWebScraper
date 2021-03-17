using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Varord.amParseQuestionsToJson
{
    public class Question
    {
        public string questionText { get; set; }
        public bool hasImage { get; set; }
        public List<string> answers { get; set; }
    }
}
