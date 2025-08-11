# Footactique - Frontend

Application React avec TypeScript pour la création de schémas tactiques de football.

## 🚀 Installation

```bash
# Installer les dépendances
npm install

# Copier le fichier d'environnement
cp .env.example .env

# Démarrer le serveur de développement
npm start
```

L'application sera accessible à l'adresse [http://localhost:3000](http://localhost:3000).

## 🔧 Configuration

### Variables d'environnement

Créez un fichier `.env` à la racine du projet avec les variables suivantes :

```env
REACT_APP_API_URL=http://localhost:5000/api
```

**Note :** Assurez-vous que votre backend est démarré sur le port 5000 avant de tester l'application.

## 🛠️ Technologies utilisées

- **React 18** avec TypeScript
- **Tailwind CSS** pour le styling
- **React Router** pour la navigation
- **Axios** pour les appels API
- **Lucide React** pour les icônes
- **JWT Decode** pour la gestion des tokens

## 📁 Structure du projet

```
src/
├── components/          # Composants React
│   ├── LandingPage.tsx  # Page d'accueil
│   ├── LoginPage.tsx    # Page de connexion
│   ├── RegisterPage.tsx # Page d'inscription
│   ├── DashboardPage.tsx # Tableau de bord
│   ├── CompositionEditor.tsx # Éditeur de schémas
│   ├── ProfilePage.tsx  # Page de profil
│   └── Navigation.tsx   # Navigation principale
├── contexts/            # Contextes React
│   └── AuthContext.tsx  # Contexte d'authentification
├── services/            # Services
│   └── api.ts          # Service API
├── types/              # Types TypeScript
│   └── api.ts          # Types pour l'API
└── App.tsx             # Composant principal
```

## 🔐 Authentification

L'application utilise un système d'authentification JWT complet avec :

- **Connexion/Inscription** avec validation des erreurs
- **Protection des routes** automatique
- **Gestion automatique du refresh token**
- **Décodage JWT** pour récupérer les informations utilisateur
- **Redirection automatique** selon l'état de connexion
- **Gestion des erreurs réseau** avec messages informatifs

### Fonctionnalités d'authentification :

- ✅ Décodage automatique du JWT
- ✅ Vérification de l'expiration des tokens
- ✅ Refresh automatique des tokens expirés
- ✅ Gestion des erreurs de connexion
- ✅ Messages d'erreur personnalisés
- ✅ Protection contre les tokens invalides

## 🎨 Design

L'interface suit exactement les maquettes fournies avec :
- Design moderne et épuré
- Palette de couleurs cohérente (bleu, blanc, gris)
- Composants responsifs
- Animations et transitions fluides

## 📱 Pages disponibles

1. **Landing Page** (`/`) - Page d'accueil avec présentation
2. **Login** (`/login`) - Connexion utilisateur
3. **Register** (`/register`) - Inscription utilisateur
4. **Dashboard** (`/dashboard`) - Tableau de bord principal
5. **Composition Editor** (`/compositions/new`, `/compositions/:id/edit`) - Éditeur de schémas
6. **Profile** (`/profile`) - Gestion du profil utilisateur

## 🔧 Configuration API

L'application se connecte automatiquement à l'API backend configurée dans le fichier `.env`.

**URL par défaut :** `http://localhost:5000/api`

**Endpoints utilisés :**
- `POST /auth/register` - Inscription
- `POST /auth/login` - Connexion
- `POST /auth/refresh` - Rafraîchir le token
- `GET /teamcompositions` - Liste des schémas
- `GET /teamcompositions/{id}` - Détail d'un schéma
- `POST /teamcompositions` - Créer un schéma
- `PUT /teamcompositions/{id}` - Modifier un schéma
- `DELETE /teamcompositions/{id}` - Supprimer un schéma

## 🚀 Scripts disponibles

- `npm start` - Démarre le serveur de développement
- `npm run build` - Construit l'application pour la production
- `npm test` - Lance les tests
- `npm run eject` - Éjecte la configuration (irréversible)

## 📝 Fonctionnalités

### ✅ Implémentées
- ✅ Authentification complète (login/register) avec JWT
- ✅ Navigation et routage avec protection
- ✅ Dashboard avec statistiques en temps réel
- ✅ Éditeur de compositions (interface de base)
- ✅ Page de profil utilisateur
- ✅ Design responsive selon les maquettes
- ✅ Gestion complète des erreurs API
- ✅ Refresh automatique des tokens
- ✅ Variables d'environnement configurées

### 🚧 En développement
- 🔄 Éditeur de terrain interactif avec drag & drop
- 🔄 Export des schémas (PDF, SVG, PNG)
- 🔄 Système de favoris
- 🔄 Templates prédéfinis
- 🔄 Mise à jour du profil utilisateur

## 🎯 Prochaines étapes

1. **Démarrer le backend** sur le port 5000
2. **Tester l'authentification** avec un vrai compte
3. **Implémenter l'éditeur de terrain** interactif
4. **Ajouter la fonctionnalité d'export**
5. **Créer un système de templates**
6. **Améliorer la gestion des erreurs**
7. **Ajouter des tests unitaires et d'intégration**

## 🔍 Dépannage

### Erreur de connexion au serveur
Si vous voyez "Impossible de se connecter au serveur", vérifiez que :
1. Votre backend est démarré sur le port 5000
2. L'URL dans `.env` est correcte
3. Aucun pare-feu ne bloque la connexion

### Erreur d'authentification
Si l'authentification échoue :
1. Vérifiez que les identifiants sont corrects
2. Assurez-vous que l'utilisateur existe dans la base de données
3. Vérifiez les logs du backend pour plus de détails
