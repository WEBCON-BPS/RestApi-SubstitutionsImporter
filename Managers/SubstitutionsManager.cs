using WebCon.ImportSubstitutionsApplication.Interfaces;
using System.Data;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebCon.ImportSubstitutionsApplication.Models;
using WebCon.ImportSubstitutionsApplication.Models.Substitution;

namespace WebCon.ImportSubstitutionsApplication.Managers
{
    public class SubstitutionsManager : ISubstitutionsManager
    {
        private readonly IConfigurationSettings _configurationSettings;
        private readonly IRestClient _httpClient;
        private readonly ILogger _logger;
        private IEnumerable<string> RequiredColumns { get; } = new List<string> { PersonColumnName, ActingColumnName, DateFromColumnName, DateToColumnName, IsActiveColumnName };
        private const string PersonColumnName = "Person";
        private const string ActingColumnName = "Acting";
        private const string DateFromColumnName = "DateFrom";
        private const string DateToColumnName = "DateTo";
        private const string IsActiveColumnName = "IsActive";
        private const string  ProcessIdColumnName = "ProcessId";
        private const string CompanyIdColumnName = "CompanyId";
        private const int SubstitutionsPerPage = 20;
        private const string GetSubstitutionsEndpoint = "api/data/v4.0/admin/substitutions";
        private const string AddSubstitutionEndpoint = "api/data/v4.0/admin/substitutions";

        public SubstitutionsManager(IConfigurationSettings configurationSettings, IRestClient httpClient, ILogger logger)
        {
            _configurationSettings = configurationSettings;
            _httpClient = httpClient;
            _logger = logger;
        }

        public void ImportSubstitutions()
        {
            var externalSubstitutions = GetActiveSubstitutionsFromExternalSource();
            var bpsSubstitutions = GetBpsSubstitutions();
            var newSubstitutions = externalSubstitutions.Except(bpsSubstitutions, new SubstitutionsComparer()).ToList();
            if (!newSubstitutions.Any())
            {
                _logger.Append("There are no new substitutions in external source.");
                return;
            }
            _logger.Append($"Found {newSubstitutions.Count()} new substitutions.");
            UpdateSubstitutions(newSubstitutions);
        }

        private IEnumerable<ExternalSubstitution> GetActiveSubstitutionsFromExternalSource()
        {
            var externalSubstitutionsTable = new DataTable();

            FillTableWithExternalSubstitutions(externalSubstitutionsTable);
            ValidateExternalDataTableColumns(externalSubstitutionsTable);

            var resultList = new List<ExternalSubstitution>();

            foreach (DataRow row in externalSubstitutionsTable.Rows)
            {
                ValidateRow(row);

                if ((bool) row[IsActiveColumnName] == false)
                {
                    continue;
                }

                resultList.Add(new ExternalSubstitution()
                {
                    PersonName = row[PersonColumnName].ToString(),
                    ActingName = row[ActingColumnName].ToString(),
                    DateFrom = (DateTime) row[DateFromColumnName],
                    DateTo = (DateTime) row[DateToColumnName],
                    IsActive = true,
                    CompanyId = row[CompanyIdColumnName] == DBNull.Value ? (int?) null : (int) row[CompanyIdColumnName],
                    ProcessId = row[ProcessIdColumnName] == DBNull.Value ? (int?) null : (int) row[ProcessIdColumnName],

                });
            }

            return resultList;
        }

        private void FillTableWithExternalSubstitutions(DataTable table)
        {
            using (var connection = new SqlConnection(_configurationSettings.ConnectionString))
            {
                using (var sqlCommand = new SqlCommand(_configurationSettings.SqlCommand, connection))
                {
                    using (var dataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        dataAdapter.Fill(table);
                    }
                }
            }
        }

        private void ValidateExternalDataTableColumns(DataTable dataTable)
        {
            foreach (var columnName in RequiredColumns)
            {
                if (!dataTable.Columns.Contains(columnName))
                {
                    throw new ArgumentException($"Column {columnName} does not exist in external source table.");
                }
            }

            AddColumn(dataTable, ProcessIdColumnName, typeof(int));
            AddColumn(dataTable, CompanyIdColumnName, typeof(int));
        }

        private void AddColumn(DataTable dataTable, string columnName, Type columnType)
        {
            if (!dataTable.Columns.Contains(columnName))
            {
                dataTable.Columns.Add(columnName, columnType);
            }
        }

        private void ValidateRow(DataRow row)
        {
            foreach (var column in RequiredColumns)
            {
                if (row[column] == DBNull.Value)
                {
                    throw new ArgumentException($"Column {column} cannot has null values");
                }
            }
        }

        private IEnumerable<ExternalSubstitution> GetBpsSubstitutions()
        {
            var resultCollection = new List<ExternalSubstitution>();
            var page = 0;
            var response = _httpClient.Get($"{GetSubstitutionsEndpoint}?size={SubstitutionsPerPage}&page={page++}");
            var bpsPagedSubstitutions = JsonConvert.DeserializeObject<SubstitutionsCollection>(response.Content.ReadAsStringAsync().Result);

            while (bpsPagedSubstitutions?.Substitutions.Count > 0)
            {
                var pagedSubstitutions = bpsPagedSubstitutions.Substitutions.Select(elem => new ExternalSubstitution()
                {
                    PersonName = elem.OriginalPerson.BpsId,
                    ActingName = elem.SubstitutePerson.BpsId,
                    DateFrom = elem.StartDate,
                    DateTo = elem.EndDate,
                }).ToList();

                resultCollection = resultCollection.Concat(pagedSubstitutions).ToList();

                response = _httpClient.Get($"{GetSubstitutionsEndpoint}?size={SubstitutionsPerPage}&page={page++}");
                bpsPagedSubstitutions = JsonConvert.DeserializeObject<SubstitutionsCollection>(response.Content.ReadAsStringAsync().Result);
            }

            return resultCollection;
        }

        private void UpdateSubstitutions(IEnumerable<ExternalSubstitution> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                _logger.Append($"Adding substitution for {substitution.PersonName} by {substitution.ActingName} from {substitution.DateFrom} to {substitution.DateTo} (ProcessId = {substitution.ProcessId}, CompanyId = {substitution.CompanyId})");
                var data = JsonConvert.SerializeObject(new Substitution(substitution, _configurationSettings.DatabaseId));
                _httpClient.Send(AddSubstitutionEndpoint, data);
            }
        }
    }
}
