import { Award, User as UserIcon, LogOut, Menu, X } from 'lucide-react';
import { User } from '../types';
import { Button } from './ui/button';
import { useState } from 'react';
import { motion, AnimatePresence } from 'motion/react';

interface MobileNavProps {
  user: User;
  onLogout: () => void;
}

export function MobileNav({ user, onLogout }: MobileNavProps) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <header className="bg-card border-b shadow-sm sticky top-0 z-50 lg:hidden">
        <div className="px-4 py-3">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <div className="bg-gradient-to-br from-primary to-secondary rounded-lg p-2 shadow-md">
                <Award className="w-5 h-5 text-white" />
              </div>
              <div>
                <h1 className="text-lg">PromiPoints</h1>
                <p className="text-xs text-muted-foreground">Grupo Prominente</p>
              </div>
            </div>
            <Button 
              variant="ghost" 
              size="sm"
              onClick={() => setIsOpen(!isOpen)}
              className="p-2"
            >
              {isOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
            </Button>
          </div>
        </div>
      </header>

      <AnimatePresence>
        {isOpen && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              className="fixed inset-0 bg-black/50 z-40 lg:hidden"
              onClick={() => setIsOpen(false)}
            />
            <motion.div
              initial={{ x: '100%' }}
              animate={{ x: 0 }}
              exit={{ x: '100%' }}
              transition={{ type: 'spring', damping: 25 }}
              className="fixed top-0 right-0 bottom-0 w-[280px] bg-card border-l shadow-2xl z-50 lg:hidden"
            >
              <div className="p-4 border-b">
                <div className="flex items-center justify-between mb-4">
                  <h2 className="font-semibold">Menú</h2>
                  <Button 
                    variant="ghost" 
                    size="sm"
                    onClick={() => setIsOpen(false)}
                    className="p-2"
                  >
                    <X className="w-5 h-5" />
                  </Button>
                </div>
                
                <div className="flex items-center gap-3 p-3 bg-gradient-to-r from-primary/10 to-secondary/10 rounded-lg">
                  <div className="bg-primary/20 rounded-full p-2">
                    <UserIcon className="w-5 h-5 text-primary" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="font-medium truncate">{user.name}</p>
                    <p className="text-xs text-muted-foreground truncate">{user.department}</p>
                  </div>
                </div>
              </div>

              <div className="p-4 space-y-2">
                <Button 
                  variant="outline" 
                  className="w-full justify-start h-12"
                  onClick={() => {
                    onLogout();
                    setIsOpen(false);
                  }}
                >
                  <LogOut className="w-4 h-4 mr-3" />
                  Cerrar Sesión
                </Button>
              </div>

              <div className="absolute bottom-0 left-0 right-0 p-4 border-t bg-muted/30">
                <p className="text-xs text-center text-muted-foreground">
                  PromiPoints v1.0
                </p>
                <p className="text-xs text-center text-muted-foreground mt-1">
                  Grupo Prominente © 2025
                </p>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </>
  );
}
