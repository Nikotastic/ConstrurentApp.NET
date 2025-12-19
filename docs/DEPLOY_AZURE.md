# Guía de Despliegue Gratuito en Azure + Supabase

Esta guía te ayudará a deployar **ConstrurentApp.NET** de forma gratuita.

## 1. Base de Datos (Supabase)

1. Crea un proyecto en [Supabase](https://supabase.com/).
2. Ve a **Project Settings > Database**.
3. Busca la sección **Connection String** y selecciona **URI**.
4. Copia la URL (será algo como `postgresql://postgres.[ID]:[PASSWORD]@...supabase.com:5432/postgres`).
   - _Nota: Asegúrate de reemplazar `[PASSWORD]` con tu contraseña real._

## 2. Backend (Azure App Service)

Azure ofrece un plan gratuito llamado **F1 (Free)**.

1. Entra al [Portal de Azure](https://portal.azure.com/).
2. Crea un **Web App**:
   - **Runtime stack:** .NET 8 (LTS).
   - **Operating System:** Linux.
   - **Region:** Selecciona una cercana (ej. East US).
   - **Pricing Plan:** Selecciona "Free F1".
3. Una vez creado, ve a **Settings > Environment variables** y añade:
   - `CONN_STR`: (La URL de Supabase que copiaste arriba).
   - `JWT__Key`: Una clave secreta larga (32+ caracteres).
   - `ENABLE_SWAGGER`: `true` (opcional, para ver Swagger en producción).
   - `ASPNETCORE_ENVIRONMENT`: `Production`.

## 3. Frontend (Azure Static Web Apps)

Este servicio es ideal para Angular y es gratis para siempre.

1. En el Portal de Azure, busca **Static Web Apps**.
2. Crea una nueva:
   - **Source:** GitHub (conecta tu repo).
   - **Build Presets:** Angular.
   - **App location:** `/client`.
   - **Output location:** `dist/client/browser` (verifica esto ejecutando `npm run build` localmente).
3. Una vez deployado, Azure te dará una URL (ej. `https://proud-sea-12345.azurestaticapps.net`).

## 4. Conexión Final

1. En el código de tu frontend, actualiza `client/src/environments/environment.prod.ts` con la URL de tu **Backend** (Azure Web App).
2. Haz `push` de tus cambios a GitHub.
3. El despliegue se iniciará automáticamente.

---
