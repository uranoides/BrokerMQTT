using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LavagnaVisual.OpenApi
{
    public static class SortDocumentOpenApiExtensions
    {
        public static OpenApiOptions SortDocument(this OpenApiOptions options)
        {
            options.AddDocumentTransformer<CanonicalDocumentTransformer>();

            return options;
        }
    }

    public class CanonicalDocumentTransformer : IOpenApiDocumentTransformer
    {
        private class OperationTypeComparer : IComparer<HttpMethod>
        {
            private static readonly List<HttpMethod> order = [
                HttpMethod.Get,
                HttpMethod.Post,
                HttpMethod.Put,
                HttpMethod.Patch,
                HttpMethod.Delete,
                HttpMethod.Head,
                HttpMethod.Options
            ];

            public int Compare(HttpMethod x, HttpMethod y)
            {
                return order.IndexOf(x).CompareTo(order.IndexOf(y));
            }
        }

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            // Sort the paths by key
            var sortedPaths = document.Paths.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
            document.Paths.Clear();
            foreach (var path in sortedPaths)
            {
                document.Paths.Add(path.Key, path.Value);
            }

            // Sort the operations of each path by type in this order: GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS
            var comparer = new OperationTypeComparer();
            foreach (var path in document.Paths)
            {
                var operations = path.Value.Operations;
                if (operations != null)
                {
                    var sortedOperations = operations.OrderBy(o => o.Key, comparer).ToDictionary(o => o.Key, o => o.Value);
                    operations.Clear();
                    foreach (var operation in sortedOperations)
                    {
                        operations.Add(operation.Key, operation.Value);
                    }
                }
            }

            // Sort the elements of the tags field by name
            if (document.Tags != null)
            {
                document.Tags.Clear();
                foreach (var tag in document.Tags.OrderBy(t => t.Name))
                {
                    document.Tags.Add(tag);
                }
            }

            return Task.CompletedTask;
        }
    }
}
