import React from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';

const LandingPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-white">
      {/* Header */}
      <header className="flex items-center justify-between px-6 py-4 border-b border-gray-200">
        <div className="flex items-center space-x-2">
          <div className="w-8 h-8 bg-blue-600 rounded flex items-center justify-center">
            <span className="text-white font-bold text-lg">F</span>
          </div>
          <span className="text-xl font-semibold text-gray-900">Footactique</span>
        </div>
        <div className="flex items-center space-x-4">
          <Link
            to="/login"
            className="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
          >
            Connexion
          </Link>
          <Link
            to="/register"
            className="px-4 py-2 text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
          >
            Inscription
          </Link>
        </div>
      </header>

      {/* Hero Section */}
      <section className="px-6 py-16 text-center">
        <h1 className="text-4xl md:text-6xl font-bold text-gray-900 mb-6">
          Créez vos schémas tactiques en{' '}
          <span className="text-blue-600">quelques clics</span>
        </h1>
        <p className="text-xl text-gray-600 mb-8 max-w-3xl mx-auto">
          Footactique vous permet de créer, partager et exporter vos schémas tactiques de football avec une interface intuitive et des outils professionnels.
        </p>
        <Link
          to="/register"
          className="inline-flex items-center px-8 py-4 bg-blue-600 text-white text-lg font-semibold rounded-lg hover:bg-blue-700 transition-colors"
        >
          Commencer gratuitement
          <ArrowRight className="ml-2 w-5 h-5" />
        </Link>
      </section>

      {/* Features Section */}
      <section className="px-6 py-16 bg-gray-50">
        <div className="max-w-6xl mx-auto">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-4">
              Tout ce dont vous avez besoin
            </h2>
            <p className="text-lg text-gray-600">
              Des outils puissants pour créer des schémas tactiques professionnels
            </p>
          </div>

          <div className="grid md:grid-cols-3 gap-8">
            {/* Feature 1 */}
            <div className="bg-white p-8 rounded-lg shadow-sm">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
                <div className="w-6 h-6 bg-blue-500 rounded-full relative">
                  <div className="absolute -top-1 -left-1 w-2 h-2 bg-blue-500 rounded-full"></div>
                  <div className="absolute -bottom-1 -right-1 w-2 h-2 bg-blue-500 rounded-full"></div>
                </div>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">
                Éditeur intuitif
              </h3>
              <p className="text-gray-600">
                Glissez-déposez vos joueurs sur le terrain et ajustez leurs positions avec une précision millimétrique.
              </p>
            </div>

            {/* Feature 2 */}
            <div className="bg-white p-8 rounded-lg shadow-sm">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
                <div className="w-6 h-6 bg-blue-500 rounded"></div>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">
                Templates prêts
              </h3>
              <p className="text-gray-600">
                Utilisez nos formations prédéfinies (4-4-2, 4-3-3, 3-5-2, etc.) ou créez vos propres templates.
              </p>
            </div>

            {/* Feature 3 */}
            <div className="bg-white p-8 rounded-lg shadow-sm">
              <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
                <div className="w-6 h-6 bg-blue-500 relative">
                  <div className="absolute inset-0 flex items-center justify-center">
                    <div className="w-3 h-3 bg-white rounded-sm"></div>
                  </div>
                </div>
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">
                Export multiple
              </h3>
              <p className="text-gray-600">
                Exportez vos schémas en PDF pour les partager avec votre équipe ou les imprimer.
              </p>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default LandingPage; 