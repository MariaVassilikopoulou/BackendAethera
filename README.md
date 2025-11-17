# Aethera – Perfume Webshop API (ASP.NET Core 8 + Azure)

Aethera is a modern webshop backend built with **ASP.NET Core 8**, **Azure Cosmos DB**, **Azure Key Vault**, and **Azure Entra ID (External ID)**.  
It handles typical webshop features such as browsing products, managing a cart, and placing orders – all secured by JWT authentication.

This repository contains the backend API.  
The frontend (Next.js) connects to this API with CORS enabled for both local and production environments.

## 🚀 Features

### **1. Secure & Cloud-Ready**
- Azure Key Vault for secrets  
- Azure Cosmos DB as NoSQL database  
- Azure Entra External ID for user authentication  
- JWT access token validation  
- GitHub Actions CI/CD → Azure App Service  

### **2. Webshop Functionality**
- Product catalogue (Cosmos DB container: `perfumes`)  
- Shopping cart per user  

### **3. Developer Friendly**
- AutoMapper DTO → Model mappings  
- Custom Cosmos DB JSON serializer  
- Clean dependency injection setup  
- Swagger + JWT support  
- CORS configured for local dev + Vercel frontend

## 🔐 Authentication (Azure Entra External ID)
The API uses **JWT Bearer tokens**:
- `Authority`  
- `ClientId`  
- `Audience`  
- `SwaggerScope`

These are taken from **Azure Key Vault**, not from `appsettings.json`.

## 🔄 CI/CD Pipeline (GitHub Actions → Azure App Service)
- Restore dependencies
- Build + publish .NET app
- Login to Azure via service principal
- Deploy to Azure App Service

## 🛠 Technologies Used

- ASP.NET Core 8
- Azure Cosmos DB
- Azure Key Vault
- Azure Entra External ID
- AutoMapper
- Swagger
- GitHub Actions
- Vercel (Frontend)
- JWT Authentication
- Generic Repository Pattern

## [📘 What I Focused On and Learned]
Built a cloud-ready ASP.NET Core webshop backend. 
Focused on JWT auth with Azure Entra, Cosmos DB with a repository pattern, Key Vault secrets, AutoMapper, Swagger, and CI/CD to Azure.
Learned secure Azure connections, clean dependency injection, scalable API structure, and proper error handling.

