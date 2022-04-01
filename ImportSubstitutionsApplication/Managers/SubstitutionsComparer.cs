using WebCon.ImportSubstitutionsApplication.Models;
using System.Collections.Generic;

namespace WebCon.ImportSubstitutionsApplication.Managers
{
    public class SubstitutionsComparer : IEqualityComparer<ExternalSubstitution>
    {
        public bool Equals(ExternalSubstitution lhs, ExternalSubstitution rhs)
        {
            if ((lhs == null) ^ (rhs == null))
            {
                return false;
            }

            if (lhs == null)
            {
                return true;
            }

            return lhs.ActingName == rhs.ActingName && lhs.DateFrom == rhs.DateFrom && lhs.DateTo == rhs.DateTo && lhs.PersonName == rhs.PersonName;
        }

        public int GetHashCode(ExternalSubstitution obj)
        {
            return (obj.ActingName + obj.DateFrom + obj.DateTo + obj.PersonName).GetHashCode();
        }
    }
}
