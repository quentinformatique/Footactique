import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, Download } from 'lucide-react';
import { apiService } from '../services/api';
import { TeamComposition, PlayerPosition } from '../types/api';
import Navigation from './Navigation';
import FootballField from './FootballField';
import PlayerPanel from './PlayerPanel';
import { PdfExportService } from '../services/pdfExport';

const CompositionEditor: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditing = !!id;

  const [composition, setComposition] = useState<TeamComposition>({
    name: '',
    formation: '',
    description: '',
    players: []
  });
  const [selectedPlayer, setSelectedPlayer] = useState<PlayerPosition | null>(null);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [compositionExists, setCompositionExists] = useState(false);

  useEffect(() => {
    if (isEditing && id) {
      const fetchComposition = async () => {
        try {
          setLoading(true);
          const data = await apiService.getTeamComposition(parseInt(id));
          setComposition(data);
          setCompositionExists(true);
        } catch (error) {
          
          // Si la composition n'existe pas, on reste en mode création
          setCompositionExists(false);
        } finally {
          setLoading(false);
        }
      };
      fetchComposition();
    }
  }, [id, isEditing, navigate]);

  const handleSave = async () => {
    if (!composition.name.trim() || !composition.formation.trim()) {
      alert('Veuillez remplir tous les champs obligatoires');
      return;
    }

    setSaving(true);
    try {
      if (isEditing && id && compositionExists) {
        await apiService.updateTeamComposition(parseInt(id), composition);
      } else {
        await apiService.createTeamComposition(composition);
      }
      navigate('/dashboard');
    } catch (error) {
      
      alert('Erreur lors de la sauvegarde');
    } finally {
      setSaving(false);
    }
  };

  const handleExport = async () => {
    try {
      await PdfExportService.exportCompositionAsPdf(composition);
    } catch (error) {
      
      alert('Erreur lors de l\'export PDF');
    }
  };

  const handlePlayerMove = (playerId: number, x: number, y: number) => {
    setComposition(prev => ({
      ...prev,
      players: prev.players.map(player => 
        player.id === playerId ? { ...player, x, y } : player
      )
    }));
  };

  const handlePlayerSelect = (player: PlayerPosition) => {
    setSelectedPlayer(player);
  };

  const handlePlayerAdd = (player: PlayerPosition) => {
    const newPlayer = {
      ...player,
      id: Date.now() // ID temporaire pour le frontend
    };
    setComposition(prev => ({
      ...prev,
      players: [...prev.players, newPlayer]
    }));
  };

  const handlePlayerUpdate = (updatedPlayer: PlayerPosition) => {
    setComposition(prev => ({
      ...prev,
      players: prev.players.map(player => 
        player.id === updatedPlayer.id ? updatedPlayer : player
      )
    }));
    setSelectedPlayer(updatedPlayer);
  };

  const handlePlayerDelete = (playerId: number) => {
    setComposition(prev => ({
      ...prev,
      players: prev.players.filter(player => player.id !== playerId)
    }));
    if (selectedPlayer?.id === playerId) {
      setSelectedPlayer(null);
    }
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
                {isEditing && compositionExists ? 'Éditer le schéma' : 'Créer un schéma'}
              </h1>
              <p className="text-gray-600">
                {isEditing && compositionExists ? 'Modifiez votre schéma tactique' : 'Créez votre nouveau schéma tactique'}
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
              onClick={handleSave}
              disabled={saving}
              className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
            >
              <Save className="w-4 h-4 mr-2" />
              {saving ? 'Sauvegarde...' : 'Sauvegarder'}
            </button>
          </div>
        </div>

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Left Panel - Information */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-6">
                Informations
              </h2>
              
              <div className="space-y-6">
                <div>
                  <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-2">
                    Nom du schéma
                  </label>
                  <input
                    type="text"
                    id="name"
                    value={composition.name}
                    onChange={(e) => setComposition({ ...composition, name: e.target.value })}
                    placeholder="Ex: Formation 4-4-2 offensive"
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>

                <div>
                  <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-2">
                    Description
                  </label>
                  <textarea
                    id="description"
                    rows={4}
                    value={composition.description || ''}
                    onChange={(e) => setComposition({ ...composition, description: e.target.value })}
                    placeholder="Description du schéma tactique..."
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>

                <div>
                  <label htmlFor="formation" className="block text-sm font-medium text-gray-700 mb-2">
                    Formation
                  </label>
                  <select
                    id="formation"
                    value={composition.formation}
                    onChange={(e) => setComposition({ ...composition, formation: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  >
                    <option value="">Sélectionner une formation</option>
                    <option value="4-4-2">4-4-2</option>
                    <option value="4-3-3">4-3-3</option>
                    <option value="3-5-2">3-5-2</option>
                    <option value="4-2-3-1">4-2-3-1</option>
                    <option value="3-4-3">3-4-3</option>
                    <option value="5-3-2">5-3-2</option>
                  </select>
                </div>
              </div>
            </div>

            {/* Player Panel */}
            <PlayerPanel
              players={composition.players}
              selectedPlayer={selectedPlayer}
              onPlayerSelect={handlePlayerSelect}
              onPlayerAdd={handlePlayerAdd}
              onPlayerUpdate={handlePlayerUpdate}
              onPlayerDelete={handlePlayerDelete}
            />
          </div>

          {/* Right Panel - Football Field */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow-sm p-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-6">
                Éditeur de terrain
              </h2>
              
              <div className="h-[600px]">
                <FootballField
                  players={composition.players}
                  onPlayerMove={handlePlayerMove}
                  onPlayerSelect={handlePlayerSelect}
                  selectedPlayer={selectedPlayer}
                />
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default CompositionEditor; 