using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.DependencyInjection
{
    public abstract class DependencyInjectionTestFramework : XunitTestFramework
    {
        protected DependencyInjectionTestFramework(IMessageSink messageSink) : base(messageSink) { }

        protected sealed override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            var builder = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                .ConfigureServices(services => services.AddSingleton<ITestOutputHelperAccessor, TestOutputHelperAccessor>())
                .ConfigureServices(services => ConfigureServices(assemblyName, services));

            ConfigureHost(builder);

            var host = builder.Build();
            using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                Configure(scope.ServiceProvider);

            return new DependencyInjectionTestFrameworkExecutor(host,
                assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }

        protected virtual void ConfigureHost(IHostBuilder builder) { }

        protected virtual void ConfigureServices(AssemblyName assemblyName, IServiceCollection services) => ConfigureServices(services);

        protected abstract void ConfigureServices(IServiceCollection services);

        protected virtual void Configure(IServiceProvider provider) { }
    }
}
