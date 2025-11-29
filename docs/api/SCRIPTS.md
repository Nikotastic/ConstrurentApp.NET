# 🔌 API Testing Scripts

Utility scripts for testing API endpoints, authentication, and functionality.

## 📂 Available Scripts

All scripts are located in `docs/api/` directory.

### 🔐 Authentication Scripts

#### `test-auth.ps1`

**Purpose:** Test authentication endpoints (login, register, token validation).

**Usage:**

```powershell
.\docs\api\test-auth.ps1
```

**What it does:**

- Tests user registration endpoint
- Tests login endpoint
- Validates JWT token generation
- Checks token expiration
- Tests protected endpoints with token

**When to use:**

- After implementing authentication changes
- To verify JWT configuration
- When troubleshooting login issues

---

### 🧪 General API Testing

#### `test-api.ps1`

**Purpose:** Comprehensive API endpoint testing suite.

**Usage:**

```powershell
.\docs\api\test-api.ps1
```

**What it does:**

- Tests all major API endpoints
- Validates request/response formats
- Checks HTTP status codes
- Tests CRUD operations
- Verifies data validation
- Tests error handling

**Endpoints tested:**

- `/api/auth/login`
- `/api/auth/register`
- `/api/customers`
- `/api/products`
- `/api/vehicles`
- `/api/rentals`
- `/api/sales`

**When to use:**

- After API changes or updates
- Before deployment
- For integration testing
- To verify API is working correctly

---

## 🚀 Quick Start

### Test Authentication Flow

```powershell
# Test complete auth flow
.\docs\api\test-auth.ps1
```

### Test All Endpoints

```powershell
# Run comprehensive API tests
.\docs\api\test-api.ps1
```

---

## 📋 Prerequisites

- PowerShell 5.1 or higher
- API running (locally or Docker)
- Network access to API endpoint

### Default API Endpoints

- **Local Development:** `http://localhost:5000`
- **Docker:** `http://localhost:5000`
- **Swagger UI:** `http://localhost:5000/swagger`

---

## 🔧 Customization

### Change API Base URL

Edit the script and modify the base URL:

```powershell
# In test-api.ps1 or test-auth.ps1
$baseUrl = "http://localhost:5000/api"
# Change to your API URL
```

### Add Custom Tests

You can extend the scripts with your own test cases:

```powershell
# Example: Test custom endpoint
$response = Invoke-RestMethod -Uri "$baseUrl/custom/endpoint" -Method Get
Write-Host "Response: $response"
```

---

## 📊 Understanding Test Results

### Success Indicators

- ✅ Green "PASS" messages
- HTTP 200/201 status codes
- Valid JSON responses
- Expected data structure

### Failure Indicators

- ❌ Red "FAIL" messages
- HTTP 400/401/500 status codes
- Empty or malformed responses
- Exception errors

---

## 🔍 Troubleshooting

### "Connection refused"

**Solution:**

- Verify API is running: `docker ps`
- Check API URL is correct
- Ensure port 5000 is not blocked

### "401 Unauthorized"

**Solution:**

- Check authentication credentials
- Verify JWT token is valid
- Ensure token is included in request headers

### "500 Internal Server Error"

**Solution:**

- Check API logs: `docker logs firmness-api`
- Verify database connection
- Check for missing configuration

---

## 📚 Related Documentation

- **[API Endpoints](ENDPOINTS.md)** - Complete API reference
- **[Authentication](AUTHENTICATION.md)** - JWT authentication guide
- **[AI Scripts](../AI/SCRIPTS.md)** - AI chatbot testing scripts

---

## 💡 Tips

- Run scripts from the **project root directory**
- Ensure API is running before testing
- Check Swagger UI for interactive API testing: `http://localhost:5000/swagger`
- Use verbose mode for detailed output: `$VerbosePreference = "Continue"`
