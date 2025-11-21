# API Endpoints - Firmness

## 🔗 Base URL

```
Development: https://localhost:7192/swagger/index.html
Production:  https://your-domain.com/api
```

## 📚 Swagger UI

Interactive documentation available at:
```
https://localhost:7192/swagger/index.html
```

---

## 🔐 Authentication

### POST `/Auth/login`

**Description:** Authenticate existing user

**Authentication:** Not required

**Request Body:**
```json
{
  "email": "admin@firmness.com",
  "password": "Admin123!"
}
```

**Response 200 (Success):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresInSeconds": 3600
}
```

**Errors:**
- `401 Unauthorized` - Invalid credentials
- `400 Bad Request` - Validation failed

---

### POST `/Auth/register-client`

**Description:** Register new client

**Authentication:** Not required (public endpoint)

**Request Body:**
```json
{
  "username": "client123",
  "email": "client@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "document": "123456789",
  "phone": "3001234567",
  "address": "123 Main Street"
}
```

**Response 200 (Success):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresInSeconds": 3600
}
```

**Errors:**
- `409 Conflict` - Email already registered
- `409 Conflict` - Username already in use
- `400 Bad Request` - Validation failed (weak password, required fields, etc.)

**Notes:**
- Automatically creates a `Customer` and an `ApplicationUser` (Identity)
- Automatically assigns the `Client` role
- Sends welcome email (if configured)
- Returns JWT token for immediate use

---

## 👥 Customers API

> **⚠️ All Customers endpoints require JWT authentication and `Admin` role**

### GET `/CustomersApi`

**Description:** List all clients with pagination

**Authentication:** Bearer Token (role: `Admin`)

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `pageNumber` | int | 1 | Page number |
| `pageSize` | int | 10 | Page size |

**Request:**
```http
GET /api/CustomersApi?pageNumber=1&pageSize=10
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response 200:**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "phone": "3001234567",
      "address": "123 Main Street",
      "document": "123456789",
      "isActive": true,
      "identityUserId": "user-id-guid"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalCount": 50,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

### GET `/CustomersApi/{id}`

**Description:** Get a specific customer by ID

**Authentication:** Bearer Token (role: `Admin`)

**Path Parameters:**
- `id` (Guid) - Customer ID

**Response 200:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phone": "3001234567",
  "address": "123 Main Street",
  "document": "123456789",
  "isActive": true,
  "identityUserId": "user-id-guid",
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T10:30:00Z"
}
```

**Errors:**
- `404 Not Found` - Customer does not exist

---

### POST `/CustomersApi`

**Description:** Create a new customer

**Authentication:** Bearer Token (role: `Admin`)

**Request Body:**
```json
{
  "firstName": "Mary",
  "lastName": "Smith",
  "email": "mary@example.com",
  "phone": "3009876543",
  "address": "456 Oak Avenue",
  "document": "987654321",
  "isActive": true
}
```

**Response 201 (Created):**
```json
{
  "id": "new-customer-guid",
  "firstName": "Mary",
  "lastName": "Smith",
  "email": "mary@example.com",
  "phone": "3009876543",
  "address": "456 Oak Avenue",
  "document": "987654321",
  "isActive": true
}
```

**Errors:**
- `400 Bad Request` - Validation failed
- `409 Conflict` - Email already exists

---

### PUT `/CustomersApi/{id}`

**Description:** Update an existing customer

**Authentication:** Bearer Token (role: `Admin`)

**Path Parameters:**
- `id` (Guid) - Customer ID

**Request Body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John Michael",
  "lastName": "Doe Smith",
  "email": "john.doe@example.com",
  "phone": "3001234567",
  "address": "789 New Street",
  "document": "123456789",
  "isActive": true
}
```

**Response 200:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John Michael",
  "lastName": "Doe Smith",
  "email": "john.doe@example.com",
  "phone": "3001234567",
  "address": "789 New Street",
  "document": "123456789",
  "isActive": true
}
```

**Errors:**
- `404 Not Found` - Customer does not exist
- `400 Bad Request` - Validation failed

---

### DELETE `/CustomersApi/{id}`

**Description:** Delete a customer and their associated Identity user

**Authentication:** Bearer Token (role: `Admin`)

**Path Parameters:**
- `id` (Guid) - Customer ID

**Response 204 (No Content)**

**Errors:**
- `404 Not Found` - Customer does not exist

**⚠️ Important:**
- Deletes the `Customer` from the table
- Deletes the associated `ApplicationUser` (Identity)
- Deletes all related records (UserRoles, UserTokens, etc.)
- This operation is **irreversible**
- GDPR compliant (complete data deletion)

---

## 🔑 JWT Authentication

### How to Authenticate

#### 1. Obtain Token

```bash
curl -X POST "https://localhost:7192/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@firmness.com",
    "password": "Admin123!"
  }'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyLWlkIiwibmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwiZXhwIjoxNzMyMTIzNDU2fQ...",
  "expiresInSeconds": 3600
}
```

#### 2. Use Token in Requests

**Header:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Complete example with curl:**
```bash
# 1. Login and save token
TOKEN=$(curl -s -X POST "https://localhost:7192/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@firmness.com","password":"Admin123!"}' \
  | jq -r '.token')

# 2. Use token to consume protected endpoint
curl -X GET "https://localhost:7192/api/CustomersApi" \
  -H "Authorization: Bearer $TOKEN"
```

**Example with PowerShell:**
```powershell
# 1. Login
$response = Invoke-RestMethod -Uri "https://localhost:7192/api/Auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{"email":"admin@firmness.com","password":"Admin123!"}'

$token = $response.token

# 2. Use token
$headers = @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Uri "https://localhost:7192/api/CustomersApi" -Headers $headers
```

### Token Claims

The JWT token includes the following claims:

- `sub` - User ID (GUID)
- `email` - User email
- `name` - Username
- `role` - User role(s) (Admin, Client)
- `exp` - Expiration date (Unix timestamp)

**Token duration:** 1 hour (3600 seconds)

---

## 📝 Roles and Permissions

| Role | Description | Access |
|------|-------------|--------|
| `Admin` | System administrator | Full access to all endpoints |
| `Client` | Registered client | Limited access (future) |

### Public Endpoints (no authentication)
- `POST /Auth/login`
- `POST /Auth/register-client`

### Protected Endpoints (require JWT)
- All `/CustomersApi/*` endpoints require `Admin` role

---

## 🧪 Testing

### Option 1: Swagger UI (Recommended)

1. Open: https://localhost:7192/swagger
2. Click "Authorize" 🔓
3. Enter: `Bearer {your-token}`
4. Test endpoints directly

### Option 2: REST Client (VS Code)

Use the included [`TEST_ENDPOINTS.http`](TEST_ENDPOINTS.http) file:

```http
### 1. Login
POST https://localhost:7192/api/Auth/login
Content-Type: application/json

{
  "email": "admin@firmness.com",
  "password": "Admin123!"
}

### 2. Get Customers (use token from step 1)
GET https://localhost:7192/api/CustomersApi
Authorization: Bearer {{token}}
```

### Option 3: PowerShell Scripts

```bash
# Test authentication
./docs/api/test-auth.ps1

# Test API endpoints
./docs/api/test-api.ps1
```

---

## 📋 HTTP Status Codes

| Code | Meaning |
|------|---------|
| `200 OK` | Successful request |
| `201 Created` | Resource created successfully |
| `204 No Content` | Successful deletion |
| `400 Bad Request` | Invalid data or validation failed |
| `401 Unauthorized` | Invalid or missing token |
| `403 Forbidden` | Insufficient permissions |
| `404 Not Found` | Resource not found |
| `409 Conflict` | Conflict (e.g., duplicate email) |
| `500 Internal Server Error` | Server error |

---

## 🔒 Security Notes

- ✅ All passwords must be at least 6 characters
- ✅ JWT tokens expire after 1 hour
- ✅ Customer deletion is irreversible
- ✅ Only users with `Admin` role can manage customers
- ✅ Public endpoints have rate limiting (future)

---

## 📚 More Information

- **[Authentication and Authorization](AUTHENTICATION.md)** - JWT details, roles and policies
- **[Test Collection](TEST_ENDPOINTS.http)** - Complete examples with REST Client
- **[Swagger UI](https://localhost:7192/swagger)** - Interactive documentation

---

## 🐛 Common Errors

### 401 Unauthorized

**Cause:** Invalid or missing token

**Solution:**
1. Verify you're sending the `Authorization: Bearer {token}` header
2. Verify the token hasn't expired (1 hour)
3. Login again to get a fresh token

### 403 Forbidden

**Cause:** You don't have the required role

**Solution:**
- `/CustomersApi` endpoints require `Admin` role
- Verify your user has the correct role

### 409 Conflict

**Cause:** Duplicate email or username

**Solution:**
- Use a different email/username
- Or delete the existing user first (from the dashboard)

### SSL Certificate Error

**Cause:** Development certificate not trusted

**Solution:**
```bash
# Trust the development certificate
dotnet dev-certs https --trust
```
