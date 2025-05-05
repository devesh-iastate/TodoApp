
# 📝 Dynamic To-Do Application

A clean, functional, and flexible To-Do application that demonstrates essential design patterns and full-stack engineering using Vue.js and .NET Core.

---

## 🚀 Features

- ✅ **Pick Your Provider (Factory Pattern)**: Choose between Local DB and Firebase in the UI.
- 🔍 **Smart Searching (Debounced)**: Efficient, server-side debounced search.
- ➕ **Add To-Dos**: Create new tasks with title and description.
- ✏️ **Update To-Dos**: Edit titles or descriptions inline.
- 🗑️ **Delete To-Dos**: Remove completed or unneeded tasks.
- 📡 **Real-Time Polling**: Polls backend for latest updates every second.
- 🔐 **Firebase Auth Middleware**: Secure backend endpoints using Firebase ID tokens.

---

## 🛠️ Tech Stack

### Frontend
- **Vue.js 3**
- Axios
- Lodash (for debounce)

### Backend
- **C#** with **.NET Core**
- **Entity Framework Core**
- SQLite (can be swapped with SQL Server, MySQL)
- Firebase Admin SDK for authentication

---

## ▶️ Getting Started

### 🔧 Prerequisites
- Node.js + npm
- .NET SDK 7 or 8
- Firebase Project (for authentication)

### 📦 Backend Setup

```bash
cd TodoApp
dotnet restore
dotnet ef database update
dotnet run
```

### 🌐 Frontend Setup

```bash
cd client-app
npm install
npm run serve
```

---

## 📌 Environment Variables

`.env` file (backend):
```
FIREBASE_PROJECT_ID=your-project-id
FIREBASE_CREDENTIALS_PATH=path/to/your/serviceAccountKey.json
```

`.env` file (frontend):
```
VUE_APP_API_BASE=http://localhost:5000/api
```

---

## 🧪 Testable URLs

- `GET /api/todos/:provider`
- `POST /api/todos/:provider`
- `PUT /api/todos/:provider/:id`
- `DELETE /api/todos/:provider/:id`
- `GET /api/todos/:provider/last-updated`

---

## 📄 License

MIT License © 2025
