import { toast } from 'sonner';

export const showSuccess = (msg: string, options?: Parameters<typeof toast.success>[1]) =>
  toast.success(msg, options);

export const showError = (msg: string, options?: Parameters<typeof toast.error>[1]) =>
  toast.error(msg, options);

export const showInfo = (msg: string, options?: Parameters<typeof toast.info>[1]) =>
  toast.info(msg, options);

export const showWarning = (msg: string, options?: Parameters<typeof toast.warning>[1]) =>
  toast.warning(msg, options);
