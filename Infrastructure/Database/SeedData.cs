using BCrypt.Net;
using Infrastructure.Database;
using Infrastructure.Enums;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public static class SeedData
    {
        public static async Task InitializeAsync(InfraDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!context.Users.Any())
            {
                var admin = new UserModel
                {
                    Username = "admin",
                    Email = "admin@ofimatica.com",
                    Name = "Super",
                    Surname = "Admin",
                    Role = UserRole.Admin,
                    Password = BCrypt.Net.BCrypt.HashPassword("pass"),
                    IsActive = true
                };

                var user = new UserModel
                {
                    Username = "user",
                    Email = "user@ofimatica.com",
                    Name = "Demo",
                    Surname = "User",
                    Role = UserRole.User,
                    Password = BCrypt.Net.BCrypt.HashPassword("pass"),
                    IsActive = true
                };

                context.Users.AddRange(admin, user);
            }
            if (!context.Resources.Any())
            {
                var resources = new List<ResourceModel>
                {
                    new ResourceModel { Name = "Cien Años de Soledad", Author = "Gabriel García Márquez", Description = "Novela clásica del realismo mágico.", Publication = 1967, ResourceType = ResourceType.Libro },
                    new ResourceModel { Name = "Don Quijote de la Mancha", Author = "Miguel de Cervantes", Description = "Obra maestra de la literatura española.", Publication = 1605, ResourceType = ResourceType.Libro },
                    new ResourceModel { Name = "El Señor de los Anillos", Author = "J. R. R. Tolkien", Description = "Fantasía épica en la Tierra Media.", Publication = 1954, ResourceType = ResourceType.Libro },
                    new ResourceModel { Name = "El Padrino", Author = "Francis Ford Coppola", Description = "Clásico del cine de mafia.", Publication = 1972, ResourceType = ResourceType.Pelicula },
                    new ResourceModel { Name = "La Lista de Schindler", Author = "Steven Spielberg", Description = "Drama histórico sobre el Holocausto.", Publication = 1993, ResourceType = ResourceType.Pelicula },
                    new ResourceModel { Name = "Inception", Author = "Christopher Nolan", Description = "Ciencia ficción sobre sueños dentro de sueños.", Publication = 2010, ResourceType = ResourceType.Pelicula },
                    new ResourceModel { Name = "Thriller", Author = "Michael Jackson", Description = "Álbum más vendido de todos los tiempos.", Publication = 1982, ResourceType = ResourceType.Musica },
                    new ResourceModel { Name = "Bohemian Rhapsody", Author = "Queen", Description = "Canción icónica de rock progresivo.", Publication = 1975, ResourceType = ResourceType.Musica },
                    new ResourceModel { Name = "Imagine", Author = "John Lennon", Description = "Himno pacifista de los 70s.", Publication = 1971, ResourceType = ResourceType.Musica }
                };
                context.Resources.AddRange(resources);
            }
            await context.SaveChangesAsync();
        }
    }
}
