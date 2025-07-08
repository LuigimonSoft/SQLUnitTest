using System.Threading.Tasks;
using FluentAssertions;
using SQLUnitTest.Models;
using System.Collections.Generic;
using SQLUnitTest.Services;
using SQLUnitTest.Services.Handlers;
using SQLUnitTest.Services.Models;
using System.Text.Json;
using Xunit;
using SQLUnitTest.Repositories;
using System.Data;
using SQLUnitTest.Models.Mocking;

namespace SQLUnitTest.Tests.Services
{
    public class BDDTestRunnerTests
    {
        private class FakeHandler : ITestCaseHandler
        {
            public bool CanHandleCalled { get; private set; }
            public bool ExecuteCalled { get; private set; }
            public BaseTestCase? ExecutedTestCase { get; private set; }
            public List<BaseTestCase> ExecutedCases { get; } = new();

            public bool CanHandle(BaseTestCase testCase)
            {
                CanHandleCalled = true;
                return true; // handle any test case
            }

            public Task<TestResult> ExecuteAsync(BaseTestCase testCase)
            {
                ExecuteCalled = true;
                ExecutedTestCase = testCase;
                ExecutedCases.Add(testCase);
                return Task.FromResult(new TestResult { Passed = true, Report = "ok" });
            }
        }

        private class FakeRepository : IDbRepository
        {
            public List<string> Queries { get; } = new();
            public List<string> StoredProcedures { get; } = new();

            public Task<DataSet> ExecuteStoredProcedureAsync(string storedProcedure, object? parameters, string connectionName)
            {
                StoredProcedures.Add(storedProcedure);
                return Task.FromResult(new DataSet());
            }

            public Task<DataTable> QueryAsync(string query, string connectionName)
            {
                Queries.Add(query);
                return Task.FromResult(new DataTable());
            }
        }

        [Fact]
        public async Task GivenExecutionTestCaseWhenRunTestAsyncShouldInvokeHandlerThenReturnResult()
        {
            var handler = new FakeHandler();
            var repo = new FakeRepository();
            var runner = new BDDTestRunner(new[] { handler }, repo);
            var testCase = new TestCase
            {
                Should =
                {
                    new ExecutionTestCase { StoredProcedure = "sp" }
                }
            };

            var result = await runner.RunTestAsync(testCase);

            handler.CanHandleCalled.Should().BeTrue();
            handler.ExecuteCalled.Should().BeTrue();
            result.Passed.Should().BeTrue();
            result.Report.Should().Be("ok");
        }

        [Fact]
        public async Task GivenStoredProcedurePreConditionWhenRunTestAsyncExecutesStoredProcedure()
        {
            var handler = new FakeHandler();
            var repo = new FakeRepository();
            var runner = new BDDTestRunner(new[] { handler }, repo);
            var testCase = new TestCase
            {
                Mock = new MockBlock
                {
                    PreConditions = new List<MockQuery>
                    {
                        new MockQuery { Connection = "MainDb", Query = "sp_seed", Type = PreConditionType.StoredProcedure }
                    }
                },
                Should = { new ExecutionTestCase { StoredProcedure = "sp" } }
            };

            await runner.RunTestAsync(testCase);

            repo.StoredProcedures.Should().Contain("sp_seed");
        }

        [Fact]
        public async Task GivenJsonWhenRunTestAsyncShouldDeserializeThenExecute()
        {
            var handler = new FakeHandler();
            var repo = new FakeRepository();
            var runner = new BDDTestRunner(new[] { handler }, repo);
            var json = "{\"should\":[{\"type\":\"ExecutionTestCase\",\"storedProcedure\":\"sp\"}]}";

            var result = await runner.RunTestAsync(json);

            handler.CanHandleCalled.Should().BeTrue();
            handler.ExecuteCalled.Should().BeTrue();
            result.Passed.Should().BeTrue();
            result.Report.Should().Be("ok");
        }

        [Fact]
        public async Task GivenComplexJsonWhenRunTestAsyncShouldMaterializeStoredProcedureCompareCase()
        {
            var handler = new FakeHandler();
            var repo = new FakeRepository();
            var runner = new BDDTestRunner(new[] { handler }, repo);
            var json = @"{
  ""describe"": ""User report comparison"",
  ""context"": ""Filtering users by region"",
  ""mock"": {
    ""preConditions"": [
      {
        ""connection"": ""MainDb"",
        ""query"": ""INSERT INTO Users ...""
      }
    ]
  },
  ""should"": [
    {
      ""testName"": ""Compare SP outputs"",
      ""type"": ""StoredProcedureCompareTestCase"",
      ""storedProcedure"": ""sp_Main"",
      ""expectedProcedure"": {
        ""storedProcedure"": ""sp_Expected"",
        ""connectionName"": ""ExpectedDb""
      }
    }
  ]
}";

            var result = await runner.RunTestAsync(json);

            handler.CanHandleCalled.Should().BeTrue();
            handler.ExecuteCalled.Should().BeTrue();
            var compare = handler.ExecutedTestCase.Should().BeOfType<StoredProcedureCompareTestCase>().Subject;
            compare.StoredProcedure.Should().Be("sp_Main");
            compare.ExpectedStoredProcedure.Should().Be("sp_Expected");
            compare.ExpectedConnection.Should().Be("ExpectedDb");
            result.Passed.Should().BeTrue();
            result.Report.Should().Be("ok");
        }

        [Fact]
        public async Task GivenJsonWithMultipleShouldWhenRunTestAsyncShouldDeserializeAll()
        {
            var handler = new FakeHandler();
            var repo = new FakeRepository();
            var runner = new BDDTestRunner(new[] { handler }, repo);
            var json = @"{
  ""describe"": ""User report comparison"",
  ""context"": ""Filtering users by region"",
  ""mock"": {
    ""preConditions"": [
      {
        ""connection"": ""MainDb"",
        ""query"": ""INSERT INTO Users ...""
      }
    ]
  },
  ""should"": [
    {
      ""testName"": ""Compare SP outputs"",
      ""type"": ""StoredProcedureCompareTestCase"",
      ""storedProcedure"": ""sp_Main"",
      ""expectedProcedure"": {
        ""storedProcedure"": ""sp_Expected"",
        ""connectionName"": ""ExpectedDb""
      }
    },
    {
      ""testName"": ""Compare SP2 outputs"",
      ""type"": ""StoredProcedureCompareTestCase"",
      ""storedProcedure"": ""sp_Main2"",
      ""expectedProcedure"": {
        ""storedProcedure"": ""sp_Expected2"",
        ""connectionName"": ""ExpectedDb""
      }
    }
  ]
}";

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new BaseTestCaseJsonConverter());
            var deserialized = JsonSerializer.Deserialize<TestCase>(json, options)!;

            deserialized.Describe.Should().Be("User report comparison");
            deserialized.Context.Should().Be("Filtering users by region");
            deserialized.Mock.Should().NotBeNull();
            deserialized.Mock!.PreConditions.Should().ContainSingle();
            var pre = deserialized.Mock.PreConditions![0];
            pre.Connection.Should().Be("MainDb");
            pre.Query.Should().Be("INSERT INTO Users ...");
            pre.Type.Should().Be(PreConditionType.Query);
            deserialized.Should.Should().HaveCount(2);

            var result = await runner.RunTestAsync(json);

            handler.ExecutedCases.Should().HaveCount(2);
            handler.ExecutedCases[0].Should().BeOfType<StoredProcedureCompareTestCase>();
            handler.ExecutedCases[1].Should().BeOfType<StoredProcedureCompareTestCase>();
            var c1 = (StoredProcedureCompareTestCase)handler.ExecutedCases[0];
            c1.StoredProcedure.Should().Be("sp_Main");
            c1.ExpectedStoredProcedure.Should().Be("sp_Expected");
            var c2 = (StoredProcedureCompareTestCase)handler.ExecutedCases[1];
            c2.StoredProcedure.Should().Be("sp_Main2");
            c2.ExpectedStoredProcedure.Should().Be("sp_Expected2");
            result.Report.Should().Be("ok\nok");
            result.Passed.Should().BeTrue();
            repo.Queries.Should().ContainSingle(q => q == "INSERT INTO Users ...");
        }
    }
}
