// Keeps only the getCurrentMonth helper used across components.
// All data persistence is now handled via api.ts (HTTP calls to the backend).

export const getCurrentMonth = (): string => {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
};
