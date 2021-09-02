using Hangfire;
using Hangfire.SqlServer;
using Hangfire.FluentNHibernateStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProCodeGuide.Samples.Hangfire.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Snork.FluentNHibernateTools;

namespace ProCodeGuide.Samples.Hangfire {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            //services.AddHangfire(configuration => configuration
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseFluentNHibernateJobStorage(Configuration.GetConnectionString("OracleConnection"), ProviderTypeEnum.OracleClient10Managed, new FluentNHibernateStorageOptions {
            //        TransactionIsolationLevel = System.Transactions.IsolationLevel.Serializable,
            //        QueuePollInterval = TimeSpan.FromSeconds(15),
            //        JobExpirationCheckInterval = TimeSpan.FromHours(1),
            //        CountersAggregateInterval = TimeSpan.FromMinutes(5),
            //        UpdateSchema = true,
            //        DashboardJobListLimit = 50000,
            //        InvisibilityTimeout = TimeSpan.FromMinutes(15),
            //        TransactionTimeout = TimeSpan.FromMinutes(1),
            //        DefaultSchema = "info_desenv_75", // use database provider's default schema
            //        TablePrefix = "Hangfire_"
            //    }));

            services.AddHangfireServer();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProCodeGuide.Samples.Hangfire", Version = "v1" });
            });

            services.AddTransient<IJobTestService, JobTestService>();
            services.AddSingleton<IDivision, Division>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProCodeGuide.Samples.Hangfire v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
            // A cada 15 segundos
            RecurringJob.AddOrUpdate(() => svp.GetService<IDivision>().DivisionRandom(), "*/15 * * * * *");
        }
    }
}
