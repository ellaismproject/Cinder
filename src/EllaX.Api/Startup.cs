﻿using System;
using Autofac;
using AutoMapper;
using EllaX.Api.Infrastructure.Hosting;
using EllaX.Data;
using EllaX.Data.Options;
using EllaX.Logic;
using EllaX.Logic.Clients;
using EllaX.Logic.Clients.Options;
using EllaX.Logic.Options;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace EllaX.Api
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
            // database
            services.Configure<RepositoryOptions>(options =>
                options.ConnectionString = Configuration.GetConnectionString("RepositoryConnection"));
            services.Configure<LocationOptions>(options =>
                options.ConnectionString = Configuration.GetConnectionString("GeoIpConnection"));
            services.Configure<BlockchainClientOptions>(Configuration.GetSection("Blockchain"));
            services.Configure<NetworkClientOptions>(Configuration.GetSection("Network"));

            services.AddHttpClient<IBlockchainClient, BlockchainClient>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
            services.AddMediatR();
            services.AddAutoMapper(cfg => cfg.AddProfiles(typeof(Service)));

            // hosted services
            services.AddHostedService<NetworkHealthHostedService>();

            // mvc
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<Repository>().SingleInstance();
            builder.RegisterType<LocationService>().As<ILocationService>().SingleInstance();

            builder.RegisterType<BlockchainService>().As<IBlockchainService>().InstancePerLifetimeScope();
            builder.RegisterType<PeerService>().As<IPeerService>().InstancePerLifetimeScope();
            builder.RegisterType<StatisticsService>().As<IStatisticsService>().InstancePerLifetimeScope();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(options => options.AllowAnyOrigin());
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
