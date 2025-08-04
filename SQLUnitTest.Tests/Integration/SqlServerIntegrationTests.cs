using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using SQLUnitTest.Models;
using SQLUnitTest.Models.Mocking;
using SQLUnitTest.Reporting;
using SQLUnitTest.Repositories;
using SQLUnitTest.Services;
using SQLUnitTest.Services.Handlers;
using Xunit;

namespace SQLUnitTest.Tests.Integration
{
    public class SqlServerIntegrationTests
    {
        [Fact]
        public async Task RunnerExecutesPreConditionsAndStoredProcedure()
        {
            if (Environment.GetEnvironmentVariable("SQLSERVER_AVAILABLE") != "1")
            {
                return;
            }

            var masterStr = "Server=localhost,1433;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True";

            var dbName = Guid.NewGuid().ToString("N").Substring(0, 8);
            var createDb = $"CREATE DATABASE [{dbName}]";
            var connections = new Dictionary<string, string>
            {
                { "Default", masterStr + $";Initial Catalog={dbName}" },
                { "Master", masterStr }
            };

            var repo = new AdoDbRepository(connections);
            await repo.QueryAsync(createDb, "Master");

            var services = new ServiceCollection();
            services.AddSingleton<IDbRepository>(repo);
            services.AddSingleton<IMarkdownReporter, MarkdownReporter>();
            services.AddTransient<ITestCaseHandler, ExecutionTestCaseHandler>();
            services.AddTransient<ITestRunner, BDDTestRunner>();
            var provider = services.BuildServiceProvider();

            var runner = provider.GetRequiredService<ITestRunner>();

            var seedFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".sql");
            await File.WriteAllTextAsync(seedFile, "CREATE PROCEDURE sp_seed AS INSERT INTO Users(Name) VALUES ('Charlie');");

            var countFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".sql");
            await File.WriteAllTextAsync(countFile, "CREATE PROCEDURE sp_get_total AS SELECT COUNT(*) AS Total FROM Users;");

            var jsonFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
            var jsonContent =
                "{ \"mock\" : { \"preConditions\" : [ { \"connection\" : \"Default\", \"query\" : \"INSERT INTO Users(Name) VALUES ('Alice'), ('Bob');\", \"type\" : \"Query\" } ] } }";
            await File.WriteAllTextAsync(jsonFile, jsonContent);

            var test = new TestCase
            {
                Mock = new MockBlock
                {
                    PreConditions = new List<MockQuery>
                    {
                        new MockQuery{ Connection="Default", Query="CREATE TABLE Users(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(50));", Type=PreConditionType.Query },
                        new MockQuery{ Connection="Default", Query=seedFile, Type=PreConditionType.SqlFile },
                        new MockQuery{ Connection="Default", Query=countFile, Type=PreConditionType.SqlFile },
                        new MockQuery{ Connection="Default", Query="sp_seed", Type=PreConditionType.StoredProcedure },
                        new MockQuery{ Connection="Default", Query=jsonFile, Type=PreConditionType.JsonFile }
                    }
                },
                Should = new List<BaseTestCase>
                {
                    new ExecutionTestCase { StoredProcedure = "sp_get_total" }
                }
            };

            var result = await runner.RunTestAsync(test);

            var countTable = await repo.QueryAsync("SELECT COUNT(*) AS Total FROM Users;", "Default");
            countTable.Rows[0][0].Should().Be(3);
            result.Passed.Should().BeTrue();

            // Clean up database to avoid attach conflicts on subsequent runs
            SqlConnection.ClearAllPools();
            await repo.QueryAsync($"DROP DATABASE [{dbName}];", "Master");
        }
    }
}
