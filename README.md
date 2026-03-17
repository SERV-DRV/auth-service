# Auth Service (.NET 8)

API de autenticación para registro, inicio de sesión, perfil de usuario, verificación de correo y recuperación de contraseña.

## 1) ¿Qué hace este servicio?

Este servicio permite:
- Registrar usuarios (`register`) con foto opcional.
- Iniciar sesión (`login`) por email o username.
- Verificar email con token.
- Reenviar verificación de email.
- Solicitar recuperación de contraseña.
- Restablecer contraseña con token.
- Consultar perfil autenticado y perfil por id.
- Exponer endpoints de salud (`/health` y `/api/v1/health`).

Tecnologías principales:
- ASP.NET Core 8
- Entity Framework Core + PostgreSQL
- JWT Bearer Authentication
- Serilog
- Swagger/OpenAPI
- Cloudinary (imágenes de perfil)
- SMTP (emails)

---

## 2) Estructura de carpetas

```text
auth-service/
├── AuthService.sln
├── docker-compose.yml
├── Dockerfile
├── LICENSE
├── README.md
└── src/
		├── AuthService.Api/
		│   ├── appsettings.json
		│   ├── appsettings.Development.json
		│   ├── Program.cs
		│   ├── Controllers/
		│   │   └── AuthController.cs
		│   ├── Extensions/
		│   │   ├── AuthenticationExtensions.cs
		│   │   ├── RateLimitingExtensions.cs
		│   │   ├── SecurityExtensions.cs
		│   │   └── ServiceCollectionExtensions.cs
		│   ├── Middlewares/
		│   │   └── GlobalExceptionMiddleware.cs
		│   ├── Model/
		│   │   ├── ErrorResponse.cs
		│   │   └── FormFileAdapter.cs
		│   ├── ModelBienders/
		│   │   └── FileDataModelBinder.cs
		│   └── Properties/
		│       └── launchSettings.json
		├── AuthService.Application/
		│   ├── DTOs/
		│   ├── Exceptions/
		│   ├── Interfaces/
		│   ├── Services/
		│   └── Validators/
		├── AuthService.Domain/
		│   ├── Constants/
		│   ├── Entities/
		│   ├── Enums/
		│   └── Interfaces/
		└── AuthService.Persistence/
				├── Data/
				├── Migrations/
				└── Repositories/
```

---

## 3) Requerimientos

- .NET SDK 8.0+
- PostgreSQL 16+ (local o en contenedor)
- Docker y Docker Compose (opcional)
- Cuenta/configuración SMTP para envío de correos
- Cuenta/configuración Cloudinary para imágenes de perfil

---

## 4) Instalación y ejecución

### 4.1 Instalación local

1. Clona el repositorio y entra a la carpeta del proyecto.
2. Restaura dependencias:

```bash
dotnet restore
```

3. Configura variables en:
	- `src/AuthService.Api/appsettings.Development.json` (cadena de conexión local)
	- `src/AuthService.Api/appsettings.json` (`JwtSettings`, `SmtpSettings`, `CloudinarySettings`)

4. Compila la solución:

```bash
dotnet build
```

5. Ejecuta el API:

```bash
dotnet run --project src/AuthService.Api
```

Swagger disponible en:
- `http://localhost:5001/swagger`
- `https://localhost:7139/swagger`

### 4.2 Instalación con Docker

```bash
docker compose up --build
```

Esto levantará PostgreSQL + API.

API expuesta en:
- `http://localhost:5001`

---

## 5) Funcionamiento general del programa

Flujo principal:
1. El cliente consume endpoints bajo `api/v1/auth`.
2. El API valida entradas (DTOs y reglas de negocio).
3. Persiste/consulta datos en PostgreSQL con EF Core.
4. Genera JWT en login exitoso.
5. Maneja emails de verificación y reset de contraseña.
6. Responde en JSON (camelCase).
7. Errores globales se unifican con `success = false`.

Seguridad y control:
- JWT para endpoints protegidos (`GET /api/v1/auth/profile`).
- Rate limiting:
	- `AuthPolicy`: 5 solicitudes por minuto por IP (auth sensible).
	- `ApiPolicy`: token bucket para endpoints generales.
- Headers de seguridad (CSP, X-Frame-Options, etc).

---

## 6) Endpoints y qué espera cada uno

Base URL local (desarrollo):
- `http://localhost:5001`
- `https://localhost:7139`

### 4.1 Salud

#### GET `/health`
Valida que el servicio esté arriba.

Respuesta ejemplo:
```json
{
	"status": "Healthy",
	"timestamp": "2026-03-17T12:00:00.000Z"
}
```

#### GET `/api/v1/health`
Healthcheck compatible para monitoreo.

---

### 4.2 Autenticación y usuario (`/api/v1/auth`)

#### 1) POST `/api/v1/auth/register`
Registra usuario nuevo.

**Tipo de body:** `multipart/form-data`

Campos esperados:
- `name` (string, requerido, máx 25)
- `surname` (string, requerido, máx 25)
- `username` (string, requerido)
- `email` (string, requerido, formato email)
- `password` (string, requerido, mínimo 8)
- `phone` (string, requerido, exactamente 8)
- `profilePicture` (archivo opcional, jpg/jpeg/png/webp, máx 5MB)

Respuesta exitosa (201):
```json
{
	"success": true,
	"user": {
		"id": "usr_xxx",
		"name": "Juan",
		"surname": "Pérez",
		"username": "juanp",
		"email": "juan@mail.com",
		"profilePicture": "https://...",
		"phone": "12345678",
		"role": "USER",
		"status": false,
		"isEmailVerified": false,
		"createdAt": "2026-03-17T12:00:00Z",
		"updatedAt": "2026-03-17T12:00:00Z"
	},
	"message": "Usuario registrado exitosamente. Por favor, verifica tu email para activar la cuenta.",
	"emailVerificationRequired": true
}
```

---

#### 2) POST `/api/v1/auth/login`
Inicia sesión por `email` o `username`.

Body JSON esperado:
```json
{
	"emailOrUsername": "juan@mail.com",
	"password": "MiPassword123"
}
```

Respuesta exitosa (200):
```json
{
	"success": true,
	"message": "Login exitoso",
	"token": "eyJhbGciOi...",
	"userDetails": {
		"id": "usr_xxx",
		"username": "juanp",
		"profilePicture": "https://...",
		"role": "USER"
	},
	"expiresAt": "2026-03-17T16:00:00Z"
}
```

---

#### 3) GET `/api/v1/auth/profile` (requiere JWT)
Obtiene el perfil del usuario autenticado.

Header requerido:
- `Authorization: Bearer <token>`

Respuesta exitosa (200):
```json
{
	"success": true,
	"message": "Perfil obtenido exitosamente",
	"data": {
		"id": "usr_xxx",
		"name": "Juan",
		"surname": "Pérez",
		"username": "juanp",
		"email": "juan@mail.com",
		"profilePicture": "https://...",
		"phone": "12345678",
		"role": "USER",
		"status": true,
		"isEmailVerified": true,
		"createdAt": "2026-03-17T12:00:00Z",
		"updatedAt": "2026-03-17T12:00:00Z"
	}
}
```

---

#### 4) POST `/api/v1/auth/profile/by-id`
Obtiene perfil por `userId`.

Body JSON esperado:
```json
{
	"userId": "usr_xxx"
}
```

---

#### 5) POST `/api/v1/auth/verify-email`
Verifica correo con token enviado por email.

Body JSON esperado:
```json
{
	"token": "token_de_verificacion"
}
```

---

#### 6) POST `/api/v1/auth/resend-verification`
Reenvía email de verificación.

Body JSON esperado:
```json
{
	"email": "juan@mail.com"
}
```

---

#### 7) POST `/api/v1/auth/forgot-password`
Solicita recuperación de contraseña.

Body JSON esperado:
```json
{
	"email": "juan@mail.com"
}
```

> Por seguridad, la respuesta no revela si el email existe o no.

---

#### 8) POST `/api/v1/auth/reset-password`
Cambia contraseña usando token de recuperación.

Body JSON esperado:
```json
{
	"email": "juan@mail.com",
	"resetToken": "token_reset",
	"newPassword": "NuevaPassword123"
}
```

Respuesta exitosa:
```json
{
	"success": true,
	"message": "Contraseña actualizada exitosamente",
	"data": {
		"email": "juan@mail.com",
		"reset": true
	}
}
```

---

## 7) ¿Cómo probarlo rápido? (flujo recomendado)

1. Abrir Swagger.
2. Ejecutar `POST /api/v1/auth/register`.
3. Revisar correo y obtener token de verificación.
4. Ejecutar `POST /api/v1/auth/verify-email`.
5. Ejecutar `POST /api/v1/auth/login` y copiar `token`.
6. En Swagger, botón **Authorize** y pegar: `Bearer <token>`.
7. Ejecutar `GET /api/v1/auth/profile`.
8. Probar recuperación:
	- `POST /forgot-password`
	- `POST /reset-password`

---

## 8) Formato de error esperado

Cuando ocurre error de negocio o autenticación, el middleware global responde con:

```json
{
	"success": false,
	"message": "Mensaje de error",
	"errorCode": "EMAIL_ALREADY_EXISTS",
	"traceId": "00-...",
	"timestamp": "2026-03-17T12:00:00Z"
}
```

Posibles códigos de error:
- `EMAIL_ALREADY_EXISTS`
- `USERNAME_ALREADY_EXISTS`
- `INVALID_FILE_FORMAT`
- `IMAGE_UPLOAD_FAILED`
- `FILE_TOO_LARGE`

También puede retornar `401` (credenciales inválidas), `404`, `409`, `429` (rate limit) o `500`.

---

## 9) Qué debe esperar el usuario final

- Registro exitoso, pero cuenta inactiva hasta verificar email.
- Login retorna JWT solo si la cuenta está activa y credenciales son válidas.
- Operaciones sensibles tienen límite de intentos por minuto.
- Mensajes de error consistentes y en formato JSON.
- Perfil accesible únicamente con token válido.

---

## 10) Ejemplos rápidos para Postman

### Login (JSON)
```json
{
	"emailOrUsername": "usuario@mail.com",
	"password": "Password123"
}
```

### Forgot password (JSON)
```json
{
	"email": "usuario@mail.com"
}
```

### Reset password (JSON)
```json
{
	"email": "usuario@mail.com",
	"resetToken": "token_recibido_en_correo",
	"newPassword": "NuevaPassword123"
}
```

### Register (form-data)
Campos:
- `name`: Juan
- `surname`: Pérez
- `username`: juanp
- `email`: juan@mail.com
- `password`: Password123
- `phone`: 12345678
- `profilePicture`: (archivo opcional)