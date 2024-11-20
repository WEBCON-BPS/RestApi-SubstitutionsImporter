using WebCon.ImportSubstitutionsApplication.Models.Enums;
using System;
using System.Collections.Generic;

namespace WebCon.ImportSubstitutionsApplication.Models.Substitution
{
    public class Substitution
    {
        public PersonInfo OriginalPerson { get; set; }
        public PersonInfo SubstitutePerson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubstitutionType Type { get; set; }
        public BaseInfo BusinessEntity { get; set; }
        public ActiveApplications ActiveApplications { get; set; }

        public Substitution()
        {

        }
        public Substitution(ExternalSubstitution externalSubstitution, int dbId)
        {
            OriginalPerson = new PersonInfo() { BpsId = externalSubstitution.PersonName,  TypedInValue = true };
            SubstitutePerson = new PersonInfo() { BpsId = externalSubstitution.ActingName };
            StartDate = externalSubstitution.DateFrom;
            EndDate = externalSubstitution.DateTo;
            Type = SubstitutionType.TaskDelegation;
            BusinessEntity = externalSubstitution.CompanyId.HasValue ? new BaseInfo() { Id = externalSubstitution.CompanyId.Value, DbId = dbId } : null;
            ActiveApplications = new ActiveApplications()
            {
                AllApplications = !externalSubstitution.ProcessId.HasValue,
                Processes = externalSubstitution.ProcessId.HasValue ? new List<ProcessInfo> { new ProcessInfo() { Id = externalSubstitution.ProcessId.Value, DbId = dbId } } : null
            };
        }
    }
}
