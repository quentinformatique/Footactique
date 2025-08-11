import React from 'react';
import { Monitor, Smartphone } from 'lucide-react';

const MobileBlock: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
      <div className="max-w-md text-center">
        <div className="bg-white rounded-lg shadow-lg p-8">
          <div className="flex items-center justify-center mb-6">
            <Monitor className="w-12 h-12 text-blue-600 mr-4" />
            <Smartphone className="w-8 h-8 text-gray-400" />
          </div>
          
          <h1 className="text-2xl font-bold text-gray-900 mb-4">
            Application Desktop uniquement
          </h1>
          
          <p className="text-gray-600 mb-6">
            Footactique est optimisé pour les écrans d'ordinateur. 
            Veuillez utiliser un ordinateur ou une tablette pour accéder à l'application.
          </p>
          
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <p className="text-sm text-blue-800">
              <strong>Pourquoi ?</strong> L'éditeur de schémas tactiques nécessite 
              un écran plus large pour une expérience optimale.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default MobileBlock; 