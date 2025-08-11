import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';
import { apiService } from '../services/api';
import { User } from '../types/api';

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
  updateUser: (token: string) => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

interface JwtPayload {
  sub: string; // user ID
  email: string;
  username: string; // Custom claim for username
  exp: number;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  const decodeToken = (token: string): User | null => {
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return {
        id: decoded.sub,
        username: decoded.username,
        email: decoded.email
      };
    } catch (error) {
      console.error('Erreur lors du décodage du token:', error);
      return null;
    }
  };

  const isTokenExpired = (token: string): boolean => {
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return decoded.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  };

  useEffect(() => {
    // Vérifier si l'utilisateur est connecté au chargement
    const token = localStorage.getItem('token');
    if (token && !isTokenExpired(token)) {
      const userData = decodeToken(token);
      if (userData) {
        setUser(userData);
      } else {
        // Token invalide, le supprimer
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
      }
    } else if (token && isTokenExpired(token)) {
      // Token expiré, essayer de le rafraîchir
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        apiService.refreshToken({ refreshToken })
          .then(response => {
            localStorage.setItem('token', response.token);
            localStorage.setItem('refreshToken', response.refreshToken);
            const userData = decodeToken(response.token);
            if (userData) {
              setUser(userData);
            }
          })
          .catch(() => {
            // Échec du refresh, supprimer les tokens
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
          })
          .finally(() => setLoading(false));
        return;
      } else {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
      }
    }
    setLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    try {
      console.log('AuthContext: Tentative de connexion...');
      const response = await apiService.login({ email, password });
      console.log('AuthContext: Réponse reçue du serveur');
      
      localStorage.setItem('token', response.token);
      localStorage.setItem('refreshToken', response.refreshToken);
      
      const userData = decodeToken(response.token);
      if (userData) {
        console.log('AuthContext: Utilisateur décodé:', userData);
        setUser(userData);
      } else {
        throw new Error('Token invalide reçu du serveur');
      }
    } catch (error) {
      console.error('AuthContext: Erreur lors de la connexion:', error);
      throw error;
    }
  };

  const register = async (username: string, email: string, password: string) => {
    try {
      await apiService.register({ username, email, password });
    } catch (error) {
      throw error;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    setUser(null);
  };

  const updateUser = (token: string) => {
    const userData = decodeToken(token);
    if (userData) {
      setUser(userData);
    } else {
      logout(); // Si le token est invalide, on déconnecte l'utilisateur
    }
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    login,
    register,
    logout,
    updateUser,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}; 