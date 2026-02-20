import React, { useState } from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { DashboardPage } from './pages/DashboardPage';
import { ProductsPage } from './pages/ProductsPage';
import { AccountsPage } from './pages/AccountsPage';
import { InvoicesPage } from './pages/InvoicesPage';

function App() {
  const [tenant, setTenant] = useState('public');

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout tenant={tenant} onTenantChange={setTenant} />}>
          <Route index element={<DashboardPage tenant={tenant} />} />
          <Route path="products" element={<ProductsPage tenant={tenant} />} />
          <Route path="accounts" element={<AccountsPage tenant={tenant} />} />
          <Route path="invoices" element={<InvoicesPage tenant={tenant} />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
