import { useEffect, useState } from 'react';
import { apiGet } from '../api/client';
import type { Invoice } from '../types';

export function InvoicesPage({ tenant }: { tenant: string }) {
  const [invoices, setInvoices] = useState<Invoice[]>([]);

  useEffect(() => {
    apiGet<Invoice[]>('/invoices', tenant).then(setInvoices).catch(() => setInvoices([]));
  }, [tenant]);

  return (
    <section>
      <h2>Invoices</h2>
      <table width="100%" cellPadding={8}>
        <thead>
          <tr>
            <th align="left">Number</th>
            <th align="left">Date</th>
            <th align="left">Status</th>
            <th align="right">Amount</th>
          </tr>
        </thead>
        <tbody>
          {invoices.map((invoice) => (
            <tr key={invoice.id}>
              <td>{invoice.number}</td>
              <td>{new Date(invoice.invoiceDateUtc).toLocaleDateString()}</td>
              <td>{invoice.status}</td>
              <td align="right">{invoice.totalAmount.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
