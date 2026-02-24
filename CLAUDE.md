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

**PromiPoints** is a corporate peer recognition/points system — a pure frontend SPA with no backend or API. All data lives in `localStorage`.

### Key files

- `UI/src/App.tsx` — root component; handles routing logic (login → onboarding → role-based dashboard)
- `UI/src/utils/storage.ts` — all data access; initializes demo data on first load; uses `promipoints_*` namespace keys
- `UI/src/types/index.ts` — all TypeScript interfaces (`User`, `MonthlyAllocation`, `PointAssignment`)
- `UI/src/components/UserDashboard.tsx` — employee view (assign points, view received recognitions)
- `UI/src/components/PeopleDashboard.tsx` — HR/admin view (analytics, charts, CSV export)
- `UI/src/components/AssignPoints.tsx` — modal for assigning recognition points

### Data models

```typescript
User { id, name, email, role: 'employee' | 'people', department }
MonthlyAllocation { userId, month: 'YYYY-MM', pointsRemaining, pointsReceived }
PointAssignment { id, fromUserId, toUserId, points, category, message?, timestamp, month }
```

### Business rules

- Each user gets **10 points per month** to assign to peers
- 8 predefined recognition categories
- Role `people` sees admin/HR dashboards; role `employee` sees the standard dashboard
- Authentication is email-based (corporate domain validation), no tokens — session persisted to localStorage
- Demo users use Spanish names/departments; UI text is in Spanish

### UI stack

- React 18 + TypeScript, built with Vite (SWC compiler)
- Tailwind CSS v4 + shadcn/ui components (in `UI/src/components/ui/`)
- Recharts for data visualization, Motion (Framer) for animations, Sonner for toasts
- `react-hook-form` for form handling

### Conventions

- `UI/src/components/ui/` contains shadcn/ui primitives — do not edit these manually; use the shadcn CLI or copy updated versions
- Tailwind class merging uses `cn()` helper (`clsx` + `tailwind-merge`)
- No global state manager; state flows via React hooks and props; persistence goes through `storage.ts`
