# Application
# To run application using docker
execute in powershell
```
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\aspnetapp.pfx"  -p password
dotnet dev-certs https --trust
```
maybe you need to create folder "USERPROFILE\\.aspnet\https\" before that

after that run in command line
```
docker-compose up -d --build
```
Before using application you need initialise and seed database

visit site https://127.0.0.1:7018/api/Auth/seed 

or use https://127.0.0.1:7018/swagger
and run request api/Auth/seed

frontend will be available at http://localhost:8080
# Environment variables
backend
```
DATABASE_CONNECTION_STRING: "Server=postgres_db:5432;Database=coworking;User Id=root;Password=password;TrustServerCertificate=True;"
```
frontend
```
API_URL: localhost:7018
```
frontend env variable not working for now so you need to change file environment.ts
```
export const environment = {
  server: 'localhost:7018'
};
```
