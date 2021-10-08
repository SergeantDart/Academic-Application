using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace College_Mangement_System
{
    public class StudyGroupsHomogenityDetails
    {
        public string studyGroupId;
        public int studentsNo;

        public StudyGroupsHomogenityDetails(string studyGroupId, int studentsNo)
        {
            this.studyGroupId = studyGroupId;
            this.studentsNo = studentsNo;
        }
    }
}