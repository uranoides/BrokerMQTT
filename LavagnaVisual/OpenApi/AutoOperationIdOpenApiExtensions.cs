using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Reflection;

namespace LavagnaVisual.OpenApi
{
    public static class AutoOperationIdOpenApiExtensions
    {
        public static OpenApiOptions AddOperationIDSpecs(this OpenApiOptions options)
        {
            options.AddOperationTransformer<AutoOperationIdTransformer>();
            options.AddDocumentTransformer<OperationIdUniquenessTransformer>();

            return options;
        }
    }

    /// <summary>
    /// An OpenAPI operation transformer that automatically sets the OperationId for API endpoints.
    /// It derives the OperationId from the name of the handler method, prioritizing
    /// explicitly set OperationIds (e.g., via .WithName() or [OpenApiName]).
    /// </summary>
    public class AutoOperationIdTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            // If the OperationId is already explicitly set, leave it as is.
            // This allows manual overrides (e.g., using .WithName() or [OpenApiName] attribute).
            if (!string.IsNullOrEmpty(operation.OperationId))
            {
                return Task.CompletedTask;
            }

            // Attempt to retrieve the MethodInfo for the endpoint handler.
            // For Minimal APIs, the handler's MethodInfo is typically found in the EndpointMetadata.
            // For Controller-based APIs, it's available through ControllerActionDescriptor.
            MethodInfo? handlerMethod = null;

            // Try to get MethodInfo for Minimal APIs
            // EndpointMetadata contains the delegate's MethodInfo
            if (context.Description?.ActionDescriptor?.EndpointMetadata != null)
            {
                handlerMethod = context.Description.ActionDescriptor.EndpointMetadata
                                    .OfType<MethodInfo>()
                                    .FirstOrDefault();
            }

            // Fallback: Try to get MethodInfo for Controller-based APIs
            if (handlerMethod == null && context.Description?.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                handlerMethod = controllerActionDescriptor.MethodInfo;
            }

            // If no handler method could be found, throw an exception.
            // This helps identify misconfigured endpoints during application startup.
            if (handlerMethod == null)
            {
                var path = context.Description?.RelativePath ?? "Unknown Path";
                throw new ApplicationException($"Cannot get an operation handler for endpoint '{path}'. " +
                                               "Ensure it's correctly configured or explicitly named.");
            }

            // Set the OperationId using the handler method's name.
            // Remove "Async" suffix if present for cleaner OperationIds.
            string operationId = handlerMethod.Name;
            if (operationId.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
            {
                operationId = operationId.Substring(0, operationId.Length - "Async".Length);
            }
            operation.OperationId = operationId;


            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// An OpenAPI document transformer that validates the uniqueness of OperationIds across the entire
    /// generated OpenAPI document. It throws an exception if any duplicate OperationIds are found,
    /// ensuring the generated specification is valid and client generators function correctly.
    /// </summary>
    public class OperationIdUniquenessTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            // Collect all OperationIds present in the document.
            var allOperationIds = document.Paths.Values
                .SelectMany(pathItem => pathItem.Operations.Values)
                .Where(op => !string.IsNullOrEmpty(op.OperationId)) // Only consider operations that have an OperationId
                .Select(op => op.OperationId)
                .ToList();

            // Find any duplicate OperationIds.
            var duplicateOperationIds = allOperationIds
                .GroupBy(id => id)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateOperationIds.Any())
            {
                // If duplicates are found, construct a detailed error message.
                var conflictDetails = duplicateOperationIds.Select(duplicateId =>
                {
                    // Find the paths associated with this duplicate OperationId.
                    var pathsWithConflict = document.Paths
                        .Where(pair => pair.Value.Operations.Values.Any(op => op.OperationId == duplicateId))
                        .Select(pair => pair.Key); // Get only the path string

                    return $"  - '{duplicateId}' found at paths: {string.Join(", ", pathsWithConflict)}";
                });

                throw new ApplicationException(
                   $"""
               OpenAPI document generation failed due to duplicate Operation IDs:
               {string.Join("\n", conflictDetails)}
               Ensure all Minimal API handlers have unique names, or use .WithName() or [OpenApiName] to explicitly set unique Operation IDs.
               """
                );
            }

            return Task.CompletedTask;
        }
    }
}
