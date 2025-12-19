using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Nodes;

namespace LavagnaVisual.OpenApi
{
    public static class TagsOpenApiExtensions
    {
        public static OpenApiOptions DescribeTags<T>(this OpenApiOptions options)
        {
            options.AddDocumentTransformer<TagDescriptionsTransformer<T>>();

            return options;
        }
    }
    public class TagDescriptionsTransformer<T> : IOpenApiDocumentTransformer
    {
        record TagDefinition(string Name, string Value, string? Description, string? Category);

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var tags = LoadTagDefinitions(typeof(T));

            tags.Select(tag => new OpenApiTag
            {
                Name = tag.Value,
                Description = tag.Description ?? string.Empty,
            }).ToHashSet();

            var tagGroups = tags.GroupBy(tag => tag.Category ?? "Misc");

            if (tagGroups.Any())
            {
                var xTagGroups = new JsonArray(tagGroups.OrderBy(g => g.Key).Select(g =>
                {
                    var xTagGroupTags = new JsonArray(g.Select(t => t.Value)
                       .Distinct()
                       .OrderBy(s => s)
                       .Select(s => JsonValue.Create<string>(s))
                       .ToArray());

                    return new JsonObject
                    {
                        ["name"] = g.Key,
                        ["tags"] = xTagGroupTags
                    };
                }).ToArray());

                document.Extensions ??= new Dictionary<string, IOpenApiExtension>();
                document.Extensions.Add("x-tagGroups", new JsonNodeExtension(xTagGroups));
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<TagDefinition> LoadTagDefinitions(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var tags = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
               .Select(t =>
               {
                   var name = t.Name;
                   var value = Convert.ToString(t.GetRawConstantValue());
                   var description = t.GetCustomAttribute<DescriptionAttribute>()?.Description;
                   var category = t.GetCustomAttribute<CategoryAttribute>()?.Category;
                   if (string.IsNullOrWhiteSpace(value))
                       throw new ApplicationException(
                       $""" 
                        OpenAPI document generation failed due Tag definition with invalid value:{name}={Convert.ToString(value)}]
                        Ensure all tag definition have unique value. 
                        """
                    );
                   return new TagDefinition(name, value, description, category);
               });

            var duplicatedTag = tags
               .GroupBy(tag => tag.Value)
               .Where(group => group.Count() > 1)
               .Select(group => group.Key)
               .ToList();

            if (duplicatedTag.Any())
                throw new ApplicationException(
                    $""" 
                OpenAPI document generation failed due to duplicate Tag definition:[{string.Join(",", duplicatedTag)}]
                Ensure all tag definition have unique value. 
                """
                );

            return tags;
        }
    }
}
