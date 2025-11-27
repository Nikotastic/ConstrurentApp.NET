# 🤖 AI Chatbot (Gemini) - Documentation & Configuration

This directory contains documentation, scripts, and tools related to the AI Chatbot integrated into the Firmness application. The chatbot uses **Google Gemini 2.0 Flash** to provide intelligent responses based on real-time data from the database (vehicles, products, prices, availability).

## 📋 Features

- **Inventory Query**: Responds to questions about available vehicles and machinery.
- **Product Query**: Information on products for sale, prices, and stock.
- **Real-Time Data**: Connects to the PostgreSQL database to fetch up-to-date information.
- **Expense Analysis**: Ability to analyze expense JSONs and provide financial recommendations.
- **Multi-language**: Optimized to respond in Spanish (Colombia), but adaptable.
- **Function Calling**: Intelligently decides when to query the database.

## 📂 Directory Structure

- **Configuration & Test Scripts**:

  - `setup-gemini-key.ps1`: Script to securely configure your Gemini API Key.
  - `verify-gemini-config.ps1`: Verifies that the current configuration is correct.
  - `test-chatbot.ps1`: Performs a quick functional test of the chatbot.
  - `diagnose-chatbot.ps1`: Complete diagnostic tool for troubleshooting.
  - `test-gemini.sh`: Test script for Linux/WSL environments.

- **Documentation**:
  - `TROUBLESHOOTING.md`: Guide for common problems and known errors.

## 🚀 Step-by-Step Configuration Guide

### Prerequisites

- Docker Desktop installed and running.
- A Google Gemini API Key (get it for free at [Google AI Studio](https://makersuite.google.com/app/apikey)).

### Step 1: Configure the API Key

You have two options to configure the API Key:

**Option A: Using the automatic script (Recommended)**
Run the following command in PowerShell and follow the instructions:

```powershell
.\docs\AI\setup-gemini-key.ps1
```

**Option B: Manually**
Create or edit the `.env` file in the project root and add your key:

```env
GEMINI_API_KEY=Your_Key_Here_AIzaSy...
Gemini__Model=gemini-2.0-flash
```

### Step 2: Verify Configuration

Run the verification script to ensure everything is correct:

```powershell
.\docs\AI\verify-gemini-config.ps1
```

### Step 3: Run the Application

We recommend using Docker to run the entire system:

```powershell
# From the project root
docker-compose up -d --build
```

### Step 4: Test the Chatbot

Once the containers are running, you can test the chatbot in several ways:

1. **Test Script**:

   ```powershell
   .\docs\AI\test-chatbot.ps1
   ```

2. **From the Browser**:
   - Open `http://localhost`
   - Click on the chatbot icon in the bottom right corner.
   - Ask something like: "¿Qué maquinaria tienen disponible?" (What machinery is available?)

## 🏗️ Architecture & Developer Guide

For a deep dive into the system architecture, code structure, and customization options, please read the **[Architecture & Developer Guide](ARCHITECTURE.md)**.

## 🔧 Troubleshooting

If you encounter errors (like 404, 500, or empty responses), check the [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) file for detailed solutions to common problems.
