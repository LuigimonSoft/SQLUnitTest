using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SQLUnitTest.Repositories;
using SQLUnitTest.Reporting;
using SQLUnitTest.Services;
using SQLUnitTest.Services.Handlers;

namespace SQLUnitTest.DependencyInjection
{
    /// <summary>
    /// Registers SQLUnitTest services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlUnitTest(this IServiceCollection services, IDictionary<string, string> connections)
        {
            services.AddSingleton<IDbRepository>(new AdoDbRepository(connections));
            services.AddSingleton<IMarkdownReporter, MarkdownReporter>();
            services.AddTransient<ITestCaseHandler, ExecutionTestCaseHandler>();
            services.AddTransient<ITestRunner, BDDTestRunner>();
            return services;
        }
    }
}
