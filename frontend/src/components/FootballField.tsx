import React, { useState, useRef } from 'react';
import Draggable from 'react-draggable';
import { PlayerPosition } from '../types/api';

interface FootballFieldProps {
  players: PlayerPosition[];
  onPlayerMove: (playerId: number, x: number, y: number) => void;
  onPlayerSelect: (player: PlayerPosition) => void;
  selectedPlayer: PlayerPosition | null;
}

interface PlayerComponentProps {
  player: PlayerPosition;
  position: { x: number; y: number };
  isSelected: boolean;
  onPlayerMove: (playerId: number, x: number, y: number) => void;
  onPlayerSelect: (player: PlayerPosition) => void;
}

// Composant séparé pour chaque joueur
const PlayerComponent: React.FC<PlayerComponentProps> = ({
  player,
  position,
  isSelected,
  onPlayerMove,
  onPlayerSelect
}) => {
  const playerRef = useRef<HTMLDivElement | null>(null);

  const handlePlayerDrag = (playerId: number, x: number, y: number) => {
    // Convertir les coordonnées pixels en coordonnées normalisées
    const fieldElement = playerRef.current?.parentElement;
    if (fieldElement) {
      const rect = fieldElement.getBoundingClientRect();
      const normalizedX = x / rect.width;
      const normalizedY = 1 - (y / rect.height); // Inverser Y
      onPlayerMove(playerId, normalizedX, normalizedY);
    }
  };

  return (
    <Draggable
      nodeRef={playerRef}
      position={{ x: position.x - 20, y: position.y - 20 }} // Centrer le joueur
      onStop={(e, data) => {
        handlePlayerDrag(player.id!, data.x + 20, data.y + 20);
      }}
      bounds="parent"
    >
      <div
        ref={playerRef}
        className={`absolute w-10 h-10 rounded-full border-2 cursor-move transition-all duration-200 ${
          isSelected 
            ? 'border-yellow-400 shadow-lg scale-110' 
            : 'border-white hover:scale-105'
        }`}
        style={{ backgroundColor: player.color || '#3B82F6' }}
        onClick={() => onPlayerSelect(player)}
      >
        <div className="flex items-center justify-center h-full text-white font-bold text-sm">
          {player.number || player.playerName.charAt(0).toUpperCase()}
        </div>
        
        {/* Nom du joueur */}
        <div className="absolute -bottom-6 left-1/2 transform -translate-x-1/2 bg-black bg-opacity-75 text-white text-xs px-2 py-1 rounded whitespace-nowrap">
          {player.playerName}
        </div>
      </div>
    </Draggable>
  );
};

const FootballField: React.FC<FootballFieldProps> = ({
  players,
  onPlayerMove,
  onPlayerSelect,
  selectedPlayer
}) => {
  const [fieldSize, setFieldSize] = useState({ width: 0, height: 0 });

  // Référence pour le terrain
  const fieldRef = useRef<HTMLDivElement>(null);

  React.useEffect(() => {
    if (fieldRef.current) {
      const rect = fieldRef.current.getBoundingClientRect();
      setFieldSize({ width: rect.width, height: rect.height });
    }
  }, []);

  // Convertir les coordonnées normalisées (0-1) en pixels
  const normalizedToPixels = (normalizedX: number, normalizedY: number) => {
    return {
      x: normalizedX * fieldSize.width,
      y: (1 - normalizedY) * fieldSize.height // Inverser Y car 0 = haut en CSS
    };
  };

  return (
    <div className="relative w-full h-full min-h-[500px] rounded-lg overflow-hidden border-4 border-white shadow-xl">
      {/* Référence pour le terrain */}
      <div ref={fieldRef} className="absolute inset-0" />
      
      {/* Fond du terrain avec texture d'herbe */}
      <div className="absolute inset-0 bg-gradient-to-br from-green-500 via-green-600 to-green-700">
        {/* Texture d'herbe */}
        <div className="absolute inset-0 opacity-20" 
             style={{
               backgroundImage: `
                 repeating-linear-gradient(
                   90deg,
                   transparent,
                   transparent 2px,
                   rgba(255,255,255,0.1) 2px,
                   rgba(255,255,255,0.1) 4px
                 ),
                 repeating-linear-gradient(
                   0deg,
                   transparent,
                   transparent 4px,
                   rgba(255,255,255,0.05) 4px,
                   rgba(255,255,255,0.05) 8px
                 )
               `
             }}
        />
      </div>
      
      {/* Lignes du terrain */}
      <div className="absolute inset-0">
        {/* Ligne de touche (bordure) */}
        <div className="absolute inset-0 border-2 border-white" />
        
        {/* Ligne médiane */}
        <div className="absolute top-0 left-1/2 w-0.5 h-full bg-white transform -translate-x-1/2" />
        
        {/* Cercle central */}
        <div className="absolute top-1/2 left-1/2 w-24 h-24 border-2 border-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        
        {/* Point central */}
        <div className="absolute top-1/2 left-1/2 w-2 h-2 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        
        {/* Surface de réparation gauche */}
        <div className="absolute top-1/2 left-0 w-1/6 h-1/2 border-2 border-white transform -translate-y-1/2" />
        
        {/* Surface de réparation droite */}
        <div className="absolute top-1/2 right-0 w-1/6 h-1/2 border-2 border-white transform -translate-y-1/2" />
        
        {/* Zone de but gauche */}
        <div className="absolute top-1/2 left-0 w-1/12 h-1/3 border-2 border-white transform -translate-y-1/2" />
        
        {/* Zone de but droite */}
        <div className="absolute top-1/2 right-0 w-1/12 h-1/3 border-2 border-white transform -translate-y-1/2" />
        
        {/* Points de penalty */}
        <div className="absolute top-1/3 left-1/12 w-2 h-2 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        <div className="absolute top-2/3 left-1/12 w-2 h-2 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        <div className="absolute top-1/3 right-1/12 w-2 h-2 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        <div className="absolute top-2/3 right-1/12 w-2 h-2 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        
        {/* Points de corner */}
        <div className="absolute top-0 left-0 w-3 h-3 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        <div className="absolute top-0 right-0 w-3 h-3 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        <div className="absolute bottom-0 left-0 w-3 h-3 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        <div className="absolute bottom-0 right-0 w-3 h-3 bg-white rounded-full transform -translate-x-1/2 -translate-y-1/2" />
        
        {/* Arc de penalty gauche */}
        <div className="absolute top-1/2 left-1/12 w-6 h-6 border-2 border-white border-r-0 border-b-0 rounded-full transform -translate-x-1/2 -translate-y-1/2" 
             style={{ borderTopLeftRadius: '50%', borderTopRightRadius: '0', borderBottomLeftRadius: '0', borderBottomRightRadius: '0' }} />
        
        {/* Arc de penalty droite */}
        <div className="absolute top-1/2 right-1/12 w-6 h-6 border-2 border-white border-l-0 border-b-0 rounded-full transform -translate-x-1/2 -translate-y-1/2" 
             style={{ borderTopLeftRadius: '0', borderTopRightRadius: '50%', borderBottomLeftRadius: '0', borderBottomRightRadius: '0' }} />
        
        {/* Buts */}
        <div className="absolute top-1/2 left-0 w-2 h-16 bg-white transform -translate-y-1/2 shadow-lg" />
        <div className="absolute top-1/2 right-0 w-2 h-16 bg-white transform -translate-y-1/2 shadow-lg" />
        
        {/* Filets des buts */}
        <div className="absolute top-1/2 left-0 w-2 h-16 bg-white bg-opacity-20 transform -translate-y-1/2" 
             style={{ backgroundImage: 'repeating-linear-gradient(90deg, transparent, transparent 1px, rgba(255,255,255,0.2) 1px, rgba(255,255,255,0.2) 2px), repeating-linear-gradient(0deg, transparent, transparent 2px, rgba(255,255,255,0.2) 2px, rgba(255,255,255,0.2) 4px)' }} />
        <div className="absolute top-1/2 right-0 w-2 h-16 bg-white bg-opacity-20 transform -translate-y-1/2" 
             style={{ backgroundImage: 'repeating-linear-gradient(90deg, transparent, transparent 1px, rgba(255,255,255,0.2) 1px, rgba(255,255,255,0.2) 2px), repeating-linear-gradient(0deg, transparent, transparent 2px, rgba(255,255,255,0.2) 2px, rgba(255,255,255,0.2) 4px)' }} />
        
        {/* Marques de penalty (petits arcs) */}
        <div className="absolute top-1/4 left-1/12 w-4 h-4 border-2 border-white border-r-0 border-b-0 rounded-full transform -translate-x-1/2 -translate-y-1/2" 
             style={{ borderTopLeftRadius: '50%', borderTopRightRadius: '0', borderBottomLeftRadius: '0', borderBottomRightRadius: '0' }} />
        <div className="absolute top-3/4 left-1/12 w-4 h-4 border-2 border-white border-r-0 border-t-0 rounded-full transform -translate-x-1/2 -translate-y-1/2" 
             style={{ borderTopLeftRadius: '0', borderTopRightRadius: '0', borderBottomLeftRadius: '50%', borderBottomRightRadius: '0' }} />
        <div className="absolute top-1/4 right-1/12 w-4 h-4 border-2 border-white border-l-0 border-b-0 rounded-full transform -translate-x-1/2 -translate-y-1/2" 
             style={{ borderTopLeftRadius: '0', borderTopRightRadius: '50%', borderBottomLeftRadius: '0', borderBottomRightRadius: '0' }} />
        <div className="absolute top-3/4 right-1/12 w-4 h-4 border-2 border-white border-l-0 border-t-0 rounded-full transform -translate-x-1/2 -translate-y-1/2" 
             style={{ borderTopLeftRadius: '0', borderTopRightRadius: '0', borderBottomLeftRadius: '0', borderBottomRightRadius: '50%' }} />
      </div>

      {/* Joueurs */}
      {players.map((player) => {
        const position = normalizedToPixels(player.x, player.y);
        const isSelected = selectedPlayer?.id === player.id;
        
        return (
          <PlayerComponent
            key={player.id}
            player={player}
            position={position}
            isSelected={isSelected}
            onPlayerMove={onPlayerMove}
            onPlayerSelect={onPlayerSelect}
          />
        );
      })}

      {/* Légende */}
      <div className="absolute bottom-4 left-4 bg-black bg-opacity-75 text-white text-xs px-3 py-2 rounded">
        <div className="font-semibold mb-1">Légende :</div>
        <div>• Cliquez sur un joueur pour le sélectionner</div>
        <div>• Glissez-déposez pour déplacer les joueurs</div>
      </div>
    </div>
  );
};

export default FootballField; 