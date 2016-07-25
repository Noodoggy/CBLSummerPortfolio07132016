namespace CBLSummerPortfolio07132016.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CBLSummerPortfolio07132016.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Mod"))
            {
                roleManager.Create(new IdentityRole { Name = "Mod" });
            }
            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "cbentonlandry@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "cbentonlandry@gmail.com",
                    Email = "cbentonlandry@gmail.com",
                    FirstName = "Benton",
                    LastName = "Landry",
                    DisplayName = "ChBeLa"
                }, "Kissme^3");
            }

            var userId = userManager.FindByEmail("cbentonlandry@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "moderator@coderfoundry.com",
                    Email = "moderator@coderfoundry.com",
                    FirstName = "Antonio",
                    LastName = "Raynor",
                    DisplayName = "ANIVRA"
                }, "CoderFoundry_2016");

                userId = userManager.FindByEmail("moderator@coderfoundry.com").Id;
                userManager.AddToRole(userId, "Mod");
            }
        }
    }
}
