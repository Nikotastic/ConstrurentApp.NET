# 🔧 Troubleshooting - AI Chatbot

This guide details the most common problems encountered during the chatbot implementation and their solutions.

## 🚨 Common Issues

### 1. Error 404 Not Found at `/api/chat/message`

**Symptom**: The frontend shows a 404 error when trying to send a message.
**Cause**: The reverse proxy (Nginx) is not correctly redirecting requests to the backend, or the URL is malformed.
**Solution**:

- Verify that the `firmness-api` container is running.
- Check the Nginx configuration (`client/nginx.conf`). It must have `proxy_pass http://firmness-api:8080/api/;`.
- Ensure the path is not being duplicated (e.g., `/api/api/chat/...`).

### 2. Error "Gemini API Key not configured"

**Symptom**: API logs show that the API Key is empty or null.
**Cause**: The `GEMINI_API_KEY` environment variable is not being passed correctly to the Docker container.
**Solution**:

- Verify that the `.env` file exists in the root and contains the key.
- Ensure `docker-compose.yml` has the line `- Gemini__ApiKey=${GEMINI_API_KEY}`.
- Run `.\docs\AI\verify-gemini-config.ps1` to check.

### 3. Error "Duplicate URL models/models/..."

**Symptom**: The Gemini API returns a 404.
**Cause**: A code error was concatenating "models/" twice.
**Solution**: This error has been fixed in the code (`SmartGeminiChatService.cs`). If it persists, ensure you have the latest code version and rebuild the containers.

### 4. Chatbot responds "Sorry, there was an error..."

**Symptom**: The chatbot responds with a generic error message.
**Cause**: Could be a timeout, an unhandled exception, or the Gemini API rejected the request.
**Solution**:

- Check API logs: `docker logs firmness-api --tail 50`.
- Use the diagnostic script: `.\docs\AI\diagnose-chatbot.ps1`.

## 🛠️ Diagnostic Tools

We have included powerful scripts to help you diagnose problems:

### `diagnose-chatbot.ps1`

This script performs a full system check:

1. Verifies Internet connection.
2. Tests DNS resolution.
3. Checks local environment variables.
4. Attempts to connect directly to the Gemini API (bypassing the app).
5. Verifies that Docker containers are running.
6. Tests API endpoints locally.

**Usage**:

```powershell
.\docs\AI\diagnose-chatbot.ps1
```

### Useful Docker Commands

View real-time API logs:

```powershell
docker logs firmness-api -f
```

View client logs (Frontend/Nginx):

```powershell
docker logs firmness-client -f
```

Restart AI/Chatbot services:

```powershell
docker-compose restart api
```

Rebuild everything from scratch (useful if you changed code or configuration):

```powershell
docker-compose down
docker-compose up -d --build
```
