# Multi-tenant Inventory + Accounting Starter

This repository contains a starter implementation of **Inventory and Accounting software** built with:

- React + TypeScript frontend
- .NET 8 minimal API backend
- PostgreSQL database using **schema-per-tenant**

## Key features implemented

### Inventory
- Product catalog (SKU, cost, sell price, active/inactive)
- Inventory transactions endpoint (in/out adjustments)
- Dashboard stock metrics

### Accounting
- Chart of accounts
- Journal entry posting with debit/credit balancing validation
- Invoice and invoice lines with automatic total calculation
- Dashboard revenue and unpaid invoices metrics

### Multi-tenancy
- Tenant is passed via `X-Tenant` header.
- Backend sanitizes tenant name and maps it to PostgreSQL schema.
- On each request, schema is auto-created if missing and activated via `SET search_path`.

## Run backend

```bash
cd backend/InventoryAccounting.Api
dotnet restore
dotnet run
```

API Swagger: `http://localhost:5000/swagger`

## Run frontend

```bash
cd frontend
npm install
npm run dev
```

App: `http://localhost:5173`

## Suggested production hardening
- Add authentication/authorization and tenant-user mapping.
- Replace ad-hoc schema/table creation with EF Core migrations + tenant migration runner.
- Add inventory valuation logic (FIFO/Weighted Average), purchase orders, payments, taxes, and bank reconciliation.
- Add background jobs, audit logging, and automated tests.
