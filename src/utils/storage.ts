import { User, MonthlyAllocation, PointAssignment } from '../types';

const STORAGE_KEYS = {
  CURRENT_USER: 'promipoints_current_user',
  USERS: 'promipoints_users',
  ALLOCATIONS: 'promipoints_allocations',
  ASSIGNMENTS: 'promipoints_assignments',
};

export const getCurrentMonth = (): string => {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
};

export const storage = {
  // Current User
  setCurrentUser: (user: User | null) => {
    if (user) {
      localStorage.setItem(STORAGE_KEYS.CURRENT_USER, JSON.stringify(user));
    } else {
      localStorage.removeItem(STORAGE_KEYS.CURRENT_USER);
    }
  },

  getCurrentUser: (): User | null => {
    const data = localStorage.getItem(STORAGE_KEYS.CURRENT_USER);
    return data ? JSON.parse(data) : null;
  },

  // Users
  getUsers: (): User[] => {
    const data = localStorage.getItem(STORAGE_KEYS.USERS);
    return data ? JSON.parse(data) : [];
  },

  setUsers: (users: User[]) => {
    localStorage.setItem(STORAGE_KEYS.USERS, JSON.stringify(users));
  },

  // Monthly Allocations
  getAllocations: (): MonthlyAllocation[] => {
    const data = localStorage.getItem(STORAGE_KEYS.ALLOCATIONS);
    return data ? JSON.parse(data) : [];
  },

  setAllocations: (allocations: MonthlyAllocation[]) => {
    localStorage.setItem(STORAGE_KEYS.ALLOCATIONS, JSON.stringify(allocations));
  },

  getUserAllocation: (userId: string, month: string): MonthlyAllocation | null => {
    const allocations = storage.getAllocations();
    return allocations.find(a => a.userId === userId && a.month === month) || null;
  },

  updateAllocation: (allocation: MonthlyAllocation) => {
    const allocations = storage.getAllocations();
    const index = allocations.findIndex(
      a => a.userId === allocation.userId && a.month === allocation.month
    );
    
    if (index >= 0) {
      allocations[index] = allocation;
    } else {
      allocations.push(allocation);
    }
    
    storage.setAllocations(allocations);
  },

  // Point Assignments
  getAssignments: (): PointAssignment[] => {
    const data = localStorage.getItem(STORAGE_KEYS.ASSIGNMENTS);
    return data ? JSON.parse(data) : [];
  },

  setAssignments: (assignments: PointAssignment[]) => {
    localStorage.setItem(STORAGE_KEYS.ASSIGNMENTS, JSON.stringify(assignments));
  },

  addAssignment: (assignment: PointAssignment) => {
    const assignments = storage.getAssignments();
    assignments.push(assignment);
    storage.setAssignments(assignments);
  },

  getReceivedPoints: (userId: string): PointAssignment[] => {
    const assignments = storage.getAssignments();
    return assignments.filter(a => a.toUserId === userId);
  },

  // Initialize monthly allocation
  ensureMonthlyAllocation: (userId: string) => {
    const month = getCurrentMonth();
    let allocation = storage.getUserAllocation(userId, month);
    
    if (!allocation) {
      allocation = {
        userId,
        month,
        pointsRemaining: 10,
        pointsReceived: 0,
      };
      storage.updateAllocation(allocation);
    }
    
    return allocation;
  },
};

// Initialize demo users on first load
export const initializeDemoData = () => {
  const existingUsers = storage.getUsers();
  
  if (existingUsers.length === 0) {
    const demoUsers: User[] = [
      {
        id: '1',
        name: 'María García',
        email: 'maria.garcia@grupoprominente.com',
        role: 'employee',
        department: 'Desarrollo',
      },
      {
        id: '2',
        name: 'Juan Pérez',
        email: 'juan.perez@grupoprominente.com',
        role: 'employee',
        department: 'Marketing',
      },
      {
        id: '3',
        name: 'Ana Rodríguez',
        email: 'ana.rodriguez@grupoprominente.com',
        role: 'people',
        department: 'People & Culture',
      },
      {
        id: '4',
        name: 'Carlos López',
        email: 'carlos.lopez@grupoprominente.com',
        role: 'employee',
        department: 'Ventas',
      },
      {
        id: '5',
        name: 'Laura Martínez',
        email: 'laura.martinez@grupoprominente.com',
        role: 'employee',
        department: 'Desarrollo',
      },
      {
        id: '6',
        name: 'Diego Sánchez',
        email: 'diego.sanchez@grupoprominente.com',
        role: 'employee',
        department: 'Operaciones',
      },
      {
        id: '7',
        name: 'Sofia Torres',
        email: 'sofia.torres@grupoprominente.com',
        role: 'employee',
        department: 'Marketing',
      },
      {
        id: '8',
        name: 'Roberto Flores',
        email: 'roberto.flores@grupoprominente.com',
        role: 'employee',
        department: 'Ventas',
      },
    ];
    
    storage.setUsers(demoUsers);
    
    // Initialize allocations for demo users
    const month = getCurrentMonth();
    demoUsers.forEach(user => {
      storage.ensureMonthlyAllocation(user.id);
    });

    // Add some demo assignments
    const demoAssignments: PointAssignment[] = [
      {
        id: '1',
        fromUserId: '2',
        toUserId: '1',
        points: 3,
        category: 'Trabajo en equipo',
        message: '¡Excelente colaboración en el proyecto!',
        timestamp: Date.now() - 86400000 * 2,
        month,
      },
      {
        id: '2',
        fromUserId: '4',
        toUserId: '1',
        points: 2,
        category: 'Innovación',
        message: 'Gran idea para mejorar el proceso',
        timestamp: Date.now() - 86400000 * 5,
        month,
      },
      {
        id: '3',
        fromUserId: '5',
        toUserId: '2',
        points: 4,
        category: 'Liderazgo',
        timestamp: Date.now() - 86400000 * 3,
        month,
      },
      {
        id: '4',
        fromUserId: '1',
        toUserId: '4',
        points: 2,
        category: 'Colaboración',
        message: 'Siempre dispuesto a ayudar',
        timestamp: Date.now() - 86400000,
        month,
      },
    ];

    storage.setAssignments(demoAssignments);

    // Update allocations based on assignments
    demoAssignments.forEach(assignment => {
      const fromAllocation = storage.getUserAllocation(assignment.fromUserId, month);
      if (fromAllocation) {
        fromAllocation.pointsRemaining -= assignment.points;
        storage.updateAllocation(fromAllocation);
      }

      const toAllocation = storage.getUserAllocation(assignment.toUserId, month);
      if (toAllocation) {
        toAllocation.pointsReceived += assignment.points;
        storage.updateAllocation(toAllocation);
      }
    });
  }
};
