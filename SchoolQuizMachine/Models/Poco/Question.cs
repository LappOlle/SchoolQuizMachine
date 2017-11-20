using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolQuizMachine.Models.Poco
{
    public class QuestionData
    {
        public string Question { get; set; }
        public string AlternativeOne { get; set; }
        public string AlternativeTwo { get; set; }
        public string AlternativeThree { get; set; }
        public string Answer { get; set; }
    }
}
