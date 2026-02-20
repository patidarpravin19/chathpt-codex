using InventoryAccounting.Api.Contracts;
using Microsoft.Extensions.Options;

namespace InventoryAccounting.Api.Services;

public interface ITenantProvider
{
    string GetTenant();
}

public class TenantProvider(IHttpContextAccessor accessor, IOptions<TenantOptions> options) : ITenantProvider
{
    private readonly IHttpContextAccessor _accessor = accessor;
    private readonly TenantOptions _options = options.Value;

    public string GetTenant()
    {
        var requestedTenant = _accessor.HttpContext?.Request.Headers["X-Tenant"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(requestedTenant))
        {
            return _options.DefaultTenant;
        }

        return SanitizeSchemaName(requestedTenant);
    }

    private static string SanitizeSchemaName(string input)
    {
        var chars = input.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
        var schema = new string(chars).ToLowerInvariant();
        return string.IsNullOrWhiteSpace(schema) ? "public" : schema;
    }
}
