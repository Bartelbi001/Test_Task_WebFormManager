# WebFormManager

A lightweight web application for submitting, storing, listing, and searching form submissions with flexible, dynamic schema support.  
Designed as part of a full-stack test assignment.

## ✨ Overview

- **Frontend:** Vue 3 + TailwindCSS
- **Backend:** ASP.NET Core (Clean Architecture)
- **Storage:** In-Memory + FileStorage (Dev)
- **Form Flexibility:** Backend doesn't depend on hardcoded models — supports multiple form types.
- **Validation:** Full client- and server-side validation
- **Search/List UI:** All submissions (from different forms) are displayed on one searchable page.

---

## 🛠️ Tech Stack

### Frontend
- **Vue 3**
- **Vite** — build tool
- **Tailwind CSS** — styling
- **Vue Router** — routing
- **Axios** — HTTP client
- **vee-validate + yup** — form validation

### Backend
- **ASP.NET Core Web API**
- **Clean Code Architecture**
- **FluentValidation** — request validation
- **Serilog** — logging
- **In-Memory / File Storage** — no DB setup required

### Tooling
- **IDE:** JetBrains Rider / WebStorm
- **Tests:** xUnit (Unit + Integration)
- **Version Control:** Git + GitHub

---

## 📁 Project Structure (Backend)

```
WebFormManager/
│
├── WebFormManager.API/            # API layer (controllers, DI, middleware)
├── WebFormManager.Application/    # Business logic (services, DTOs, validators)
├── WebFormManager.Infrastructure/ # Storage (InMemory + FileStorage), logging, etc.
├── WebFormManager.Tests/          # Unit and integration tests
└── WebFormManager.sln             # Solution file
```

---

## 🚀 Setup & Run

### Prerequisites:
- [.NET 8 SDK](https://dotnet.microsoft.com/)
- [Node.js + npm](https://nodejs.org/)
- IDE: Rider / VS Code / WebStorm (optional)

### Backend (API)
```bash
cd WebFormManager
dotnet restore
dotnet run --project WebFormManager.API
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```

---

## 📸 Screenshots

### 🏠 About Page
The "About" stub page that demonstrates navigation.
![About Page](./screenshots/About_page.png)

### 📝 Successfully Created Form
Example of a completed form and successful data submission.
![Form Submitted](./screenshots/Successfully_created_form.png)

### 👤 Added User (Form Input Example)
List and search for users.
![Added User](./screenshots/Added_user.png)

### ❌ Validation Errors
Verification of required fields and validation on the client.
![Validation Errors](./screenshots/Validation_errors.png)

### 🔍 JSON Payload (Backend Flexibility)
Response from the server with dynamically received data.
![JSON Payload](./screenshots/JSON_payload.png)

---

## 🧩 Part 2 – File Storage Strategy

To handle large file attachments (~100MB) with high volume (thousands of submissions):

### Storage
- **Azure Blob Storage** for scalable, reliable object storage.
- Metadata (file name, type, upload date) stored in a lightweight DB (e.g., PostgreSQL or SQLite).

### API
- **Async REST API** for upload/download with resumable support.
- **Traffic throttling**, streaming download, and secured access via SAS tokens.

### Additional Concepts
- Use of message queues (e.g., Azure Queue or RabbitMQ) for processing heavy uploads.
- CDN integration for optimized delivery.
- Optionally: virus scanning, file retention policies.

---

## 📄 Task

Full description of the test task is available in [TASK.md](./TASK.md)

---

## 👤 Author

**Ilya Shelkunov**  
Full-Stack Developer | ASP.NET Core • Vue • Clean Architecture  
[![Email](https://img.shields.io/badge/Email-D14836?style=flat&logo=gmail&logoColor=white)](mailto:Shelkunov.20014@gmail.com)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E0?style=flat&logo=telegram&logoColor=white)](https://t.me/Bartelbi001)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=flat&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/your_username)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/your_username)
