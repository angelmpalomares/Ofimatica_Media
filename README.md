# Ofimática Media

Proyecto de gestión de recursos multimedia (libros, películas, música) y usuarios, desarrollado con **ASP.NET Core MVC**, **Entity Framework Core** y **MySQL**.

---

## Requisitos previos

Antes de comenzar, asegúrate de tener instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [MySQL](https://dev.mysql.com/downloads/) (puedes usar [XAMPP](https://www.apachefriends.org/es/index.html) o MySQL Workbench / HeidiSQL)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (con workload de ASP.NET y desarrollo de escritorio)

---

## Configuración de la base de datos

- 1. Edita el archivo `appsettings.json` en el proyecto **API** y ajusta tu conexión MySQL:

   ```sh json
   "ConnectionStrings": {
     "DefaultConnection": "server=localhost;port=3306;user=root;password=tu_password;database=ofimatica_media"
   }

- 2. Abre la consola en el proyecto Infrastructure y ejecuta el comando 
   ```sh dotnet ef database update```

## Ejecución del proyecto
- 1. Abre la solución en Visual Studio
- 2. Asegúrate de que API es el proyecto de inicio.
- 3. Ejecuta con IIS Express (Se abrirá en https://localhost:44351)

## Credenciales iniciales
Se han añadido dos usuarios base para poder realizar las funciones de usuario y de admin
| Usuario  | Rol   | Contraseña  |
|----------|-------|-------------|
| admin    | Admin | pass	 |
| user	   | User  | pass        |