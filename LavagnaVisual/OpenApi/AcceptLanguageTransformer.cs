using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;

namespace LavagnaVisual.OpenApi
{
    public static class AcceptLanguageOpenApiExtensions
    {
        public static OpenApiOptions AddAcceptLanguageSpecs(this OpenApiOptions options)
        {
            options.AddOperationTransformer<AcceptLanguageOperationTransformer>();

            options.AddDocumentTransformer<AcceptLanguageDocumentTransformer>();

            return options;
        }
    }

    public class AcceptLanguageDocumentTransformer(IOptions<RequestLocalizationOptions> requestLocalizationOptions) : IOpenApiDocumentTransformer
    {
        public const string ACCEPT_LANGUAGE_PARAMETER_ID = "AcceptLanguageParameter";
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {

            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Default = "en-US"
            };

            var supportedCultures = requestLocalizationOptions.Value.SupportedCultures;
            if (supportedCultures is not null)
            {
                schema.Enum = [];
                foreach (var culture in supportedCultures)
                {
                    schema.Enum.Add(culture.Name);
                }
            }

            document.Components ??= new OpenApiComponents();
            document.Components.Parameters ??= new Dictionary<string, IOpenApiParameter>();
            document.Components.Parameters.Add(ACCEPT_LANGUAGE_PARAMETER_ID, new OpenApiParameter
            {
                In = ParameterLocation.Header,
                Name = HeaderNames.AcceptLanguage,
                Description = "accepted language in API responses",
                Required = false,
                Schema = schema
            });

            return Task.CompletedTask;
        }

    }

    public class AcceptLanguageOperationTransformer() : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            operation.Parameters ??= [];
            operation.Parameters.Add(new OpenApiParameterReference(AcceptLanguageDocumentTransformer.ACCEPT_LANGUAGE_PARAMETER_ID));

            return Task.CompletedTask;
        }
    }
}
