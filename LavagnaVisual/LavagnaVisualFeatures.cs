using Asp.Versioning;
using LavagnaVisual.Features.Recipes;
using LavagnaVisual.Features.Recipes.Services;

namespace LavagnaVisual
{
    public static class LavagnaVisualFeatures
    {
        public static RouteGroupBuilder MapLavagnaVisualEndpoints(this WebApplication app)
        {
            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder group = app.MapGroup("api")
                .WithApiVersionSet(versionSet)
                .DisableAntiforgery();

            group.MapRecipesEndpoints();

            return group;
        }

        public static IServiceCollection AddLavagnaVisualApiServices(this IServiceCollection services)
        {
            services.AddAndConfigApiVersioning();
            services.AddScoped<RecipeServiceProvider>();

            return services;
        }

        public static IServiceCollection AddAndConfigApiVersioning(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1.0);
                    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                })
            .EnableApiVersionBinding();

            return services;
        }
    }
}
