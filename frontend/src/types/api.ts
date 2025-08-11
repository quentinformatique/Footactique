// Types basés sur les DTOs de l'API backend

export interface PlayerPosition {
  id?: number;
  playerName: string;
  position?: string; // Ajouté pour compatibilité avec le backend
  number?: number;
  color?: string;
  x: number;
  y: number;
}

export interface TeamComposition {
  id?: number;
  name: string;
  formation: string;
  description?: string;
  isFavorite?: boolean;
  players: PlayerPosition[];
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateTeamCompositionDto {
  name: string;
  formation: string;
  description?: string;
  players: PlayerPosition[];
}

export interface UpdateTeamCompositionDto {
  name: string;
  formation: string;
  description?: string;
  players: PlayerPosition[];
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface User {
  id: string;
  username: string;
  email: string;
}

export interface UserProfile {
  id: string;
  username: string;
  email: string;
  createdAt: string;
}

export interface UpdateUserDto {
  username?: string;
  email?: string;
  currentPassword: string;
  newPassword?: string;
} 