using LavagnaVisual;
using LavagnaVisual.OpenApi;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (!Environment.UserInteractive)
{
    builder.Host.UseWindowsService();
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin() // In produzione, specifica i tuoi domini Angular
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddLavagnaVisualApiServices();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Sacmi ERP Web API",
            Version = context.DocumentName,
            Description = "Sacmi ERP Web API",
            Contact = new OpenApiContact { Name = "SACMI", Email = "SACMI@org.com" },
            License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };
        return Task.CompletedTask;
    });
    options.DescribeTags<Tags>();
    options.SortDocument();
    options.AddAcceptLanguageSpecs();
    options.AddOperationIDSpecs();
});

var app = builder.Build();

app.MapOpenApi("/openapi/{documentName}.json");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.OpenApiRoutePattern = "/openapi/{documentName}.json";
    });
}

app.MapLavagnaVisualEndpoints();

app.UseRouting();

app.Run();
