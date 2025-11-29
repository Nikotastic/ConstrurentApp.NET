# 🤖 AI Chatbot Scripts

Utility scripts for configuring, testing, and troubleshooting the Gemini AI Chatbot integration.

## 📂 Available Scripts

All scripts are located in `docs/AI/` directory.

### 🔧 Configuration Scripts

#### `setup-gemini-key.ps1`

**Purpose:** Interactive script to securely configure your Gemini API Key.

**Usage:**

```powershell
.\docs\AI\setup-gemini-key.ps1
```

**What it does:**

- Prompts for your Gemini API Key
- Validates the key format
- Updates the `.env` file automatically
- Configures the Gemini model version

**When to use:** First-time setup or when changing API keys.

---

#### `verify-gemini-config.ps1`

**Purpose:** Verifies that your Gemini configuration is correct and complete.

**Usage:**

```powershell
.\docs\AI\verify-gemini-config.ps1
```

**What it does:**

- Checks if `.env` file exists
- Validates `GEMINI_API_KEY` is set
- Verifies model configuration
- Tests API key format
- Displays current configuration status

**When to use:** After setup or when troubleshooting configuration issues.

---

### 🧪 Testing Scripts

#### `test-chatbot.ps1`

**Purpose:** Quick functional test of the chatbot API endpoint.

**Usage:**

```powershell
.\docs\AI\test-chatbot.ps1
```

**What it does:**

- Sends a test message to the chatbot API
- Displays the response
- Checks response time
- Validates JSON format

**When to use:** After deployment or code changes to verify chatbot is working.

---

#### `diagnose-chatbot.ps1`

**Purpose:** Comprehensive diagnostic tool for troubleshooting chatbot issues.

**Usage:**

```powershell
.\docs\AI\diagnose-chatbot.ps1
```

**What it does:**

- Checks Docker containers status
- Verifies API is running
- Tests database connectivity
- Validates Gemini API key
- Tests chatbot endpoint
- Checks logs for errors
- Provides detailed diagnostic report

**When to use:** When experiencing issues with the chatbot or for complete system health check.

---

## 🚀 Quick Start Workflow

Follow this sequence for initial setup:

1. **Setup API Key**

   ```powershell
   .\docs\AI\setup-gemini-key.ps1
   ```

2. **Verify Configuration**

   ```powershell
   .\docs\AI\verify-gemini-config.ps1
   ```

3. **Start Application**

   ```powershell
   docker compose up --build
   ```

4. **Test Chatbot**
   ```powershell
   .\docs\AI\test-chatbot.ps1
   ```

---

## 🔍 Troubleshooting Workflow

If chatbot isn't working:

1. **Run Diagnostics**

   ```powershell
   .\docs\AI\diagnose-chatbot.ps1
   ```

2. **Check Configuration**

   ```powershell
   .\docs\AI\verify-gemini-config.ps1
   ```

3. **Review Logs**

   ```powershell
   docker logs firmness-api
   ```

4. **Test Again**
   ```powershell
   .\docs\AI\test-chatbot.ps1
   ```

---

## 📚 Related Documentation

- **[AI Chatbot Guide](README.md)** - Complete AI integration documentation
- **[Architecture](ARCHITECTURE.md)** - Technical architecture and code structure
- **[Troubleshooting](TROUBLESHOOTING.md)** - Common issues and solutions

---

## 💡 Tips

- Run scripts from the **project root directory**
- Ensure Docker is running before using test scripts
- Keep your API key secure - never commit it to version control
- Use `diagnose-chatbot.ps1` for comprehensive health checks
