# 🚀 Deployment – Firmness ERP

## Essential Files

- `docker-compose.prod.yml` - Service orchestration
- `.env` - Environment variables 
- `client/Dockerfile` - Frontend build
- `client/nginx.conf` - Nginx configuration
- `src/Firmness.Api/Dockerfile` - Backend build
- `src/Firmness.Web/Dockerfile` - Admin panel build

## Deployment Local

```bash
docker-compose -f docker-compose.prod.yml up -d --build
```

## Deployment en VPS

```bash
# 1. Upload the project to the VPS
git clone https://github.com/your-user/ConstrurentApp.NET.git
cd ConstrurentApp.NET

# 2. Edit .env with your production values
nano .env

# 3. Deploy
docker-compose -f docker-compose.prod.yml up -d --build

# 4. Migrations are applied automatically when the API starts

```

## Access

- **Frontend (Clients)**: http://localhost (o http://TU_IP_VPS)
- **Admin Panel**: http://localhost:5001
- **API**: http://localhost:5000/api
- **Swagger**: http://localhost:5000/swagger
- **PgAdmin**: http://localhost:8080

### Credenciales por Defecto

**Default Credentials:**

- Email: `client@firmness.local`
- Password: `Client123!`

**Administrator:**

- Email: `admin@firmness.local`
- Password: `Admin123!`

## Useful Commands

```bash
# View logs
docker-compose -f docker-compose.prod.yml logs -f

# View logs of a specific service
docker-compose -f docker-compose.prod.yml logs -f api

# Restart
docker-compose -f docker-compose.prod.yml restart

# Stop
docker-compose -f docker-compose.prod.yml down

# Update
git pull && docker-compose -f docker-compose.prod.yml up -d --build

```

## Configuration

All variables are stored in .env.
The most important ones:

### Database

- `POSTGRES_PASSWORD` - Database password
- `POSTGRES_DB` - Database name
- `POSTGRES_USER` - PostgreSQL user

### Security

- `JWT__Key` - JWT key (already generated, 64 alphanumeric characters)
- `JWT__Issuer` - Token issuer
- `JWT__Audience` - Token audience

### Email (Gmail SMTP)

To enable sending purchase receipts by email, configure:

```env
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__SmtpPort=587
EmailSettings__SenderEmail=tu-email@gmail.com
EmailSettings__SenderName=Firmness
EmailSettings__Username=tu-email@gmail.com
EmailSettings__Password=tu-app-password
EmailSettings__EnableSsl=true
EmailSettings__TimeoutSeconds=30
```

**Note**: For Gmail, you must generate an App Password:

1. Go to https://myaccount.google.com/security
2. Enable 2-Step Verification
3. Generate an App Password
4. Use that password in `EmailSettings__Password`

### Ports

- `API_PORT=5000` - Puerto de la API
- `ADMIN_PORT=5001` - Puerto del panel de administración
- `CLIENT_PORT=80` - Puerto del frontend de clientes
- `PGADMIN_PORT=8080` - Puerto de PgAdmin

## New Features

### Guards de Autenticación

- **Frontend (Cliente)**: Only users with the Client role may access
- **Admin Panel**: Only users with the Admin role may access
- Authenticated users cannot return to login/register unless they log out

### Registro de Clientes

- Endpoint: `POST /api/auth/register-client`
- Frontend route: `/register`
- After registering, the user is redirected to the catalog

### Email Receipt Sending

- Endpoint: `POST /api/sales/{id}/send-receipt`
- Automatically sends a purchase receipt by email
- Email includes invoice number and purchase total

For production, make sure to change passwords to secure values.
