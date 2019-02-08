using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookService.DbContext;
using BookService.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        IServiceCollection _services;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            services.AddMvc()
                .AddNewtonsoftJson();
            var URI = Configuration.GetSection("CosmosSettings:URI").Value;
            var PrimaryKey = Configuration.GetSection("CosmosSettings:PrimaryKey").Value;
            var DatabaseName = Configuration.GetSection("CosmosSettings:DatabaseName").Value;

            services.AddDbContext<BookdbContext>(options => options.UseCosmos(URI,PrimaryKey,DatabaseName));

            services.AddTransient<BookService.Service.IBookService, BookService.Service.BookService>();

            services.AddLogging(options => options.AddDebug().SetMinimumLevel(LogLevel.Trace));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting(routes =>
            {
                routes.MapApplication();
            });

            app.UseAuthorization();

            BookService.Service.IBookService IbookService= _services.BuildServiceProvider().GetRequiredService<BookService.Service.IBookService>();
            IbookService.CreateTheDatabaseAsync();
            IbookService.InitAsync();

        }
    }
}
