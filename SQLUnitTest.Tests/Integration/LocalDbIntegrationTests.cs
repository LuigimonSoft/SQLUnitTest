using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SQLUnitTest.Models;
using SQLUnitTest.Models.Mocking;
using SQLUnitTest.Reporting;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;
using SQLUnitTest.Services;
using SQLUnitTest.Services.Handlers;
using SQLUnitTest.Repositories;
using Xunit;

namespace SQLUnitTest.Tests.Integration
{
    public class LocalDbIntegrationTests
    {
        [Fact]
        public async Task RunnerExecutesPreConditionsAndStoredProcedure()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }
            var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var dbName = random;
            var dbFile = Path.Combine(Path.GetTempPath(), dbName + ".mdf");
            var connStr = $"Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;AttachDbFilename={dbFile};Initial Catalog={dbName};";
            var masterStr = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master;";
            var connections = new Dictionary<string, string> { { "Default", connStr }, { "Master", masterStr } };

            var services = new ServiceCollection();
            var repo = new AdoDbRepository(connections);
            services.AddSingleton<IDbRepository>(repo);
            services.AddSingleton<IMarkdownReporter, MarkdownReporter>();
            services.AddTransient<ITestCaseHandler, ExecutionTestCaseHandler>();
            services.AddTransient<ITestRunner, BDDTestRunner>();
            var provider = services.BuildServiceProvider();

            var runner = provider.GetRequiredService<ITestRunner>();

            var sqlFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".sql");
            await File.WriteAllTextAsync(sqlFile, "CREATE PROCEDURE sp_seed AS INSERT INTO Users(Name) VALUES ('Charlie');");

            var jsonFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
            var jsonContent = "{ \"preConditions\" : [ { \"connection\" : \"Default\", \"query\" : \"INSERT INTO Users(Name) VALUES ('Alice'), ('Bob');\", \"type\" : \"Query\" } ] }";
            await File.WriteAllTextAsync(jsonFile, jsonContent);

            var test = new TestCase
            {
                Mock = new MockBlock
                {
                    PreConditions = new List<MockQuery>
                    {
                        new MockQuery{ Type = PreConditionType.InstallLocalDb },
                        new MockQuery{ Connection="Default", Query="CREATE TABLE Users(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(50));", Type=PreConditionType.Query },
                        new MockQuery{ Connection="Default", Query=sqlFile, Type=PreConditionType.SqlFile },
                        new MockQuery{ Connection="Default", Query="sp_seed", Type=PreConditionType.StoredProcedure },
                        new MockQuery{ Connection="Default", Query=jsonFile, Type=PreConditionType.JsonFile }
                    }
                },
                Should = new List<BaseTestCase>
                {
                    new ExecutionTestCase { StoredProcedure = "SELECT COUNT(*) AS Total FROM Users;" }
                }
            };

            var result = await runner.RunTestAsync(test);

            var countTable = await repo.QueryAsync("SELECT COUNT(*) AS Total FROM Users;", "Default");
            countTable.Rows[0][0].Should().Be(3);
            result.Passed.Should().BeTrue();

            // Clean up database to avoid attach conflicts on subsequent runs
            SqlConnection.ClearAllPools();
            await repo.QueryAsync($"DROP DATABASE [{dbName}];", "Master");
            File.Delete(dbFile);
            var logFile = Path.ChangeExtension(dbFile, ".ldf");
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
}
