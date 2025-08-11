import React, { useState } from 'react';
import { Plus, Trash2, Edit, Info } from 'lucide-react';
import { PlayerPosition } from '../types/api';

interface PlayerPanelProps {
  players: PlayerPosition[];
  selectedPlayer: PlayerPosition | null;
  onPlayerSelect: (player: PlayerPosition) => void;
  onPlayerAdd: (player: PlayerPosition) => void;
  onPlayerUpdate: (player: PlayerPosition) => void;
  onPlayerDelete: (playerId: number) => void;
}

const PlayerPanel: React.FC<PlayerPanelProps> = ({
  players,
  selectedPlayer,
  onPlayerSelect,
  onPlayerAdd,
  onPlayerUpdate,
  onPlayerDelete
}) => {
  const [showAddForm, setShowAddForm] = useState(false);
  const [showEditForm, setShowEditForm] = useState(false);
  const [newPlayer, setNewPlayer] = useState({
    playerName: '',
    position: '',
    number: '',
    color: '#3B82F6'
  });

  const handleAddPlayer = () => {
    if (newPlayer.playerName) {
      const player: PlayerPosition = {
        playerName: newPlayer.playerName,
        position: newPlayer.position || 'Joueur', // Position par défaut
        number: newPlayer.number ? parseInt(newPlayer.number) : undefined,
        color: newPlayer.color,
        x: 0.5, // Position centrale par défaut
        y: 0.5
      };
      onPlayerAdd(player);
      setNewPlayer({ playerName: '', position: '', number: '', color: '#3B82F6' });
      setShowAddForm(false);
    }
  };

  const handleUpdatePlayer = () => {
    if (selectedPlayer && newPlayer.playerName) {
      const updatedPlayer: PlayerPosition = {
        ...selectedPlayer,
        playerName: newPlayer.playerName,
        position: newPlayer.position || 'Joueur',
        number: newPlayer.number ? parseInt(newPlayer.number) : undefined,
        color: newPlayer.color
      };
      onPlayerUpdate(updatedPlayer);
      setNewPlayer({ playerName: '', position: '', number: '', color: '#3B82F6' });
      setShowEditForm(false);
    }
  };

  const handleEditClick = (player: PlayerPosition) => {
    setNewPlayer({
      playerName: player.playerName,
      position: player.position || '',
      number: player.number?.toString() || '',
      color: player.color || '#3B82F6'
    });
    setShowEditForm(true);
    setShowAddForm(false);
  };

  const colorOptions = [
    '#3B82F6', '#EF4444', '#10B981', '#F59E0B', '#8B5CF6',
    '#EC4899', '#06B6D4', '#84CC16', '#F97316', '#6366F1',
    '#000000', '#FFFFFF', '#6B7280', '#F3F4F6', '#1F2937'
  ];

  return (
    <div className="bg-white rounded-lg shadow-sm p-6">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold text-gray-900">
          Joueurs ({players.length})
        </h2>
                 <button
           onClick={() => {
             setShowAddForm(true);
             setShowEditForm(false);
             setNewPlayer({ playerName: '', position: '', number: '', color: '#3B82F6' });
           }}
           className="flex items-center px-3 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
         >
          <Plus className="w-4 h-4 mr-2" />
          Ajouter
        </button>
      </div>

      {/* Formulaire d'ajout */}
      {showAddForm && (
        <div className="mb-6 p-4 bg-gray-50 rounded-lg">
          <h3 className="font-semibold mb-3">Ajouter un joueur</h3>
          
          {/* Explication des champs */}
          <div className="mb-4 p-3 bg-blue-50 rounded-lg">
            <div className="flex items-start space-x-2">
              <Info className="w-4 h-4 text-blue-600 mt-0.5 flex-shrink-0" />
              <div className="text-sm text-blue-800">
                <p className="font-medium mb-1">À quoi servent ces champs ?</p>
                <ul className="space-y-1 text-xs">
                  <li><strong>Position :</strong> Rôle du joueur (ex: Attaquant, Milieu, Défenseur)</li>
                  <li><strong>Couleur :</strong> Permet d'identifier visuellement le joueur sur le terrain</li>
                  <li><strong>Numéro :</strong> Numéro de maillot (optionnel, pour l'identification)</li>
                </ul>
              </div>
            </div>
          </div>

          <div className="space-y-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Nom du joueur *
              </label>
              <input
                type="text"
                placeholder="Ex: Messi, Ronaldo..."
                value={newPlayer.playerName}
                onChange={(e) => setNewPlayer({ ...newPlayer, playerName: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Position (optionnel)
              </label>
              <input
                type="text"
                placeholder="Ex: Attaquant, Milieu, Défenseur..."
                value={newPlayer.position}
                onChange={(e) => setNewPlayer({ ...newPlayer, position: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Couleur du joueur
              </label>
              <div className="space-y-2">
                <div className="flex items-center space-x-3">
                  <input
                    type="color"
                    value={newPlayer.color}
                    onChange={(e) => setNewPlayer({ ...newPlayer, color: e.target.value })}
                    className="w-12 h-10 border border-gray-300 rounded-lg cursor-pointer"
                    title="Choisir une couleur personnalisée"
                  />
                  <span className="text-sm text-gray-600">Couleur personnalisée</span>
                </div>
                <div className="flex flex-wrap gap-2">
                  {colorOptions.map(color => (
                    <button
                      key={color}
                      type="button"
                      onClick={() => setNewPlayer({ ...newPlayer, color })}
                      className={`w-8 h-8 rounded-full border-2 transition-all ${
                        newPlayer.color === color ? 'border-gray-800 scale-110' : 'border-gray-300 hover:scale-105'
                      }`}
                      style={{ backgroundColor: color }}
                      title={`Couleur ${color}`}
                    />
                  ))}
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Numéro de maillot (optionnel)
              </label>
              <input
                type="number"
                placeholder="Ex: 10, 7, 9..."
                value={newPlayer.number}
                onChange={(e) => setNewPlayer({ ...newPlayer, number: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div className="flex space-x-2">
              <button
                onClick={handleAddPlayer}
                className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
              >
                Ajouter
              </button>
              <button
                onClick={() => setShowAddForm(false)}
                className="px-4 py-2 bg-gray-500 text-white rounded-lg hover:bg-gray-600"
              >
                Annuler
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Formulaire d'édition */}
      {showEditForm && selectedPlayer && (
        <div className="mb-6 p-4 bg-blue-50 rounded-lg">
          <h3 className="font-semibold mb-3">Modifier le joueur</h3>
          <div className="space-y-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Nom du joueur *
              </label>
              <input
                type="text"
                placeholder="Nom du joueur"
                value={newPlayer.playerName}
                onChange={(e) => setNewPlayer({ ...newPlayer, playerName: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Position (optionnel)
              </label>
              <input
                type="text"
                placeholder="Ex: Attaquant, Milieu, Défenseur..."
                value={newPlayer.position}
                onChange={(e) => setNewPlayer({ ...newPlayer, position: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Couleur du joueur
              </label>
              <div className="space-y-2">
                <div className="flex items-center space-x-3">
                  <input
                    type="color"
                    value={newPlayer.color}
                    onChange={(e) => setNewPlayer({ ...newPlayer, color: e.target.value })}
                    className="w-12 h-10 border border-gray-300 rounded-lg cursor-pointer"
                    title="Choisir une couleur personnalisée"
                  />
                  <span className="text-sm text-gray-600">Couleur personnalisée</span>
                </div>
                <div className="flex flex-wrap gap-2">
                  {colorOptions.map(color => (
                    <button
                      key={color}
                      type="button"
                      onClick={() => setNewPlayer({ ...newPlayer, color })}
                      className={`w-8 h-8 rounded-full border-2 transition-all ${
                        newPlayer.color === color ? 'border-gray-800 scale-110' : 'border-gray-300 hover:scale-105'
                      }`}
                      style={{ backgroundColor: color }}
                      title={`Couleur ${color}`}
                    />
                  ))}
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Numéro de maillot (optionnel)
              </label>
              <input
                type="number"
                placeholder="Numéro (optionnel)"
                value={newPlayer.number}
                onChange={(e) => setNewPlayer({ ...newPlayer, number: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div className="flex space-x-2">
              <button
                onClick={handleUpdatePlayer}
                className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
              >
                Modifier
              </button>
              <button
                onClick={() => setShowEditForm(false)}
                className="px-4 py-2 bg-gray-500 text-white rounded-lg hover:bg-gray-600"
              >
                Annuler
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Liste des joueurs */}
      <div className="space-y-2">
        {players.map((player) => (
          <div
            key={player.id}
            className={`p-3 rounded-lg border cursor-pointer transition-colors ${
              selectedPlayer?.id === player.id
                ? 'border-blue-500 bg-blue-50'
                : 'border-gray-200 hover:border-gray-300'
            }`}
            onClick={() => onPlayerSelect(player)}
          >
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div 
                  className="w-8 h-8 rounded-full flex items-center justify-center text-white font-bold text-sm border-2 border-white"
                  style={{ backgroundColor: player.color || '#3B82F6' }}
                >
                  {player.number || player.playerName.charAt(0).toUpperCase()}
                </div>
                <div>
                  <div className="font-semibold text-gray-900">{player.playerName}</div>
                  <div className="text-sm text-gray-600">
                    {player.position && <span>{player.position}</span>}
                    {player.position && player.number && <span> • </span>}
                    {player.number && <span>#{player.number}</span>}
                  </div>
                </div>
              </div>
              <div className="flex space-x-1">
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleEditClick(player);
                  }}
                  className="p-1 text-gray-500 hover:text-blue-600"
                >
                  <Edit className="w-4 h-4" />
                </button>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onPlayerDelete(player.id!);
                  }}
                  className="p-1 text-gray-500 hover:text-red-600"
                >
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>

      {players.length === 0 && (
        <div className="text-center py-8 text-gray-500">
          <p>Aucun joueur ajouté</p>
          <p className="text-sm">Cliquez sur "Ajouter" pour commencer</p>
        </div>
      )}
    </div>
  );
};

export default PlayerPanel; 