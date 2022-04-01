# RestApi-SubstitutionsImporter

## WebCon.ImportSubstitutionsApplication ##
**WebCon.ImportSubstitutionsApplication** is an application used to import substitutions from an external database source to WEBCON BPS in the following steps:
- Import substitutions from an external database;
- Import substitutions from WEBCON BPS;
- Substitutions comparision per Person, Acting, DateFrom, DateTo properties;
- Addition of substitutions from external database that do not exist in WEBCON BPS

All substitutions from external source will be saved with *Tasks delegation mode*. 

## Configuration ##
To run application properly several parameters need to be set in App.config file:
- **connectionString** - Connection string to the external database where substitutions are stored;
- **sqlCommand** - SQL command returning substitutions from external database;
- **portalUrl** - URL to WEBCON BPS portal;
- **databaseId** - WEBCON BPS database ID in the context of which import will be performed;
- **clientID** - WEBCON BPS client ID;
- **clientSecret** - WEBCON BPS client secret;
- **logsFilePath** - Path to the file (with .txt extension) where applications logs will be stored. If it does not exist, a new file will be created. 

## External database source ##
External database source should be a SQL table containing following **required** columns:
- **Person** -Substitute for BPS user (passed in Acting column) (VARCHAR);
- **Acting** - Person for which the substitution is created (VARCHAR);
- **DateFrom** - Substitution start date (DATETIME);
- **DateTo** - Substitution end date (DATETIME);
- **IsActive** - Substitution status (BIT);

It can also contains two **optional** columns:
- **ProcessId** - WEBCON BPS process identifier for which the substitution will be active (if NULL then substitutions will be active for all processes to which the Acting person has access to) (INT);
- **CompanyId** - WEBCON BPS business entity identifier for which the substitution will be active (if NULL then substitutions will be active for all business entities to which the Acting person has access to) (INT);

Above columns names for the data table can be changed in SubstitutionsManager.cs file.