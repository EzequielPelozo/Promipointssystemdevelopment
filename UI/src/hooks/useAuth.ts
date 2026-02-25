import { useState, useEffect } from 'react';
import { User } from '@/infrastructure/interfaces';
import { getToken, getMe, logout } from '@/actions/auth/auth.actions';

export function useAuth() {
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const token = getToken();
    if (!token) {
      setIsLoading(false);
      return;
    }
    getMe()
      .then(setCurrentUser)
      .catch(() => logout())
      .finally(() => setIsLoading(false));
  }, []);

  const handleLogin = (user: User) => setCurrentUser(user);
  const handleLogout = () => { logout(); setCurrentUser(null); };

  return { currentUser, isLoading, handleLogin, handleLogout };
}
