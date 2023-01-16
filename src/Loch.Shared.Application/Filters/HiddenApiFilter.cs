using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Loch.Shared.Application.Filters;
    /// <summary>
    /// Custom Swagger Hidden Filter
    /// </summary>
public class HiddenApiFilter : IDocumentFilter
{
    /// <summary>
    /// Hidden swagger interface feature identification
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HiddenApiAttribute : Attribute
    {
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var apiDescription in context.ApiDescriptions)
        {
            if (!apiDescription.TryGetMethodInfo(out var method)) continue;
            if (method?.ReflectedType != null && method.ReflectedType.CustomAttributes.All(t => t.AttributeType != typeof(HiddenApiAttribute)) && method.CustomAttributes.All(t => t.AttributeType != typeof(HiddenApiAttribute))) continue;
            var key = "/" + apiDescription.RelativePath;
            if (key.Contains("?"))
            {
                var idx = key.IndexOf("?", StringComparison.Ordinal);
                key = key.Substring(0, idx);
            }
            swaggerDoc.Paths.Remove(key);
        }
    }
}
