import { useEffect, useState } from 'react';
import { apiGet } from '../api/client';
import type { Account } from '../types';

export function AccountsPage({ tenant }: { tenant: string }) {
  const [accounts, setAccounts] = useState<Account[]>([]);

  useEffect(() => {
    apiGet<Account[]>('/accounts', tenant).then(setAccounts).catch(() => setAccounts([]));
  }, [tenant]);

  return (
    <section>
      <h2>Chart of Accounts</h2>
      <ul>
        {accounts.map((account) => (
          <li key={account.id}>
            {account.code} - {account.name} ({account.category})
          </li>
        ))}
      </ul>
    </section>
  );
}
