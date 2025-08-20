import axios, { AxiosInstance } from 'axios';
import { 
  AuthResponse, 
  LoginDto, 
  RegisterDto, 
  RefreshTokenRequest, 
  TeamComposition, 
  CreateTeamCompositionDto, 
  UpdateTeamCompositionDto,
  UserProfile,
  UpdateUserDto
} from '../types/api';

class ApiService {
  private api: AxiosInstance;
  private baseURL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

  constructor() {
    this.api = axios.create({
      baseURL: this.baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Intercepteur pour ajouter le token d'authentification
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Intercepteur pour gérer les erreurs d'authentification
    this.api.interceptors.response.use(
      (response) => response,
      async (error) => {
        // Ne pas intercepter les erreurs de login/register pour permettre l'affichage des erreurs
        if (error.config?.url?.includes('/auth/login') || error.config?.url?.includes('/auth/register')) {
          return Promise.reject(error);
        }

        if (error.response?.status === 401) {
          // Token expiré, essayer de le rafraîchir
          const refreshToken = localStorage.getItem('refreshToken');
          if (refreshToken) {
            try {
              const response = await this.refreshToken({ refreshToken });
              localStorage.setItem('token', response.token);
              localStorage.setItem('refreshToken', response.refreshToken);
              
              // Retenter la requête originale
              error.config.headers.Authorization = `Bearer ${response.token}`;
              return this.api.request(error.config);
            } catch (refreshError) {
              // Échec du refresh, déconnexion
              localStorage.removeItem('token');
              localStorage.removeItem('refreshToken');
              window.location.href = '/login';
            }
          } else {
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
            window.location.href = '/login';
          }
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  async register(data: RegisterDto): Promise<{ message: string }> {
    const response = await this.api.post('/auth/register', data);
    return response.data;
  }

  async login(data: LoginDto): Promise<AuthResponse> {
    try {
      const response = await this.api.post('/auth/login', data);
      return response.data;
    } catch (error: any) {
      throw error;
    }
  }

  async refreshToken(data: RefreshTokenRequest): Promise<AuthResponse> {
    const response = await this.api.post('/auth/refresh', data);
    return response.data;
  }

  // Team compositions endpoints
  async getTeamCompositions(): Promise<TeamComposition[]> {
    const response = await this.api.get('/teamcompositions');
    return response.data;
  }

  async getTeamComposition(id: number): Promise<TeamComposition> {
    const response = await this.api.get(`/teamcompositions/${id}`);
    return response.data;
  }

  async createTeamComposition(data: CreateTeamCompositionDto): Promise<TeamComposition> {
    const response = await this.api.post('/teamcompositions', data);
    return response.data;
  }

  async updateTeamComposition(id: number, data: UpdateTeamCompositionDto): Promise<void> {
    await this.api.put(`/teamcompositions/${id}`, data);
  }

  async deleteTeamComposition(id: number): Promise<void> {
    await this.api.delete(`/teamcompositions/${id}`);
  }

  // User profile endpoints
  async getUserProfile(): Promise<UserProfile> {
    const response = await this.api.get('/profile');
    return response.data;
  }

  async updateUserProfile(data: UpdateUserDto): Promise<{ token: string }> {
    const response = await this.api.put('/profile', data);
    return response.data;
  }
}

export const apiService = new ApiService(); 