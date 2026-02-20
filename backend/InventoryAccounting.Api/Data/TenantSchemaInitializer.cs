using Microsoft.EntityFrameworkCore;

namespace InventoryAccounting.Api.Data;

public static class TenantSchemaInitializer
{
    public static async Task EnsureSchemaCreatedAsync(AppDbContext db, string schemaName, CancellationToken cancellationToken = default)
    {
        await db.Database.ExecuteSqlRawAsync($"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", cancellationToken);

        await db.Database.ExecuteSqlRawAsync($@"
CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"Products\" (
    \"Id\" uuid PRIMARY KEY,
    \"Sku\" varchar(64) NOT NULL UNIQUE,
    \"Name\" varchar(256) NOT NULL,
    \"UnitCost\" numeric(18,2) NOT NULL,
    \"UnitPrice\" numeric(18,2) NOT NULL,
    \"IsActive\" boolean NOT NULL,
    \"CreatedAtUtc\" timestamp with time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"InventoryTransactions\" (
    \"Id\" uuid PRIMARY KEY,
    \"ProductId\" uuid NOT NULL REFERENCES \"{schemaName}\".\"Products\"("Id"),
    \"Quantity\" numeric(18,2) NOT NULL,
    \"TransactionType\" varchar(16) NOT NULL,
    \"Reference\" text NOT NULL,
    \"TransactionDateUtc\" timestamp with time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"Accounts\" (
    \"Id\" uuid PRIMARY KEY,
    \"Code\" varchar(32) NOT NULL UNIQUE,
    \"Name\" varchar(128) NOT NULL,
    \"Category\" varchar(32) NOT NULL,
    \"IsActive\" boolean NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"Customers\" (
    \"Id\" uuid PRIMARY KEY,
    \"Name\" text NOT NULL,
    \"Email\" text NOT NULL,
    \"Phone\" text NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"Suppliers\" (
    \"Id\" uuid PRIMARY KEY,
    \"Name\" text NOT NULL,
    \"ContactName\" text NOT NULL,
    \"Email\" text NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"JournalEntries\" (
    \"Id\" uuid PRIMARY KEY,
    \"EntryDateUtc\" timestamp with time zone NOT NULL,
    \"Memo\" text NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"JournalEntryLines\" (
    \"Id\" uuid PRIMARY KEY,
    \"JournalEntryId\" uuid NOT NULL REFERENCES \"{schemaName}\".\"JournalEntries\"("Id") ON DELETE CASCADE,
    \"AccountId\" uuid NOT NULL REFERENCES \"{schemaName}\".\"Accounts\"("Id"),
    \"Debit\" numeric(18,2) NOT NULL,
    \"Credit\" numeric(18,2) NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"Invoices\" (
    \"Id\" uuid PRIMARY KEY,
    \"Number\" text NOT NULL,
    \"CustomerId\" uuid NULL REFERENCES \"{schemaName}\".\"Customers\"("Id"),
    \"InvoiceDateUtc\" timestamp with time zone NOT NULL,
    \"TotalAmount\" numeric(18,2) NOT NULL,
    \"Status\" text NOT NULL
);

CREATE TABLE IF NOT EXISTS \"{schemaName}\".\"InvoiceLines\" (
    \"Id\" uuid PRIMARY KEY,
    \"InvoiceId\" uuid NOT NULL REFERENCES \"{schemaName}\".\"Invoices\"("Id") ON DELETE CASCADE,
    \"ProductId\" uuid NOT NULL REFERENCES \"{schemaName}\".\"Products\"("Id"),
    \"Quantity\" numeric(18,2) NOT NULL,
    \"UnitPrice\" numeric(18,2) NOT NULL,
    \"LineTotal\" numeric(18,2) NOT NULL
);
", cancellationToken);
    }
}
