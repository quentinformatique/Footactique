import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Download, Edit } from 'lucide-react';
import { apiService } from '../services/api';
import { TeamComposition } from '../types/api';
import Navigation from './Navigation';
import FootballField from './FootballField';
import { PdfExportService } from '../services/pdfExport';

const CompositionViewer: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  const [composition, setComposition] = useState<TeamComposition>({
    name: '',
    formation: '',
    players: []
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      const fetchComposition = async () => {
        try {
          setLoading(true);
          const data = await apiService.getTeamComposition(parseInt(id));
          setComposition(data);
        } catch (error) {
          console.error('Erreur lors du chargement de la composition:', error);
          navigate('/dashboard');
        } finally {
          setLoading(false);
        }
      };
      fetchComposition();
    }
  }, [id, navigate]);

  const handleExport = async () => {
    try {
      await PdfExportService.exportCompositionAsPdf(composition);
    } catch (error) {
      console.error('Erreur lors de l\'export PDF:', error);
      alert('Erreur lors de l\'export PDF');
    }
  };

  const handleEdit = () => {
    navigate(`/compositions/${id}/edit`);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        <Navigation />
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Navigation />
      
      <main className="max-w-7xl mx-auto px-6 py-8">
        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center space-x-4">
            <button
              onClick={() => navigate('/dashboard')}
              className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
            >
              <ArrowLeft className="w-5 h-5" />
            </button>
            <div>
              <h1 className="text-3xl font-bold text-gray-900">
                {composition.name}
              </h1>
              <p className="text-gray-600">
                Vue du schéma tactique
              </p>
            </div>
          </div>
          
          <div className="flex items-center space-x-4">
            <button
              onClick={handleExport}
              className="flex items-center px-4 py-2 border border-gray-300 text-gray-700 bg-white rounded-lg hover:bg-gray-50 transition-colors"
            >
              <Download className="w-4 h-4 mr-2" />
              Exporter
            </button>
            <button
              onClick={handleEdit}
              className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <Edit className="w-4 h-4 mr-2" />
              Éditer
            </button>
          </div>
        </div>

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Left Panel - Information */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-6">
                Informations du schéma
              </h2>
              
              <div className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Nom du schéma
                  </label>
                  <p className="text-gray-900 font-medium">
                    {composition.name}
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Formation
                  </label>
                  <p className="text-gray-900 font-medium">
                    {composition.formation}
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Nombre de joueurs
                  </label>
                  <p className="text-gray-900 font-medium">
                    {composition.players.length} joueur{composition.players.length > 1 ? 's' : ''}
                  </p>
                </div>

                {composition.description && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Description
                    </label>
                    <p className="text-gray-900">
                      {composition.description}
                    </p>
                  </div>
                )}
              </div>
            </div>

            {/* Players List */}
            <div className="bg-white rounded-lg shadow-sm p-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-6">
                Joueurs ({composition.players.length})
              </h2>
              
              {composition.players.length === 0 ? (
                <p className="text-gray-500 text-center py-4">
                  Aucun joueur dans ce schéma
                </p>
              ) : (
                <div className="space-y-3">
                  {composition.players.map((player) => (
                    <div
                      key={player.id}
                      className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                    >
                      <div className="flex items-center space-x-3">
                        <div 
                          className="w-8 h-8 rounded-full border-2 border-white"
                          style={{ backgroundColor: player.color || '#3B82F6' }}
                        >
                          <div className="flex items-center justify-center h-full text-white text-xs font-bold">
                            {player.number || player.playerName.charAt(0).toUpperCase()}
                          </div>
                        </div>
                        <div>
                          <p className="font-medium text-gray-900">
                            {player.playerName}
                          </p>
                          {player.number && (
                            <p className="text-sm text-gray-600">
                              #{player.number}
                            </p>
                          )}
                        </div>
                      </div>
                      {player.number && (
                        <span className="text-sm text-gray-500 font-medium">
                          #{player.number}
                        </span>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Right Panel - Football Field */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow-sm p-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-6">
                Schéma tactique
              </h2>
              
              <div className="h-[600px]">
                <FootballField
                  players={composition.players}
                  onPlayerMove={() => {}} // Pas de modification en mode lecture
                  onPlayerSelect={() => {}} // Pas de sélection en mode lecture
                  selectedPlayer={null}
                />
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default CompositionViewer; 