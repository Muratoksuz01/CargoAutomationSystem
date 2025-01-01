using CargoAutomationSystem.Data;
using CargoAutomationSystem.Entity;
using System;
using System.Collections.Generic;

public static class DataSeeding
{
    // Kargoları tanımlıyoruz
    public static List<Cargo> Cargos = new List<Cargo>
    {
        new Cargo()
        {
            CargoId = 1,
            SenderId = 1,
            CurrentBranchId = 1,
            RecipientName = "user2",
            RecipientAddress = "789 Maple St",
            RecipientPhone = "2222222222",
            HashCode = "ABC123",
            Status = "Taşımada"
        },
        new Cargo()
        {
            CargoId = 2,
            SenderId = 2,
            CurrentBranchId = 2,
            RecipientName = "user1",
            RecipientAddress = "321 Oak St",
            RecipientPhone = "1111111111",
            HashCode = "XYZfew789",
            Status = "Teslim Edildi"
        },
        new Cargo()
        {
            CargoId = 3,
            SenderId = 2,
            CurrentBranchId = 1,
            RecipientName = "user3",
            RecipientAddress = "321 Oak St",
            RecipientPhone = "3333333333",
            HashCode = "XYZ7vs89",
            Status = "Teslim Edildi"
        },
        new Cargo()
        {
            CargoId = 4,
            SenderId = 1,
            CurrentBranchId = 2,
            RecipientName = "user3",
            RecipientAddress = "321 Oak St",
            RecipientPhone = "3333333333",
            HashCode = "XYZ78vs9",
            Status = "Teslim Edildi"
        }
    };

    // Kullanıcıları tanımlıyoruz
    public static List<User> Users = new List<User>
    {
        new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            Password = "hashedpassword4",
            Address = "789 West St, Westside",
            Phone = "1111111111",
            ImageUrl = "john_doe.png",
            IsTemporary=false
        },
        new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            Password = "hashedpassword4",
            Address = "101 South St, Southside",
            Phone = "2222222222",
            ImageUrl = "jane_smith.png",
            IsTemporary=false
        },
        new User
        {
            UserId = 3,
            Username = "user3",
            Email = "user3@example.com",
            Password = "hashedpassword4",
            Address = "123 South St, Southside",
            Phone = "3333333333",
            ImageUrl = "jane_smith.png",
            IsTemporary=false
        }
    };

    // Şubeleri tanımlıyoruz
    public static List<Branch> Branches = new List<Branch>
    {
        new Branch
        {
            BranchId = 1,
            BranchName = "Central Branch",
            Email = "branch1@cargo.com",
            Address = "123 Main St, City Center",
            Password = "hashedpassword1"
        },
        new Branch
        {
            BranchId = 2,
            BranchName = "East Branch",
            Email = "branch2@cargo.com",
            Address = "456 East Rd, East Town",
            Password = "hashedpassword1"
        }
    };

    // Kargo süreçlerini tanımlıyoruz (optional, sadece örnek olması için)
    public static List<CargoProcess> CargoProcesses = new List<CargoProcess>
    {
        new CargoProcess
        {
            CargoProcessId = 1,
            CargoId = 1,
            Process = "kargo kabul edildi",
            ProcessDate = DateTime.Now.AddHours(-12)
        },
        new CargoProcess
        {
            CargoProcessId = 2,
            CargoId = 2,
            Process = "kargo kabul edildi",
            ProcessDate = DateTime.Now.AddDays(-2).AddHours(-3)
        }
    };

    // Seed Data'larını ekleyerek ilişkileri oluşturuyoruz
    public static void Seed(CargoDbContext context)
    {
         if (!context.Cargos.Any())
        {
        // Kullanıcıları ekleyelim
        foreach (var user in Users)
        {
            context.Users.Add(user);
        }
        }
 if (!context.Branches.Any())
        {
        // Şubeleri ekleyelim
        foreach (var branch in Branches)
        {
            context.Branches.Add(branch);
        }
        }
        if (!context.Cargos.Any())
        {
            // Kargoları ekleyelim
            foreach (var cargo in Cargos)
            {
                context.Cargos.Add(cargo);
            }
        }

        // Kargo süreçlerini ekleyelim
          if (!context.CargoProcesses.Any())
        {
        foreach (var cargoProcess in CargoProcesses)
        {
            context.CargoProcesses.Add(cargoProcess);
        }
        }

        // UserCargo ilişkilerini ekleyelim
         if (!context.UserCargos.Any())
        {
        context.UserCargos.AddRange(new UserCargo
        {
            UserId = 1,
            CargoId = 1
        }, new UserCargo
        {
            Id=1,
            UserId = 1,
            CargoId = 2
        }, new UserCargo
        {
            Id=2,
            UserId = 1,
            CargoId = 4
        }, new UserCargo
        {
            Id=3,
            UserId = 2,
            CargoId = 1
        }, new UserCargo
        {
            Id=4,
            UserId = 2,
            CargoId = 2
        }, new UserCargo
        {
            Id=5,
            UserId = 2,
            CargoId = 3
        }, new UserCargo
        {
            Id=6,
            UserId = 3,
            CargoId = 3
        }, new UserCargo
        {
            Id=7,
            UserId = 3,
            CargoId = 4
        });
        }
        
         if (!context.BranchCargos.Any())
        {
        // BranchCargo ilişkilerini ekleyelim
        context.BranchCargos.AddRange(
         new BranchCargo
        {
            Id=1,
            BranchId = 1,
            CargoId = 3
        }, new BranchCargo
        {
            Id=2,
            BranchId = 2,
            CargoId = 2
        }, new BranchCargo
        {
            Id=3,
            BranchId = 2,
            CargoId = 4
        },new BranchCargo
        {
            Id=4,
            BranchId = 1,
            CargoId = 1
        }
        );
        }
        // Veritabanına kaydedelim
        context.SaveChanges();
    }
}
