using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace College_Mangement_System
{
    public class SubjectAverageDetails
    {
        public string subjectName;
        public double avgMark;

        public SubjectAverageDetails(string subjectName, double avgMark)
        {
            this.subjectName = subjectName;
            this.avgMark = avgMark;
        }
    }
}