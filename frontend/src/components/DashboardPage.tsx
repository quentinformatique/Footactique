import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Plus, BarChart3, Clock, Star, Edit, Eye, Calendar, Trash2, Heart, Download } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { apiService } from '../services/api';
import { TeamComposition } from '../types/api';
import Navigation from './Navigation';
import { PdfExportService } from '../services/pdfExport';

const DashboardPage: React.FC = () => {
  const { user } = useAuth();
  const [compositions, setCompositions] = useState<TeamComposition[]>([]);
  const [loading, setLoading] = useState(true);
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [togglingFavoriteId, setTogglingFavoriteId] = useState<number | null>(null);

  useEffect(() => {
    const fetchCompositions = async () => {
      try {
        const data = await apiService.getTeamCompositions();
        setCompositions(data);
      } catch (error) {
        console.error('Erreur lors du chargement des compositions:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchCompositions();
  }, []);

  const handleDelete = async (compositionId: number) => {
    if (!window.confirm('Êtes-vous sûr de vouloir supprimer ce schéma ? Cette action est irréversible.')) {
      return;
    }

    setDeletingId(compositionId);
    try {
      await apiService.deleteTeamComposition(compositionId);
      setCompositions(prev => prev.filter(comp => comp.id !== compositionId));
    } catch (error) {
      console.error('Erreur lors de la suppression:', error);
      alert('Erreur lors de la suppression du schéma');
    } finally {
      setDeletingId(null);
    }
  };

  const handleToggleFavorite = async (compositionId: number) => {
    setTogglingFavoriteId(compositionId);
    try {
      const composition = compositions.find(c => c.id === compositionId);
      if (composition) {
        const updatedComposition = { ...composition, isFavorite: !composition.isFavorite };
        await apiService.updateTeamComposition(compositionId, updatedComposition);
        setCompositions(prev => prev.map(comp => 
          comp.id === compositionId ? { ...comp, isFavorite: !comp.isFavorite } : comp
        ));
      }
    } catch (error) {
      console.error('Erreur lors de la mise à jour du favori:', error);
      alert('Erreur lors de la mise à jour du favori');
    } finally {
      setTogglingFavoriteId(null);
    }
  };

  const handleExport = async (composition: TeamComposition) => {
    try {
      await PdfExportService.exportCompositionAsPdf(composition);
    } catch (error) {
      console.error('Erreur lors de l\'export PDF:', error);
      alert('Erreur lors de l\'export PDF');
    }
  };

  const stats = [
    {
      title: 'Total Schémas',
      value: compositions.length,
      icon: BarChart3,
      color: 'bg-blue-100 text-blue-600'
    },
    {
      title: 'Récents',
      value: compositions.filter(c => {
        // Mock: considérer comme récent si créé dans les 7 derniers jours
        return true; // Pour l'instant, tous les schémas
      }).length,
      icon: Clock,
      color: 'bg-green-100 text-green-600'
    },
    {
      title: 'Favoris',
      value: compositions.filter(c => c.isFavorite).length,
      icon: Star,
      color: 'bg-orange-100 text-orange-600'
    }
  ];

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: 'numeric',
      month: 'short',
      year: 'numeric'
    });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navigation />
      
      <main className="max-w-7xl mx-auto px-6 py-8">
        {/* Greeting */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            Bonjour, {user?.username} !
          </h1>
          <p className="text-gray-600">
            Gérez vos schémas tactiques et créez de nouvelles stratégies
          </p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          {stats.map((stat, index) => (
            <div key={index} className="bg-white p-6 rounded-lg shadow-sm">
              <div className="flex items-center justify-between">
                <div className={`p-3 rounded-lg ${stat.color}`}>
                  <stat.icon className="w-6 h-6" />
                </div>
                <div className="text-right">
                  <p className="text-sm text-gray-600">{stat.title}</p>
                  <p className="text-2xl font-bold text-gray-900">{stat.value}</p>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Create New Schema Button */}
        <div className="mb-8">
          <Link
            to="/compositions/new"
            className="inline-flex items-center px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-colors"
          >
            <Plus className="w-5 h-5 mr-2" />
            Créer un nouveau schéma
          </Link>
        </div>

        {/* Recent Schemas */}
        <div className="bg-white rounded-lg shadow-sm p-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-6">
            Mes schémas tactiques
          </h2>
          
          {loading ? (
            <div className="text-center py-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
              <p className="text-gray-600 mt-2">Chargement...</p>
            </div>
          ) : compositions.length === 0 ? (
            <div className="text-center py-12">
              <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <BarChart3 className="w-8 h-8 text-gray-400" />
              </div>
              <p className="text-gray-600 mb-4">
                Vous n'avez pas encore créé de schémas
              </p>
              <Link
                to="/compositions/new"
                className="inline-flex items-center px-4 py-2 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
              >
                Créer votre premier schéma
              </Link>
            </div>
          ) : (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
              {compositions.map((composition) => (
                <div
                  key={composition.id}
                  className="border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow bg-white relative"
                >
                  {/* Badge favori */}
                  {composition.isFavorite && (
                    <div className="absolute top-4 right-4">
                      <div className="bg-yellow-100 text-yellow-800 px-2 py-1 rounded-full text-xs font-medium flex items-center">
                        <Star className="w-3 h-3 mr-1" />
                        Favori
                      </div>
                    </div>
                  )}

                  <div className="flex items-start justify-between mb-4">
                    <div className="flex-1">
                      <h3 className="font-semibold text-gray-900 text-lg mb-1">
                        {composition.name}
                      </h3>
                      <p className="text-sm text-gray-600 mb-2">
                        Formation: <span className="font-medium">{composition.formation}</span>
                      </p>
                      <div className="flex items-center text-sm text-gray-500 mb-3">
                        <Calendar className="w-4 h-4 mr-1" />
                        {composition.createdAt ? formatDate(composition.createdAt) : 'Date inconnue'}
                      </div>
                      <p className="text-sm text-gray-500">
                        {composition.players.length} joueur{composition.players.length > 1 ? 's' : ''}
                      </p>
                    </div>
                    <div className="flex items-center space-x-1">
                      <button
                        onClick={() => composition.id && handleToggleFavorite(composition.id)}
                        disabled={togglingFavoriteId === composition.id}
                        className={`p-2 rounded-lg transition-colors disabled:opacity-50 ${
                          composition.isFavorite 
                            ? 'text-yellow-600 hover:bg-yellow-50' 
                            : 'text-gray-400 hover:text-yellow-600 hover:bg-yellow-50'
                        }`}
                        title={composition.isFavorite ? 'Retirer des favoris' : 'Ajouter aux favoris'}
                      >
                        {togglingFavoriteId === composition.id ? (
                          <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-yellow-600"></div>
                        ) : (
                          <Heart className={`w-4 h-4 ${composition.isFavorite ? 'fill-current' : ''}`} />
                        )}
                      </button>
                      <button
                        onClick={() => composition.id && handleDelete(composition.id)}
                        disabled={deletingId === composition.id}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors disabled:opacity-50"
                        title="Supprimer ce schéma"
                      >
                        {deletingId === composition.id ? (
                          <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-red-600"></div>
                        ) : (
                          <Trash2 className="w-4 h-4" />
                        )}
                      </button>
                    </div>
                  </div>
                  
                  <div className="flex items-center space-x-2">
                    <Link
                      to={`/compositions/${composition.id}/edit`}
                      className="flex-1 flex items-center justify-center px-3 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
                    >
                      <Edit className="w-4 h-4 mr-1" />
                      Éditer
                    </Link>
                    <Link
                      to={`/compositions/${composition.id}`}
                      className="flex-1 flex items-center justify-center px-3 py-2 border border-gray-300 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-50 transition-colors"
                    >
                      <Eye className="w-4 h-4 mr-1" />
                      Voir
                    </Link>
                    <button
                      onClick={() => handleExport(composition)}
                      className="flex-1 flex items-center justify-center px-3 py-2 border border-gray-300 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-50 transition-colors"
                      title="Exporter en PDF"
                    >
                      <Download className="w-4 h-4 mr-1" />
                      PDF
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default DashboardPage; 