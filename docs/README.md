# Documentation - Firmness

Technical documentation organized by category.

---

## 🔧 Setup and Configuration

Guides to configure the project.

- **[Environment Variables](setup/ENVIRONMENT.md)** - Configure `.env`, database credentials
- **[Email](setup/EMAIL_CONFIGURATION.md)** - Configure Gmail SMTP for notifications

**Scripts:**

- `setup/fix-email-auth.ps1` - Configure/fix email authentication

---

## 💻 Development

Guides for developers.

- **[Architecture](development/ARCHITECTURE.md)** - Clean Architecture, layers, patterns
- **[Migrations](development/MIGRATIONS.md)** - Entity Framework migrations, commands

---

## 🤖 AI Chatbot

Documentation for the intelligent chatbot powered by Google Gemini.

- **[Setup Guide](AI/README.md)** - Configure and test the AI chatbot
- **[Architecture](AI/ARCHITECTURE.md)** - Technical implementation and customization
- **[Troubleshooting](AI/TROUBLESHOOTING.md)** - Common issues and solutions

**Scripts:**

- `AI/setup-gemini-key.ps1` - Configure Gemini API Key
- `AI/verify-gemini-config.ps1` - Verify configuration
- `AI/test-chatbot.ps1` - Quick functionality test
- `AI/diagnose-chatbot.ps1` - Complete diagnostic tool

---

## 🔌 API

REST API documentation.

- **[Endpoints](api/ENDPOINTS.md)** - Complete list of endpoints
- **[Authentication](api/AUTHENTICATION.md)** - JWT, roles, authorization

**Testing:**

- `api/TEST_ENDPOINTS.http` - Request collection (REST Client / VS Code)
- `api/test-api.ps1` - Script to test endpoints
- `api/test-auth.ps1` - Script to test authentication

---

## 🚀 Quick Start

### Getting Started

1. [Environment Variables](setup/ENVIRONMENT.md) - Configure `.env`
2. [Migrations](development/MIGRATIONS.md) - Apply DB schema
3. Run: `dotnet run --project src/Firmness.Api`

### For Development

1. [Architecture](development/ARCHITECTURE.md) - Understand the structure
2. [Migrations](development/MIGRATIONS.md) - Work with the DB
3. [API](api/ENDPOINTS.md) - Develop endpoints

### For Integration

1. [Endpoints](api/ENDPOINTS.md) - Complete list of endpoints
2. [Authentication](api/AUTHENTICATION.md) - How to authenticate
3. `api/TEST_ENDPOINTS.http` - Usage examples

---

## 📁 Structure

```
docs/
├── setup/              # Initial setup
│   ├── ENVIRONMENT.md          # Environment variables
│   ├── EMAIL_CONFIGURATION.md  # Configure email
│   └── fix-email-auth.ps1      # Email script
│
├── development/        # Development
│   ├── ARCHITECTURE.md  # Clean Architecture
│   └── MIGRATIONS.md    # EF Core migrations
│
├── AI/                 # AI Chatbot
│   ├── README.md            # Setup guide
│   ├── ARCHITECTURE.md      # Technical details
│   ├── TROUBLESHOOTING.md   # Common issues
│   ├── setup-gemini-key.ps1
│   ├── verify-gemini-config.ps1
│   ├── test-chatbot.ps1
│   └── diagnose-chatbot.ps1
│
└── api/                # REST API
    ├── ENDPOINTS.md         # Endpoint list
    ├── AUTHENTICATION.md    # JWT and security
    └── TEST_ENDPOINTS.http  # Test collection
```

---

## 🔄 Back to Home

[← Project main README](../README.md)
