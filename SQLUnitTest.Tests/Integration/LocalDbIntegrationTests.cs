using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SQLUnitTest.Models;
using SQLUnitTest.Models.Mocking;
using SQLUnitTest.Reporting;
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
            var dbFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".mdf");
            var connStr = $"Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;AttachDbFilename={dbFile};";
            var connections = new Dictionary<string, string> { { "Default", connStr } };

            var services = new ServiceCollection();
            var repo = new AdoDbRepository(connections);
            services.AddSingleton<IDbRepository>(repo);
            services.AddSingleton<IMarkdownReporter, MarkdownReporter>();
            services.AddTransient<ITestCaseHandler, ExecutionTestCaseHandler>();
            services.AddTransient<ITestRunner, BDDTestRunner>();
            var provider = services.BuildServiceProvider();

            var runner = provider.GetRequiredService<ITestRunner>();

            var test = new TestCase
            {
                Mock = new MockBlock
                {
                    PreConditions = new List<MockQuery>
                    {
                        new MockQuery{ Connection="Default", Query="CREATE TABLE Users(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(50));" },
                        new MockQuery{ Connection="Default", Query="INSERT INTO Users(Name) VALUES ('Alice'), ('Bob');" }
                    }
                },
                Should = new List<BaseTestCase>
                {
                    new ExecutionTestCase { StoredProcedure = "SELECT COUNT(*) AS Total FROM Users;" }
                }
            };

            var result = await runner.RunTestAsync(test);

            var countTable = await repo.QueryAsync("SELECT COUNT(*) AS Total FROM Users;", "Default");
            countTable.Rows[0][0].Should().Be(2);
            result.Passed.Should().BeTrue();
        }
    }
}
