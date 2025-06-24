# RedisCacheImplementation

This project demonstrates how to implement a basic Redis-based cache service in ASP.NET Core using StackExchange.Redis.

---

## 🚀 Features

- Connect to Redis using `StackExchange.Redis`
- Generic cache service with `GetAsync`, `SetAsync`, and `RemoveAsync` methods
- Easy integration into any .NET Core application
- Configuration from `appsettings.json`

---

## 📦 Clone the Repository

```bash
git clone https://github.com/Martik2002/RedisCacheImplementation.git
cd RedisCacheImplementation

---

## 🐳 How to Run Redis using Docker

To use the Redis cache in this project, you need to have a Redis server running. The easiest way is to run it in a Docker container.

### 📥 1. Install Docker

If you haven't already, download and install Docker from:  
👉 https://www.docker.com/products/docker-desktop

---

### ▶️ 2. Run Redis with Docker

Run the following command in your terminal or PowerShell:

```bash
docker run -d --name redis-cache -p 6379:6379 redis


