namespace InventoryAccounting.Api.Contracts;

public class TenantOptions
{
    public const string SectionName = "Tenant";
    public string DefaultTenant { get; set; } = "public";
}
