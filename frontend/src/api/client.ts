const baseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api';

export async function apiGet<T>(path: string, tenant: string): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    headers: { 'X-Tenant': tenant }
  });

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`);
  }

  return response.json() as Promise<T>;
}
