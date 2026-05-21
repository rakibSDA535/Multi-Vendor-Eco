using Shop111.Constants;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Shop111.Data
{
    //public class DbSeeder
    //{

    //public static async Task SeedDefaultData(IServiceProvider service)
    //{
    //    try
    //    {
    //        var context = service.GetService<ApplicationDbContext>();

    //        // this block will check if there are any pending migrations and apply them
    //        if ((await context.Database.GetPendingMigrationsAsync()).Count() > 0)
    //        {
    //            await context.Database.MigrateAsync();
    //        }

    //        //====================================add admin and users//==========================================
    //        var userMgr = service.GetService<UserManager<IdentityUser>>();
    //        var roleMgr = service.GetService<RoleManager<IdentityRole>>();
    //        // create admin role if not exists
    //        var adminRoleExists = await roleMgr.RoleExistsAsync(Roles.Admin.ToString());
    //        if (!adminRoleExists)
    //        {
    //            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
    //        }
    //        // create user role if not exists
    //        var userRoleExists = await roleMgr.RoleExistsAsync(Roles.User.ToString());

    //        if (!userRoleExists)
    //        {
    //            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
    //        }
    //        // create manager role if not exists
    //        var managerRoleExists = await roleMgr.RoleExistsAsync(Roles.Manager.ToString());
    //        if (!managerRoleExists)
    //        {
    //            await roleMgr.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
    //        }
    //        // adding some roles to db
    //        await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
    //        await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
    //        await roleMgr.CreateAsync(new IdentityRole(Roles.Manager.ToString()));

    //        // create admin user
    //        var admin = new IdentityUser
    //        {
    //            UserName = "admin@gmail.com",
    //            Email = "admin@gmail.com",
    //            EmailConfirmed = true
    //        };

    //        var userInDb = await userMgr.FindByEmailAsync(admin.Email);
    //        if (userInDb is null)
    //        {
    //            await userMgr.CreateAsync(admin, "Admin@123");
    //            await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
    //        }
    //        //=====================================================================================================

    //        if (!context.Genres.Any())
    //        {
    //            await SeedGenreAsync(context);
    //        }

    //        if (!context.Products.Any())
    //        {
    //            await SeedProductsAsync(context);
    //            // update stock table
    //            await context.Database.ExecuteSqlRawAsync(@"
    //             INSERT INTO Stock(ProductId,Quantity) 
    //             SELECT 
    //             b.Id,
    //             10 
    //             FROM Product b
    //             WHERE NOT EXISTS (
    //             SELECT * FROM [Stock]
    //             );
    //            ");
    //        }


    //    }

    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }


    //}
   
        public class DbSeeder
        {
            public static async Task SeedDefaultData(IServiceProvider service)
            {
                try
                {
                    var context = service.GetService<ApplicationDbContext>();

                    // ১. পেন্ডিং মাইগ্রেশন চেক এবং অ্যাপ্লাই
                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        await context.Database.MigrateAsync();
                    }

                    // ২. সার্ভিসগুলো ApplicationUser দিয়ে গেট করা
                    var userMgr = service.GetService<UserManager<ApplicationUser>>();
                    var roleMgr = service.GetService<RoleManager<IdentityRole>>();

                    // ৩. রোলগুলো তৈরি করা (Admin, Manager, User)
                    string[] roleNames = { Roles.Admin.ToString(), Roles.Manager.ToString(), Roles.User.ToString() };

                    foreach (var roleName in roleNames)
                    {
                        if (!await roleMgr.RoleExistsAsync(roleName))
                        {
                            await roleMgr.CreateAsync(new IdentityRole(roleName));
                        }
                    }

                    // ৪. অ্যাডমিন ইউজার তৈরি করা
                    var adminEmail = "admin@gmail.com";
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        IsApproved = true // অ্যাডমিন সরাসরি অ্যাপ্রুভড থাকবে
                    };

                    var userInDb = await userMgr.FindByEmailAsync(adminEmail);
                    if (userInDb == null)
                    {
                        var result = await userMgr.CreateAsync(adminUser, "Admin@123");
                        if (result.Succeeded)
                        {
                            await userMgr.AddToRoleAsync(adminUser, Roles.Admin.ToString());
                        }
                    }

                    // ৫. জেনারে এবং প্রোডাক্ট সিডিং (যদি থাকে)
                    if (!context.Genres.Any())
                    {
                        await SeedGenreAsync(context);
                    }

                    if (!context.Products.Any())
                    {
                        await SeedProductsAsync(context);

                        // স্টক আপডেট করার SQL লজিক
                        await context.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO Stocks(ProductId, Quantity) 
                        SELECT Id, 10 FROM Products p
                        WHERE NOT EXISTS (SELECT 1 FROM Stocks s WHERE s.ProductId = p.Id);
                    ");
                    }
                if (!context.Locations.Any())
                {
                    context.Locations.AddRange(
                        new Location
                        {
                            City = "Dhaka",
                            Area = "Mirpur"
                        },
                        new Location
                        {
                            City = "Dhaka",
                            Area = "Uttara"
                        },
                        new Location
                        {
                            City = "Khulna",
                            Area = "Sonadanga"
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }
                catch (Exception ex)
                {
                    Console.WriteLine("Seeding Error: " + ex.Message);
                }

            }




            #region private methods
            private static async Task SeedGenreAsync(ApplicationDbContext context)
            {
                var genres = new[]
                 {
            new Genre { GenreName = "Book" },
            new Genre { GenreName = "Shirt" },
            new Genre { GenreName = "Pant" },
            new Genre { GenreName = "Blager" },
            new Genre { GenreName = "Shoe" },
            new Genre { GenreName = "Panjabi" },
            new Genre { GenreName = "SmartPhone" },
            new Genre { GenreName = "Tv" },
            new Genre { GenreName = "Laptop" },
            new Genre { GenreName = "Furniture" },
            new Genre { GenreName = "Laxarious sofa" },
            new Genre { GenreName = "Bick" },
            new Genre { GenreName = "Car" },
            new Genre { GenreName = "Packet" },
            new Genre { GenreName = "Jar" },
            new Genre { GenreName = "sack" },
            new Genre { GenreName = "Bottle" },
            new Genre { GenreName = "Electronic Goods" },

        };

                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }

            private static async Task SeedProductsAsync(ApplicationDbContext context)
            {
                var products = new List<Product>
        {
            // Book (GenreId = 1)
            new Product { ProductName = "Pride and Prejudice", CompanyName = "Jane Austen", Price = 12, GenreId = 1 },
            new Product { ProductName = "The Notebook", CompanyName = "Nicholas Sparks", Price = 11, GenreId = 1 },
            new Product { ProductName = "Outlander", CompanyName = "Diana Gabaldon", Price = 14, GenreId = 1 },
            new Product { ProductName = "Me Before You", CompanyName = "Jojo Moyes", Price = 10, GenreId = 1 },
            new Product { ProductName = "The Fault in Our Stars", CompanyName = "John Green", Price = 9, GenreId = 1 },
            
            // Shirt (GenreId = 2)
            new Product { ProductName = "The Bourne Identity", CompanyName = "Robert Ludlum", Price = 14.99, GenreId = 2 },
            new Product { ProductName = "Die Hard", CompanyName = "Roderick Thorp", Price = 13.99, GenreId = 2 },
            new Product { ProductName = "Jurassic Park", CompanyName = "Michael Crichton", Price = 15.99, GenreId = 2 },
            new Product { ProductName = "The Da Vinci Code", CompanyName = "Dan Brown", Price = 12.99, GenreId = 2 },
            new Product { ProductName = "The Hunger Games", CompanyName = "Suzanne Collins", Price = 11.99, GenreId = 2 },
            
            // Pant (GenreId = 3)
            new Product { ProductName = "Gone Girl", CompanyName = "Gillian Flynn", Price = 11.99, GenreId = 3 },
            new Product { ProductName = "The Girl with the Dragon Tattoo", CompanyName = "Stieg Larsson", Price = 10.99, GenreId = 3 },
            new Product { ProductName = "The Silence of the Lambs", CompanyName = "Thomas Harris", Price = 12.99, GenreId = 3 },
            new Product { ProductName = "Before I Go to Sleep", CompanyName = "S.J. Watson", Price = 9.99, GenreId = 3 },
            new Product { ProductName = "The Girl on the Train", CompanyName = "Paula Hawkins", Price = 13.99, GenreId = 3 },
            
            // Crime Books (GenreId = 4)
            new Product { ProductName = "The Godfather", CompanyName = "Mario Puzo", Price = 13.99, GenreId = 4 },
            new Product { ProductName = "The Girl with the Dragon Tattoo", CompanyName = "Stieg Larsson", Price = 12.99, GenreId = 4 },
            new Product { ProductName = "The Cuckoo's Calling", CompanyName = "Robert Galbraith", Price = 14.99, GenreId = 4 },
            new Product { ProductName = "In Cold Blood", CompanyName = "Truman Capote", Price = 11.99, GenreId = 4 },
            new Product { ProductName = "The Silence of the Lambs", CompanyName = "Thomas Harris", Price = 15.99, GenreId = 4 },
            
            // SelfHelp Books (GenreId = 5)
            new Product { ProductName = "The 7 Habits of Highly Effective People", CompanyName = "Stephen R. Covey", Price = 9.99, GenreId = 5 },
            new Product { ProductName = "How to Win Friends and Influence People", CompanyName = "Dale Carnegie", Price = 8.99, GenreId = 5 },
            new Product { ProductName = "Atomic Habits", CompanyName = "James Clear", Price = 10.99, GenreId = 5 },
            new Product { ProductName = "The Subtle Art of Not Giving a F*ck", CompanyName = "Mark Manson", Price = 7.99, GenreId = 5 },
            new Product { ProductName = "You Are a Badass", CompanyName = "Jen Sincero", Price = 11.99, GenreId = 5 },
            
            // Programming Books (GenreId = 6)
            new Product { ProductName = "Clean Code", CompanyName = "Robert C. Martin", Price = 19.99, GenreId = 6 },
            new Product { ProductName = "Design Patterns", CompanyName = "Erich Gamma", Price = 17.99, GenreId = 6 },
            new Product { ProductName = "Code Complete", CompanyName = "Steve McConnell", Price = 21.99, GenreId = 6 },
            new Product { ProductName = "The Pragmatic Programmer", CompanyName = "Andrew Hunt", Price = 18.99, GenreId = 6 },
            new Product { ProductName = "Head First Design Patterns", CompanyName = "Eric Freeman", Price = 20.99, GenreId = 6 }
        };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            #endregion
        }


    
}



