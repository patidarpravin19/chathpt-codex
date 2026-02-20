import { useEffect, useState } from 'react';
import { apiGet } from '../api/client';
import type { Product } from '../types';

export function ProductsPage({ tenant }: { tenant: string }) {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    apiGet<Product[]>('/products', tenant).then(setProducts).catch(() => setProducts([]));
  }, [tenant]);

  return (
    <section>
      <h2>Products</h2>
      <table width="100%" cellPadding={8}>
        <thead>
          <tr>
            <th align="left">SKU</th>
            <th align="left">Name</th>
            <th align="right">Cost</th>
            <th align="right">Price</th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td>{product.sku}</td>
              <td>{product.name}</td>
              <td align="right">{product.unitCost.toFixed(2)}</td>
              <td align="right">{product.unitPrice.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
