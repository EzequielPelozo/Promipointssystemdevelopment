import { useState, useEffect } from 'react';
import { User, MonthlyAllocation, PointAssignment } from '../types';
import { getMyAllocation, getReceivedAssignments } from '../utils/api';
import { getCurrentMonth } from '../utils/storage';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from './ui/tooltip';
import { Award, Gift, TrendingUp, Calendar, LogOut, Info, Sparkles, Clock, User as UserIcon } from 'lucide-react';
import { AssignPoints } from './AssignPoints';
import { Onboarding } from './Onboarding';
import { DashboardSkeleton } from './SkeletonLoader';
import { MobileNav } from './MobileNav';
import { motion, AnimatePresence } from 'motion/react';

interface UserDashboardProps {
  user: User;
  onLogout: () => void;
}

export function UserDashboard({ user, onLogout }: UserDashboardProps) {
  const [allocation, setAllocation] = useState<MonthlyAllocation | null>(null);
  const [receivedPoints, setReceivedPoints] = useState<PointAssignment[]>([]);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [showOnboarding, setShowOnboarding] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const loadData = async () => {
    const month = getCurrentMonth();
    try {
      const [alloc, received] = await Promise.all([
        getMyAllocation(month),
        getReceivedAssignments(),
      ]);
      setAllocation(alloc);
      const currentMonthReceived = received.filter(a => a.month === month);
      setReceivedPoints(currentMonthReceived);
    } catch (err) {
      console.error('Error loading dashboard data', err);
    }
  };

  useEffect(() => {
    const hasSeenOnboarding = localStorage.getItem(`onboarding_${user.id}`);
    if (!hasSeenOnboarding) {
      setShowOnboarding(true);
    }

    setTimeout(() => {
      loadData().finally(() => setIsLoading(false));
    }, 600);
  }, [user.id]);

  const handleOnboardingComplete = () => {
    localStorage.setItem(`onboarding_${user.id}`, 'true');
    setShowOnboarding(false);
  };

  const handleAssignSuccess = () => {
    setShowAssignModal(false);
    loadData();
  };

  const totalReceived = receivedPoints.reduce((sum, a) => sum + a.points, 0);
  const pointsUsed = allocation ? 10 - allocation.pointsRemaining : 0;
  const usagePercentage = (pointsUsed / 10) * 100;

  const categoryBreakdown = receivedPoints.reduce((acc, assignment) => {
    acc[assignment.category] = (acc[assignment.category] || 0) + assignment.points;
    return acc;
  }, {} as Record<string, number>);

  const now = new Date();
  const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
  const daysRemaining = Math.ceil((lastDay.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));

  return (
    <TooltipProvider>
      <div className="min-h-screen bg-gradient-to-br from-primary/5 via-background to-secondary/5">
        {/* Mobile Navigation */}
        <MobileNav user={user} onLogout={onLogout} />

        {/* Desktop Header */}
        <motion.header
          className="bg-card border-b shadow-sm sticky top-0 z-40 hidden lg:block"
          initial={{ y: -100 }}
          animate={{ y: 0 }}
          transition={{ type: "spring", stiffness: 100 }}
        >
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="bg-gradient-to-br from-primary to-secondary rounded-xl p-2 shadow-md">
                  <Award className="w-6 h-6 text-white" />
                </div>
                <div>
                  <h1 className="text-xl">PromiPoints</h1>
                  <p className="text-sm text-muted-foreground">Grupo Prominente</p>
                </div>
              </div>
              <div className="flex items-center gap-4">
                <div className="text-right">
                  <div className="flex items-center gap-2">
                    <UserIcon className="w-4 h-4 text-muted-foreground" />
                    <p className="text-sm">{user.name}</p>
                  </div>
                  <p className="text-xs text-muted-foreground">{user.department}</p>
                </div>
                <Button variant="outline" size="sm" onClick={onLogout}>
                  <LogOut className="w-4 h-4 mr-2" />
                  Salir
                </Button>
              </div>
            </div>
          </div>
        </motion.header>

        <main className="max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8 space-y-4 sm:space-y-8 pb-20 sm:pb-8">
          {isLoading ? (
            <DashboardSkeleton />
          ) : (
            <>
              {/* Stats Overview */}
              <motion.div
                className="grid gap-3 sm:gap-4 grid-cols-1 sm:grid-cols-3"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.1 }}
              >
                <Card className="border-2 hover:shadow-lg transition-shadow duration-300">
                  <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                    <CardTitle className="text-sm flex items-center gap-2">
                      Disponibles
                      <Tooltip>
                        <TooltipTrigger>
                          <Info className="h-3 w-3 text-muted-foreground" />
                        </TooltipTrigger>
                        <TooltipContent>
                          <p className="max-w-xs">Puntos que puedes asignar este mes</p>
                        </TooltipContent>
                      </Tooltip>
                    </CardTitle>
                    <Gift className="h-5 w-5 text-primary" />
                  </CardHeader>
                  <CardContent>
                    <div className="flex items-baseline gap-2">
                      <motion.div
                        className="text-3xl sm:text-4xl text-primary"
                        initial={{ scale: 0 }}
                        animate={{ scale: 1 }}
                        transition={{ type: "spring", delay: 0.3 }}
                      >
                        {allocation?.pointsRemaining || 0}
                      </motion.div>
                      <span className="text-muted-foreground">/ 10</span>
                    </div>
                    <Progress value={usagePercentage} className="mt-3 h-2" />
                    <p className="text-xs text-muted-foreground mt-2 flex items-center gap-1">
                      <Clock className="w-3 h-3" />
                      {daysRemaining} {daysRemaining === 1 ? 'día' : 'días'} restantes
                    </p>
                  </CardContent>
                </Card>

                <Card className="border-2 hover:shadow-lg transition-shadow duration-300">
                  <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                    <CardTitle className="text-sm flex items-center gap-2">
                      Recibidos
                      <Tooltip>
                        <TooltipTrigger>
                          <Info className="h-3 w-3 text-muted-foreground" />
                        </TooltipTrigger>
                        <TooltipContent>
                          <p className="max-w-xs">Total recibido este mes</p>
                        </TooltipContent>
                      </Tooltip>
                    </CardTitle>
                    <Award className="h-5 w-5 text-secondary" />
                  </CardHeader>
                  <CardContent>
                    <motion.div
                      className="text-3xl sm:text-4xl text-secondary"
                      initial={{ scale: 0 }}
                      animate={{ scale: 1 }}
                      transition={{ type: "spring", delay: 0.4 }}
                    >
                      {totalReceived}
                    </motion.div>
                    <p className="text-xs text-muted-foreground mt-1">este mes</p>
                    {totalReceived > 0 && (
                      <div className="flex items-center gap-1 mt-2 text-xs text-success">
                        <TrendingUp className="w-3 h-3" />
                        ¡Excelente!
                      </div>
                    )}
                  </CardContent>
                </Card>

                <Card className="border-2 hover:shadow-lg transition-shadow duration-300 sm:col-span-1 col-span-1">
                  <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                    <CardTitle className="text-sm flex items-center gap-2">
                      Reconocimientos
                      <Tooltip>
                        <TooltipTrigger>
                          <Info className="h-3 w-3 text-muted-foreground" />
                        </TooltipTrigger>
                        <TooltipContent>
                          <p className="max-w-xs">Veces que te han reconocido</p>
                        </TooltipContent>
                      </Tooltip>
                    </CardTitle>
                    <Sparkles className="h-5 w-5 text-[#FFC107]" />
                  </CardHeader>
                  <CardContent>
                    <motion.div
                      className="text-3xl sm:text-4xl"
                      initial={{ scale: 0 }}
                      animate={{ scale: 1 }}
                      transition={{ type: "spring", delay: 0.5 }}
                    >
                      {receivedPoints.length}
                    </motion.div>
                    <p className="text-xs text-muted-foreground mt-1">este mes</p>
                  </CardContent>
                </Card>
              </motion.div>

              {/* Action Card */}
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.2 }}
              >
                <Card className={`border-2 transition-all duration-300 ${
                  (allocation?.pointsRemaining || 0) > 0
                    ? 'bg-gradient-to-br from-primary/5 to-secondary/5 hover:shadow-lg'
                    : 'bg-muted/30'
                }`}>
                  <CardHeader className="pb-3">
                    <CardTitle className="flex items-center gap-2 text-base sm:text-lg">
                      <Gift className="w-5 h-5" />
                      Reconoce a tus compañeros
                    </CardTitle>
                    <CardDescription className="text-sm">
                      {(allocation?.pointsRemaining || 0) > 0 ? (
                        <>
                          Tienes <span className="text-primary font-semibold">{allocation?.pointsRemaining || 0} PromiPoints</span> disponibles
                        </>
                      ) : (
                        <>
                          Has usado todos tus puntos. ¡Excelente participación!
                        </>
                      )}
                    </CardDescription>
                  </CardHeader>
                  <CardContent className="space-y-3">
                    {(allocation?.pointsRemaining || 0) === 0 ? (
                      <div className="flex items-center gap-2 text-sm text-muted-foreground bg-muted/50 p-3 rounded-lg">
                        <Calendar className="w-4 h-4 flex-shrink-0" />
                        <span>Tus puntos se renovarán el 1° del próximo mes</span>
                      </div>
                    ) : daysRemaining <= 3 && (
                      <div className="flex items-center gap-2 text-sm text-warning bg-warning/10 px-3 py-2 rounded-lg">
                        <Clock className="w-4 h-4 flex-shrink-0" />
                        <span>¡Solo quedan {daysRemaining} {daysRemaining === 1 ? 'día' : 'días'}! No olvides usar tus puntos</span>
                      </div>
                    )}
                    <Button
                      onClick={() => setShowAssignModal(true)}
                      disabled={(allocation?.pointsRemaining || 0) === 0}
                      className="w-full bg-secondary hover:bg-secondary/90 shadow-md hover:shadow-lg transition-all h-12 text-base"
                      size="lg"
                    >
                      <Gift className="w-5 h-5 mr-2" />
                      Asignar PromiPoints
                    </Button>
                  </CardContent>
                </Card>
              </motion.div>

              {/* Received Points */}
              <AnimatePresence>
                {receivedPoints.length > 0 && (
                  <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -20 }}
                    transition={{ delay: 0.3 }}
                  >
                    <Card className="border-2">
                      <CardHeader>
                        <CardTitle className="flex items-center gap-2 text-base sm:text-lg">
                          <Sparkles className="w-5 h-5 text-secondary" />
                          Tus Reconocimientos
                        </CardTitle>
                        <CardDescription className="text-sm">
                          Puntos recibidos por categoría este mes
                        </CardDescription>
                      </CardHeader>
                      <CardContent className="space-y-4">
                        <div className="grid gap-3 grid-cols-2 sm:grid-cols-2 lg:grid-cols-4">
                          {Object.entries(categoryBreakdown).map(([category, points], index) => (
                            <motion.div
                              key={category}
                              initial={{ opacity: 0, scale: 0.8 }}
                              animate={{ opacity: 1, scale: 1 }}
                              transition={{ delay: 0.4 + index * 0.1 }}
                              className="bg-gradient-to-br from-accent to-muted rounded-lg p-3 sm:p-4 border hover:shadow-md transition-all"
                            >
                              <p className="text-xs sm:text-sm text-muted-foreground mb-2 line-clamp-2">{category}</p>
                              <p className="text-2xl sm:text-3xl text-secondary">{points}</p>
                              <div className="mt-2 h-1 bg-secondary/20 rounded-full overflow-hidden">
                                <motion.div
                                  className="h-full bg-secondary rounded-full"
                                  initial={{ width: 0 }}
                                  animate={{ width: `${(points / Math.max(...Object.values(categoryBreakdown))) * 100}%` }}
                                  transition={{ delay: 0.6 + index * 0.1, duration: 0.5 }}
                                />
                              </div>
                            </motion.div>
                          ))}
                        </div>

                        <div className="border-t pt-4 space-y-3">
                          <h4 className="flex items-center gap-2 text-sm sm:text-base">
                            <Clock className="w-4 h-4 text-muted-foreground" />
                            Historial reciente
                          </h4>
                          <div className="space-y-3">
                            {receivedPoints.slice(0, 5).map((assignment, index) => (
                              <motion.div
                                key={assignment.id}
                                initial={{ opacity: 0, x: -20 }}
                                animate={{ opacity: 1, x: 0 }}
                                transition={{ delay: 0.5 + index * 0.1 }}
                                className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-3 p-3 sm:p-4 bg-muted/30 rounded-lg hover:bg-muted/50 transition-colors border"
                              >
                                <div className="flex-1 space-y-2">
                                  <div className="flex items-center gap-2 flex-wrap">
                                    <Badge variant="secondary" className="shadow-sm text-xs">
                                      {assignment.category}
                                    </Badge>
                                    <span className="text-xs text-muted-foreground flex items-center gap-1">
                                      <Calendar className="w-3 h-3" />
                                      {new Date(assignment.timestamp).toLocaleDateString('es-MX', {
                                        day: 'numeric',
                                        month: 'short',
                                      })}
                                    </span>
                                  </div>
                                  {assignment.message && (
                                    <p className="text-sm mt-2 text-foreground italic bg-background/50 p-2 rounded line-clamp-2">
                                      "{assignment.message}"
                                    </p>
                                  )}
                                </div>
                                <div className="text-left sm:text-right sm:ml-4">
                                  <motion.div
                                    className="text-3xl text-secondary inline-block"
                                    initial={{ scale: 0 }}
                                    animate={{ scale: 1 }}
                                    transition={{ delay: 0.6 + index * 0.1, type: "spring" }}
                                  >
                                    +{assignment.points}
                                  </motion.div>
                                </div>
                              </motion.div>
                            ))}
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  </motion.div>
                )}
              </AnimatePresence>

              {receivedPoints.length === 0 && (
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.3 }}
                >
                  <Card className="border-2 border-dashed">
                    <CardContent className="flex flex-col items-center justify-center py-12 sm:py-16">
                      <motion.div
                        animate={{
                          rotate: [0, 10, -10, 10, 0],
                          scale: [1, 1.1, 1]
                        }}
                        transition={{
                          duration: 2,
                          repeat: Infinity,
                          repeatDelay: 3
                        }}
                      >
                        <Award className="w-16 sm:w-20 h-16 sm:h-20 text-muted-foreground/30 mb-4" />
                      </motion.div>
                      <h3 className="text-lg sm:text-xl mb-2 text-center px-4">Aún no has recibido PromiPoints</h3>
                      <p className="text-sm sm:text-base text-muted-foreground text-center max-w-md px-4">
                        Cuando tus compañeros reconozcan tu trabajo excepcional, verás los puntos aquí
                      </p>
                    </CardContent>
                  </Card>
                </motion.div>
              )}
            </>
          )}
        </main>

        {!isLoading && (allocation?.pointsRemaining || 0) > 0 && (
          <motion.div
            className="fixed bottom-6 right-4 z-30 lg:hidden"
            initial={{ scale: 0, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            transition={{ delay: 0.5, type: "spring" }}
          >
            <Button
              onClick={() => setShowAssignModal(true)}
              size="lg"
              className="h-14 w-14 rounded-full shadow-2xl bg-secondary hover:bg-secondary/90 p-0"
            >
              <Gift className="w-6 h-6" />
            </Button>
          </motion.div>
        )}

        {showAssignModal && (
          <AssignPoints
            currentUser={user}
            onClose={() => setShowAssignModal(false)}
            onSuccess={handleAssignSuccess}
          />
        )}

        {showOnboarding && <Onboarding onComplete={handleOnboardingComplete} />}
      </div>
    </TooltipProvider>
  );
}
