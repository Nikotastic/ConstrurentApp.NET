# 🏗️ AI Chatbot Architecture & Developer Guide

This document provides a deep dive into the technical implementation, architecture, and customization options for the Firmness AI Chatbot.

## 🏗️ System Architecture

The system follows a clean architecture pattern where the AI service acts as an intelligent orchestrator between the user and the database.

```mermaid
graph TD
    User[User (Angular Client)] -->|POST /api/chat/message| Nginx[Nginx Proxy]
    Nginx -->|Forward| API[Firmness.Api (.NET 8)]
    API -->|Route| Controller[ChatController]
    Controller -->|Use| Service[SmartGeminiChatService]

    subgraph AI Logic
        Service -->|1. Analyze Intent| Gemini[Google Gemini 2.0 Flash]
        Gemini -->|2. Request Function| Service
        Service -->|3. Execute Function| Executor[GeminiFunctionExecutor]
    end

    subgraph Data Access
        Executor -->|Query| VehicleService
        Executor -->|Query| ProductService
        VehicleService -->|Read| DB[(PostgreSQL)]
        ProductService -->|Read| DB
    end

    Executor -->|4. Return Data| Service
    Service -->|5. Send Data + Prompt| Gemini
    Gemini -->|6. Generate Response| Service
    Service -->|7. Return Response| User
```

### Key Components

1.  **SmartGeminiChatService**: The core service that manages the conversation state and interaction with the Gemini API.
2.  **GeminiFunctionExecutor**: Handles the execution of "tools" (functions) requested by the AI, such as querying the database.
3.  **GeminiToolDefinitions**: Defines the schema of available functions (`get_available_vehicles`, `analyze_expenses`, etc.) so the AI knows how to use them.

---

## 📁 Key Files

| File                                                             | Purpose                                                 |
| ---------------------------------------------------------------- | ------------------------------------------------------- |
| `src/Firmness.Application/Services/AI/SmartGeminiChatService.cs` | Main logic, prompt construction, and API communication. |
| `src/Firmness.Application/Services/AI/GeminiFunctionExecutor.cs` | Implementation of the actual logic for each tool.       |
| `src/Firmness.Application/Services/AI/GeminiToolDefinitions.cs`  | JSON schema definitions for the tools.                  |
| `src/Firmness.Api/Controllers/ChatController.cs`                 | API Endpoint definition (`/api/chat/message`).          |
| `src/Firmness.Application/AI/DTOs/*.cs`                          | Simplified DTOs optimized for token usage efficiency.   |

---

## 🛠️ Customization Guide

### 1. Modifying the System Prompt

To change the chatbot's personality or base instructions, edit the `_systemInstruction` in `SmartGeminiChatService.cs`.

```csharp
// Example in SmartGeminiChatService.cs
var systemInstruction = @"
    You are an intelligent assistant for Firmness.
    Your tone should be professional yet friendly.
    Always answer in Colombian Spanish.
    ...";
```

### 2. Adding New Keywords

To improve intent detection (e.g., to handle new types of queries without calling the LLM unnecessarily), modify the keyword detection logic in `SmartGeminiChatService.cs`.

```csharp
bool askingAboutVehicles = messageLower.Contains("vehicle") ||
                           messageLower.Contains("crane"); // Added keyword
```

### 3. Adding New Tools (Functions)

To give the AI new capabilities (e.g., checking order status):

1.  **Define the Tool**: Add the JSON schema in `GeminiToolDefinitions.cs`.
2.  **Implement Logic**: Add the method in `GeminiFunctionExecutor.cs`.
3.  **Register**: Ensure the executor calls your new method when the tool name matches.

---

## 📊 Data Flow Example

**User**: "What excavators do you have?"

1.  **Detection**: Chatbot detects keywords "excavator".
2.  **Tool Call**: AI decides to call `get_available_vehicles`.
3.  **Execution**: `VehicleService` queries PostgreSQL for vehicles with `Status = 'Available'`.
4.  **Data Retrieval**:
    - _CAT 320 (Excavator) - $450/day_
    - _Komatsu PC200 (Excavator) - $500/day_
5.  **Response Generation**: Gemini receives this data and generates:
    > "We have two excavators available: the CAT 320 for $450/day and the Komatsu PC200 for $500/day. Would you like to book one?"

---

## 📜 Implementation History

For a record of technical challenges resolved during development (such as CORS issues, Docker networking, and API URL formatting), please refer to the commit history or the closed issues in the project management system. Key resolved items include:

- **Gemini API URL Duplication**: Fixed double `models/` path segment.
- **JSON Serialization**: Fixed case-sensitivity issues between Angular and .NET.
- **Docker Environment**: Ensured API keys are correctly propagated to containers.
