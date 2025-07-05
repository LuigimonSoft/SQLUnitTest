using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SQLUnitTest.DependencyInjection;
using SQLUnitTest.Reporting;
using SQLUnitTest.Repositories;
using SQLUnitTest.Services;
using SQLUnitTest.Services.Handlers;
using Xunit;

namespace SQLUnitTest.Tests.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddSqlUnitTest_RegistersDependencies()
        {
            var services = new ServiceCollection();
            services.AddSqlUnitTest(new Dictionary<string, string> { { "Default", "Server=.;Database=Test;Trusted_Connection=True;" } });
            var provider = services.BuildServiceProvider();

            provider.GetService<IDbRepository>().Should().BeOfType<AdoDbRepository>();
            provider.GetService<IMarkdownReporter>().Should().BeOfType<MarkdownReporter>();
            provider.GetService<ITestRunner>().Should().BeOfType<BDDTestRunner>();
            provider.GetServices<ITestCaseHandler>().Should().ContainSingle(h => h is ExecutionTestCaseHandler);
        }
    }
}
