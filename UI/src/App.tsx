import { useAuth } from '@/hooks/useAuth';
import { LoginScreen } from '@/presentation/screens/auth/LoginScreen';
import { UserDashboard } from '@/presentation/screens/employee/UserDashboard';
import { PeopleDashboard } from '@/presentation/screens/hr/PeopleDashboard';
import { ToastProvider } from '@/presentation/providers/ToastProvider';

export default function App() {
  const { currentUser, isLoading, handleLogin, handleLogout } = useAuth();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="w-16 h-16 border-4 border-primary border-t-transparent rounded-full animate-spin mx-auto mb-4" />
          <p className="text-muted-foreground">Cargando...</p>
        </div>
      </div>
    );
  }

  if (!currentUser) {
    return (
      <>
        <LoginScreen onLogin={handleLogin} />
        <ToastProvider />
      </>
    );
  }

  return (
    <>
      {currentUser.role === 'people' ? (
        <PeopleDashboard user={currentUser} onLogout={handleLogout} />
      ) : (
        <UserDashboard user={currentUser} onLogout={handleLogout} />
      )}
      <ToastProvider />
    </>
  );
}
