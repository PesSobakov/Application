# Application
# To run application using docker
execute in powershell
```
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\aspnetapp.pfx"  -p password
dotnet dev-certs https --trust
```
maybe you need to create folder "USERPROFILE\\.aspnet\https\" before that

set environmental variable GROQ_API_KEY in docker-compose.yml

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
GROQ_API_KEY: "this is a secret that you shouldn't share"
```
frontend
```
API_URL: localhost:7018
```
# Screenshots
Coworking list
![orange-stone-035c0c70f 6 azurestaticapps net_coworking-list](https://github.com/user-attachments/assets/88884ad3-f638-43ac-80e7-4dbaeed7f3d0)


Coworking details
![orange-stone-035c0c70f 6 azurestaticapps net_coworking-list (2)](https://github.com/user-attachments/assets/1bd5407f-fe88-4575-a5d6-1ec3f9ab4bbb)


My bookings
![orange-stone-035c0c70f 6 azurestaticapps net_coworking-list (1)](https://github.com/user-attachments/assets/72cad30e-b43b-4ce4-9f8d-81395bd0db1a)


Booking form
![orange-stone-035c0c70f 6 azurestaticapps net_coworking-list (4)](https://github.com/user-attachments/assets/918631ae-5bc5-444d-8d89-f252cb260081)
