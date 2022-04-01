using System;

namespace WebCon.ImportSubstitutionsApplication.Models
{
    public class ExternalSubstitution
    {
        public string PersonName { get; set; }
        public string ActingName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsActive { get; set; }
        public int? ProcessId { get; set; }
        public int? CompanyId { get; set; }
    }
}
