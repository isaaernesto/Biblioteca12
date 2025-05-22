using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Biblioteca.Models;

namespace Biblioteca.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Roles
            string[] roles = { "Aluno", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Usuário admin
            string adminEmail = "admin@admin.com";
            string adminPassword = "Admin@123";
            string nomeCompleto = "Administrador do Sistemas";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // Cria o registro na tabela Usuarios (caso use)
                    var usuario = new Usuario
                    {
                        NomeCompleto = nomeCompleto,
                        CPF = "000.000.000-00",
                        Celular = "00000000000",
                        DataNascimento = "01/01/2000",
                        AppUserId = Guid.Parse(adminUser.Id),
                        IdentityUser = adminUser
                    };
                    context.Usuarios.Add(usuario);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                // Garante que está na role Admin
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}