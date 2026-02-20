using InventoryAccounting.Api.Contracts;
using InventoryAccounting.Api.Data;
using InventoryAccounting.Api.Models;
using InventoryAccounting.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TenantOptions>(builder.Configuration.GetSection(TenantOptions.SectionName));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/dashboard", async (AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    var tenant = tenantProvider.GetTenant();
    await UseTenantAsync(db, tenant, ct);

    var stockValue = await db.Products.SumAsync(p => p.UnitCost, ct);
    var totalRevenue = await db.Invoices.SumAsync(i => i.TotalAmount, ct);
    var productCount = await db.Products.CountAsync(ct);
    var unpaidInvoices = await db.Invoices.CountAsync(i => i.Status != "Paid", ct);

    return Results.Ok(new { tenant, stockValue, totalRevenue, productCount, unpaidInvoices });
});

app.MapGet("/api/products", async (AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    var products = await db.Products.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    return Results.Ok(products);
});

app.MapPost("/api/products", async (Product product, AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    db.Products.Add(product);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapGet("/api/accounts", async (AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    var accounts = await db.Accounts.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
    return Results.Ok(accounts);
});

app.MapPost("/api/accounts", async (Account account, AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    db.Accounts.Add(account);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/accounts/{account.Id}", account);
});

app.MapGet("/api/invoices", async (AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    var invoices = await db.Invoices
        .AsNoTracking()
        .Include(x => x.Lines)
        .OrderByDescending(x => x.InvoiceDateUtc)
        .ToListAsync(ct);

    return Results.Ok(invoices);
});

app.MapPost("/api/invoices", async (Invoice invoice, AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    invoice.TotalAmount = invoice.Lines.Sum(x => x.LineTotal);
    db.Invoices.Add(invoice);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/invoices/{invoice.Id}", invoice);
});

app.MapPost("/api/inventory/transactions", async (InventoryTransaction trx, AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    db.InventoryTransactions.Add(trx);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/inventory/transactions/{trx.Id}", trx);
});

app.MapPost("/api/journal-entries", async (JournalEntry entry, AppDbContext db, ITenantProvider tenantProvider, CancellationToken ct) =>
{
    await UseTenantAsync(db, tenantProvider.GetTenant(), ct);
    var debit = entry.Lines.Sum(x => x.Debit);
    var credit = entry.Lines.Sum(x => x.Credit);

    if (debit != credit)
    {
        return Results.BadRequest(new { error = "Journal entry is unbalanced." });
    }

    db.JournalEntries.Add(entry);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/journal-entries/{entry.Id}", entry);
});

app.Run();

static async Task UseTenantAsync(AppDbContext db, string tenant, CancellationToken ct)
{
    await TenantSchemaInitializer.EnsureSchemaCreatedAsync(db, tenant, ct);
    await db.Database.ExecuteSqlRawAsync($"SET search_path TO \"{tenant}\", public;", ct);
}
