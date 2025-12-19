# Authentication and Authorization

> [⬅️ Back to Main README](../../README.md) | [📚 Documentation Hub](../README.md)

## 🔐 Authentication System

The project uses **ASP.NET Core Identity** + **JWT** for authentication and authorization.

## 🎭 Roles

| Rol      | Description          | Access                           |
| -------- | -------------------- | -------------------------------- |
| `Admin`  | System administrator | Complete dashboard, complete API |
| `Client` | Registered customer  | Acceso limitado                  |

## 🔑 JWT (JSON Web Tokens)

### Configuration

```csharp
//JWT configured in Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });
```

### Environment Variables

```env
JWT__Key=clave_secreta_minimo_32_caracteres
JWT__Issuer=Firmness.Api
JWT__Audience=FirmnessClients
```

### Generar Token

```csharp
var claims = new[]
{
new Claim(ClaimTypes.NameIdentifier, user.Id),
new Claim(ClaimTypes.Name, user.UserName),
new Claim(ClaimTypes.Email, user.Email),
// Add roles
...roles.Select(role => new Claim(ClaimTypes.Role, role))
};

var token = new JwtSecurityToken(
issuer: jwtIssuer,
audience: jwtAudience,
claims: claims,
expires: DateTime.UtcNow.AddHours(1),
signingCredentials:creds
);

return new JwtSecurityTokenHandler().WriteToken(token);
```

### Use Token

**In Headers:**

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 🔒 Protect Endpoints

### In Controllers

```csharp
// Requires authentication (for any user)
[Authorize]
public class MyController : ControllerBase { }

// Requires a specific role
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase { }

// Requires a custom policy
[Authorize(Policy = "RequireAdminRole")]
public class CustomersApiController : ControllerBase { }
```

### In Actions

```csharp
public class MyController : ControllerBase
{
    // public endpoint
    [AllowAnonymous]
    public IActionResult public() { }

    // Requires authentication
    [Authorize]
    public IActionResult Protected() { }

    // Admin only
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly() { }
}
```

## 👤Identity

### ApplicationUser

Extends `IdentityUser`:

```csharp
public class ApplicationUser : IdentityUser
{
public string FirstName { get; set; }
public string LastName { get; set; }
}
```

### Relationship with Domain

```
ApplicationUser (Identity) ←→ Customer (Domain)
IdentityUserId
```

Each `Customer` has an `IdentityUserId` that points to the authenticating user.

## 🚀 Authentication Flow

### 1. Registration

```
POST /api/Auth/register-client

        ↓
Create ApplicationUser (Identity)

        ↓
Assign "Client" role

        ↓
Create linked Customer (Domain)

        ↓
Generate JWT token

        ↓
  Return token
```

### 2. Login

```
POST /api/Auth/login

Validate credentials (Identity)

Get user roles

Generate JWT token with claims

Return token
```

### 3. Authenticated Request

```
GET /api/CustomersApi
Header: Authorization Bearer {token}
        ↓
Middleware validates JWT
        ↓
Extract claims (userId, roles)
        ↓
Policy/role validation
        ↓
Execute action
```

## 🔧 Policy Settings

```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
options.AddPolicy("RequireAdminRole", policy =>
policy.RequireRole("Admin"));
});
```

## 🧪 Testing

### Get Token

```bash
curl -X POST https://localhost:7192/api/Auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"admin@test.com","password":"Admin123!"}'
```

### Use Token

```bash
curl -X GET https://localhost:7192/api/CustomersApi \
    -H "Authorization: Bearer eyJhbG..."
```

## 📚 Referencias

- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT Authentication](https://jwt.io/introduction)

---

<div align="center">
  <a href="../../README.md">⬅️ Back to Main README</a> | 
  <a href="../README.md">📚 Documentation Hub</a> | 
  <a href="ENDPOINTS.md">🔌 API Endpoints</a>
</div>
