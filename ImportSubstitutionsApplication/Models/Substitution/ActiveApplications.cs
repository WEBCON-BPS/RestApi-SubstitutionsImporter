using System.Collections.Generic;

namespace WebCon.ImportSubstitutionsApplication.Models.Substitution
{
    public class ActiveApplications
    {
        public bool AllApplications { get; set; }
        public List<ProcessInfo> Processes { get; set; }
    }
}
