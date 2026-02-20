export type Dashboard = {
  tenant: string;
  stockValue: number;
  totalRevenue: number;
  productCount: number;
  unpaidInvoices: number;
};

export type Product = {
  id: string;
  sku: string;
  name: string;
  unitCost: number;
  unitPrice: number;
  isActive: boolean;
};

export type Account = {
  id: string;
  code: string;
  name: string;
  category: string;
  isActive: boolean;
};

export type Invoice = {
  id: string;
  number: string;
  invoiceDateUtc: string;
  totalAmount: number;
  status: string;
};
