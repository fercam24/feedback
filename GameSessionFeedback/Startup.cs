using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using GameSessionFeedback.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GameSessionFeedback
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
            services.Configure<FeedbackDatabaseSettings>(Configuration.GetSection(nameof(FeedbackDatabaseSettings)));
            services.AddSingleton<IFeedbackDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<FeedbackDatabaseSettings>>().Value);

            services.Configure<GameSessionFeedbackProperties>(
                Configuration.GetSection(nameof(GameSessionFeedbackProperties)));
            services.AddSingleton<IGameSessionFeedbackProperties>(sp =>
                sp.GetRequiredService<IOptions<GameSessionFeedbackProperties>>().Value);

            services.AddSingleton<IMongoDbContext, MongoDbContext>();
            services.AddSingleton<IFeedbackService, FeedbackService>();

            services.AddHostedService<ConfigureDbIndexesService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}