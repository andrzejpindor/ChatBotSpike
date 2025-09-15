# Application Setup

This repository contains both the frontend and backend of the application. There are two ways to run the application: fully automated using Docker Compose, or manually.

---

## Option 1: Full Automation (Docker Compose)

1. Navigate to the `src` folder.
2. Run the startup script:

```bash
./run.sh
```

3. Once started, the application will be available at:

* **Frontend:** [http://localhost:8082](http://localhost:8082)
* **Backend:** [http://localhost:8080](http://localhost:8080)

> 🚨 Note: The backend database will start **without any mounted volumes**, so data will not persist between restarts.

---

## Option 2: Manual Setup

### Frontend

1. Navigate to the frontend folder.
2. Install dependencies:

```bash
npm install
```

3. Start the frontend server:

```bash
npm run start
```

### Backend

1. Open the backend project `ChatBot.Api` in your preferred IDE (e.g., Visual Studio).
2. Run the project normally through the IDE.

---

Both methods allow you to run the frontend and backend locally. Choose the method that best suits your workflow.
