import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { useIsMobile } from './hooks/useIsMobile';
import MobileBlock from './components/MobileBlock';
import LandingPage from './components/LandingPage';
import LoginPage from './components/LoginPage';
import RegisterPage from './components/RegisterPage';
import DashboardPage from './components/DashboardPage';
import CompositionEditor from './components/CompositionEditor';
import CompositionViewer from './components/CompositionViewer';
import UserProfilePage from './components/UserProfilePage';
import ProfilePage from './components/ProfilePage';

// Composant pour protéger les routes authentifiées
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  console.log('ProtectedRoute: loading=', loading, 'isAuthenticated=', isAuthenticated);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!isAuthenticated) {
    console.log('ProtectedRoute: Redirection vers /login');
    return <Navigate to="/login" replace />;
  }

  console.log('ProtectedRoute: Accès autorisé');
  return <>{children}</>;
};

// Composant pour rediriger les utilisateurs connectés
const PublicRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  console.log('PublicRoute: loading=', loading, 'isAuthenticated=', isAuthenticated);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (isAuthenticated) {
    console.log('PublicRoute: Redirection vers /dashboard');
    return <Navigate to="/dashboard" replace />;
  }

  console.log('PublicRoute: Accès autorisé');
  return <>{children}</>;
};

const AppRoutes: React.FC = () => {
  const isMobile = useIsMobile();

  // Bloquer l'accès sur mobile
  if (isMobile) {
    return <MobileBlock />;
  }

  return (
    <Routes>
      {/* Routes publiques */}
      <Route path="/" element={<LandingPage />} />
      
      <Route 
        path="/login" 
        element={
          <PublicRoute>
            <LoginPage />
          </PublicRoute>
        } 
      />
      
      <Route 
        path="/register" 
        element={
          <PublicRoute>
            <RegisterPage />
          </PublicRoute>
        } 
      />

      {/* Routes protégées */}
      <Route 
        path="/dashboard" 
        element={
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        } 
      />
      
      <Route 
        path="/compositions/new" 
        element={
          <ProtectedRoute>
            <CompositionEditor />
          </ProtectedRoute>
        } 
      />
      
      <Route 
        path="/compositions/:id" 
        element={
          <ProtectedRoute>
            <CompositionViewer />
          </ProtectedRoute>
        } 
      />
      
      <Route 
        path="/compositions/:id/edit" 
        element={
          <ProtectedRoute>
            <CompositionEditor />
          </ProtectedRoute>
        } 
      />
      
      <Route 
        path="/profile" 
        element={
          <ProtectedRoute>
            <UserProfilePage />
          </ProtectedRoute>
        } 
      />

      {/* Redirection par défaut */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

const App: React.FC = () => {
  return (
    <Router>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </Router>
  );
};

export default App;
