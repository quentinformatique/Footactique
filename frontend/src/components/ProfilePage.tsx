import React, { useState } from 'react';
import { User, Mail, Calendar } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { apiService } from '../services/api';
import Navigation from './Navigation';

const ProfilePage: React.FC = () => {
  const { user, updateUser } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    username: user?.username || '',
    email: user?.email || ''
  });

  const handleSave = async () => {
    if (!user) return;
    
    setLoading(true);
    try {
      // Appel API pour mettre à jour le profil
      const response = await apiService.updateUserProfile({
        username: formData.username,
        email: formData.email,
        currentPassword: '', // TODO: Ajouter un champ pour le mot de passe actuel
        newPassword: '' // TODO: Ajouter un champ pour le nouveau mot de passe
      });
      
      // Mettre à jour le token dans le localStorage
      localStorage.setItem('token', response.token);
      
      // Mettre à jour le contexte d'authentification avec le nouveau token
      updateUser(response.token);
      
      setIsEditing(false);
    } catch (error) {
      console.error('Erreur lors de la mise à jour du profil:', error);
      alert('Erreur lors de la mise à jour du profil');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navigation />
      
      <main className="max-w-7xl mx-auto px-6 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            Mon profil
          </h1>
          <p className="text-gray-600">
            Gérez vos informations personnelles
          </p>
        </div>

        <div className="grid lg:grid-cols-2 gap-8">
          {/* Left Card - User Details */}
          <div className="bg-white rounded-lg shadow-sm p-6">
            <div className="text-center">
              {/* Profile Picture */}
              <div className="w-20 h-20 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <User className="w-10 h-10 text-blue-600" />
              </div>
              
              {/* Username */}
              <h2 className="text-xl font-semibold text-gray-900 mb-2">
                {user?.username}
              </h2>
              
              {/* Membership Date */}
              <p className="text-gray-600 mb-4">
                Membre depuis 13/07/2025
              </p>
              
              {/* Email */}
              <div className="flex items-center justify-center text-gray-600 mb-4">
                <Mail className="w-4 h-4 mr-2" />
                <span>{user?.email}</span>
              </div>
              
              {/* Last Activity */}
              <div className="flex items-center justify-center text-gray-600">
                <Calendar className="w-4 h-4 mr-2" />
                <span>Dernière activité: 13/07/2025</span>
              </div>
            </div>
          </div>

          {/* Right Card - Personal Information Form */}
          <div className="bg-white rounded-lg shadow-sm p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-6">
              Informations personnelles
            </h2>
            
            <div className="space-y-6">
              <div>
                <label htmlFor="username" className="block text-sm font-medium text-gray-700 mb-2">
                  Nom d'utilisateur
                </label>
                <input
                  type="text"
                  id="username"
                  value={formData.username}
                  onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                  disabled={!isEditing}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-50 disabled:text-gray-500"
                />
              </div>

              <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                  Email
                </label>
                <input
                  type="email"
                  id="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  disabled={!isEditing}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-50 disabled:text-gray-500"
                />
              </div>

              <div className="pt-4">
                {isEditing ? (
                  <div className="flex space-x-4">
                    <button
                      onClick={handleSave}
                      disabled={loading}
                      className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
                    >
                      {loading ? 'Sauvegarde...' : 'Sauvegarder'}
                    </button>
                    <button
                      onClick={() => {
                        setIsEditing(false);
                        setFormData({
                          username: user?.username || '',
                          email: user?.email || ''
                        });
                      }}
                      className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                    >
                      Annuler
                    </button>
                  </div>
                ) : (
                  <button
                    onClick={() => setIsEditing(true)}
                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                  >
                    Modifier
                  </button>
                )}
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default ProfilePage; 