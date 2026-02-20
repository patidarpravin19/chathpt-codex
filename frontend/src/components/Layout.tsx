import { Link, Outlet } from 'react-router-dom';

type Props = {
  tenant: string;
  onTenantChange: (value: string) => void;
};

export function Layout({ tenant, onTenantChange }: Props) {
  return (
    <div style={{ fontFamily: 'Arial, sans-serif', margin: '0 auto', maxWidth: 1080, padding: 24 }}>
      <h1>Inventory & Accounting Suite</h1>
      <p>Multi-tenant schema: pass tenant in request header.</p>
      <label>
        Tenant:&nbsp;
        <input value={tenant} onChange={(event) => onTenantChange(event.target.value)} />
      </label>
      <nav style={{ display: 'flex', gap: 12, margin: '16px 0' }}>
        <Link to="/">Dashboard</Link>
        <Link to="/products">Products</Link>
        <Link to="/accounts">Accounts</Link>
        <Link to="/invoices">Invoices</Link>
      </nav>
      <Outlet />
    </div>
  );
}
