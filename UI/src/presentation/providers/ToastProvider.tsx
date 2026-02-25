import { Toaster } from '@/components/ui/sonner';

export function ToastProvider() {
  return (
    <Toaster
      position="top-center"
      expand={false}
      richColors
      toastOptions={{
        style: { padding: '16px' },
        className: 'text-sm sm:text-base',
      }}
    />
  );
}
