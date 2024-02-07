using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Resources;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using OpenTelemetry.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XRAyWebAPI31
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); //refer to https://opentelemetry.io/docs/languages/net/exporters/#note-for-net-core-31-and-below-and-grpc

            services.AddControllers();
            services.AddOpenTelemetry()
                .WithTracing(builder => builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AWSOtelWebAPI31"))
                    .AddXRayTraceId() // for generating AWS X-Ray compliant trace IDs
                    .AddOtlpExporter() // to export to OpenTelemetry Collector
                    .AddAspNetCoreInstrumentation() // to trace the ASPNetCore
                    .AddHttpClientInstrumentation() // to trace libraries for HTTPClient. As RestSharp is a wrapper for HTTPClient, it is somewhat supported too
                    .AddSqlClientInstrumentation() // to trace libraries for SQLClient
                    .AddAWSInstrumentation()); // to trace downstream AWS services such as S3 or DynamoDB calls

            Sdk.SetDefaultTextMapPropagator(new AWSXRayPropagator());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
