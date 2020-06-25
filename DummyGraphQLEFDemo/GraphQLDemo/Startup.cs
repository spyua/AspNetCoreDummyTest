using GraphQL;
using GraphQLDemo.Contracts;
using GraphQLDemo.Entities.Context;
using GraphQLDemo.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using GraphQLDemo.GraphQL.GraphQLSchema;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json.Serialization;

namespace GraphQLDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // For Async IO Operations
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
           
            // �[�J�U�C�{��
            services.AddDbContext<ApplicationContext>(optionsAction: options => options.UseSqlServer(Configuration.GetConnectionString("sqlConString")),
             contextLifetime: ServiceLifetime.Singleton);

                
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            // ���UIDependencyResolver����
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));           
            // ���UAppSchema����
            services.AddScoped<AppSchema>();
            // ���UGraphQL�H��GraphTypes����A�䤤AddGraphTypes()�|�۰����ڭ̵��U�Ҧ�GraphQL Types�A�٤U�ڭ̭ӧO���U�C�@��GraphQL Type���ɶ��C
            services.AddGraphQL(o => { o.ExposeExceptions = false; }).AddGraphTypes(ServiceLifetime.Scoped);

            services.AddControllersWithViews();
            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseWebSockets();
            app.UseGraphQL<AppSchema>();
            //app.UseGraphQLWebSockets<AppSchema>();  
            app.UseGraphQLPlayground();
  
        }
    }
}
