using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolQuizMachine.Models.Poco
{
    public class Person
    {

        public string Initialz { get; set; }

        public int Score { get; set; }

        public Person(string initialz, int score)
        {
            Initialz = initialz;
            Score = score;
        }

        public override string ToString()
        {
            return String.Format("Initialer:{0}\t\tPoäng: {1}", Initialz, Score);
        }
    }
}
