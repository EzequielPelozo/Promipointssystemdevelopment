import axios from 'axios';

const TOKEN_KEY = 'promipoints_token';

export const promiApi = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '',
});

promiApi.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
