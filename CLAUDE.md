# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from `UI/`:

```bash
npm i            # Install dependencies
npm run dev      # Dev server on port 3000 (auto-opens browser)
npm run build    # Production build → /UI/build/
```

No test framework is configured.

## Architecture

**PromiPoints** is a corporate peer recognition/points system. Frontend SPA backed by an ASP.NET Core 8 Web API with SQL Server.

### Key files

- `UI/src/App.tsx` — root; uses `useAuth` hook; wraps `ToastProvider`; role-based routing
- `UI/src/hooks/useAuth.ts` — auth bootstrap hook (token validation, login/logout state)
- `UI/src/config/api/promiApi.ts` — Axios instance + JWT interceptor
- `UI/src/config/adapters/` — one adapter per swappable library (animation, chart, icons, toast)
- `UI/src/infrastructure/interfaces/index.ts` — domain types (`User`, `MonthlyAllocation`, `PointAssignment`, `Category`)
- `UI/src/infrastructure/mappers/api.mappers.ts` — maps backend int IDs → frontend string IDs
- `UI/src/infrastructure/helpers/date.ts` — `getCurrentMonth()`
- `UI/src/actions/{domain}/` — all API calls grouped by domain (auth, users, allocations, assignments)
- `UI/src/presentation/screens/` — one folder per screen (auth, employee, hr)
- `UI/src/presentation/components/shared/` — reusable UI components (MobileNav, SkeletonLoader, ImageWithFallback)
- `UI/src/presentation/providers/ToastProvider.tsx` — Sonner `<Toaster />` wrapper
- `UI/src/types/index.ts` — re-exports from `infrastructure/interfaces` (backward compat)

### Data models

```typescript
User { id: string, name, email, role: 'employee' | 'people', department }
MonthlyAllocation { userId: string, month: 'YYYY-MM', pointsRemaining, pointsReceived }
PointAssignment { id: string, fromUserId, toUserId, points, category, message?, timestamp, month }
```

### Business rules

- Each user gets **10 points per month** to assign to peers
- 8 predefined recognition categories
- Role `people` sees admin/HR dashboards; role `employee` sees the standard dashboard
- Authentication via JWT (stored in `localStorage` under `promipoints_token`)
- Demo users use Spanish names/departments; UI text is in Spanish

### UI stack

- React 18 + TypeScript, built with Vite (SWC compiler)
- Tailwind CSS v4 + shadcn/ui components (in `UI/src/components/ui/`)
- Recharts for data visualization, Motion (Framer) for animations, Sonner for toasts
- `@` alias resolves to `UI/src/`

### Conventions

- `UI/src/components/ui/` contains shadcn/ui primitives — do not edit these manually; use the shadcn CLI or copy updated versions
- `UI/src/config/adapters/` — import library exports only from adapters, never directly from `lucide-react`, `recharts`, `motion/react`, or `sonner` in screen/component files
- Tailwind class merging uses `cn()` helper (`clsx` + `tailwind-merge`) from `@/components/ui/utils`
- No global state manager; state flows via React hooks and props
- Backend uses `int` IDs; frontend uses `string` IDs — mapping happens in `api.mappers.ts`
