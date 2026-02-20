import { useEffect, useState } from 'react';
import { apiGet } from '../api/client';
import type { Dashboard } from '../types';

export function DashboardPage({ tenant }: { tenant: string }) {
  const [data, setData] = useState<Dashboard | null>(null);

  useEffect(() => {
    apiGet<Dashboard>('/dashboard', tenant).then(setData).catch(() => setData(null));
  }, [tenant]);

  if (!data) return <p>Loading dashboard...</p>;

  return (
    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, minmax(160px, 1fr))', gap: 12 }}>
      <Metric title="Tenant" value={data.tenant} />
      <Metric title="Products" value={String(data.productCount)} />
      <Metric title="Stock Value" value={`$${data.stockValue.toFixed(2)}`} />
      <Metric title="Revenue" value={`$${data.totalRevenue.toFixed(2)}`} />
      <Metric title="Unpaid Invoices" value={String(data.unpaidInvoices)} />
    </div>
  );
}

function Metric({ title, value }: { title: string; value: string }) {
  return (
    <div style={{ border: '1px solid #ddd', borderRadius: 8, padding: 16 }}>
      <h3 style={{ marginTop: 0 }}>{title}</h3>
      <p style={{ fontSize: 20, marginBottom: 0 }}>{value}</p>
    </div>
  );
}
