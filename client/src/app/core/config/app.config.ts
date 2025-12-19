
  // API Configuration

export const API_CONFIG = {
  BASE_URL: '/api',
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/auth/login',
      REGISTER: '/auth/register-client',
      CURRENT_USER: '/auth/current-user',
    },
    PRODUCTS: '/products',
    CATEGORIES: '/categories',
    CUSTOMERS: '/customers',
    SALES: '/sales',
    CART: '/cart',
  },
  TIMEOUT: 30000, // 30 seconds
} as const;


// Storage Keys

export const STORAGE_KEYS = {
  AUTH_TOKEN: 'firmness_auth_token',
  TOKEN_EXPIRY: 'firmness_token_expiry',
  USER_PREFERENCES: 'firmness_user_preferences',
} as const;

// Route Paths

export const ROUTES = {
  HOME: '',
  LOGIN: 'login',
  REGISTER: 'register',
  DASHBOARD: 'dashboard',
  PRODUCTS: 'products',
  CART: 'cart',
  ORDERS: 'orders',
  PROFILE: 'profile',
} as const;


// User Roles
export const USER_ROLES = {
  ADMIN: 'Admin',
  CLIENT: 'Client',
} as const;


// Validation Patterns
export const VALIDATION_PATTERNS = {
  EMAIL: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
  PHONE: /^[0-9]{10}$/,
  PASSWORD_MIN_LENGTH: 6,
  USERNAME_MIN_LENGTH: 3,
} as const;
