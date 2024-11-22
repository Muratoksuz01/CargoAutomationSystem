using CargoAutomationSystem.Entity;

public static class DataSeeding
{
    public static List<User> Users = new List<User>
    {
        new User
        {
            UserId = 1,
            Username = "user1",
            Email = "user1@example.com",
            Password = "hashedpassword4",
            Address = "789 West St, Westside",
            Phone = "11111111111",
            ImageUrl = "john_doe.png"
        },
        new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            Password = "hashedpassword4",
            Address = "101 South St, Southside",
            Phone = "22222222222",
            ImageUrl = "jane_smith.png"
        },  new User
        {
            UserId = 3,
            Username = "user3",
            Email = "user3@example.com",
            Password = "hashedpassword4",
            Address = "101 South St, Southside",
            Phone = "33333333333",
            ImageUrl = "jane_smith.png"
        }
    };

    public static List<Branch> Branches = new List<Branch>
    {
        new Branch
        {
            BranchId = 1,
            Email = "branch1@cargo.com",
            Password = "hashedpassword1",
            BranchName = "Central Branch",
            Address = "123 Main St, City Center"
        },
        new Branch
        {
            BranchId = 2,
            Email = "branch2@cargo.com",
            Password = "hashedpassword1",
            BranchName = "East Branch",
            Address = "456 East Rd, East Town"
        }
    };

    public static List<Cargo> Cargos { get; } = new List<Cargo>
    {
        new Cargo()
        {
            CargoId=1,
            SenderId = 1,
            CurrentBranchId = 1,
            RecipientName = "user2",
            RecipientAddress = "789 Maple St",
            RecipientPhone = "22222222222",
            HashCode = "ABC123",
            Status = "Taşımada"
        },
           new Cargo()
        {
            CargoId=3,
            SenderId = 2,
            CurrentBranchId = 1,
            RecipientName = "user3",
            RecipientAddress = "321 Oak St",
            RecipientPhone = "33333333333",
            HashCode = "XYZ7vs89",
            Status = "Tamamlandı"
        },
        new Cargo()
        {
            CargoId=2,
            SenderId = 2,
            CurrentBranchId = 2,
            RecipientName = "user1",
            RecipientAddress = "321 Oak St",
            RecipientPhone = "11111111111",
            HashCode = "XYZfew789",
            Status = "Tamamlandı"
        },

            new Cargo()
        {
            CargoId=4,
            SenderId = 1,
            CurrentBranchId = 2,
            RecipientName = "user3",
            RecipientAddress = "321 Oak St",
            RecipientPhone = "33333333333",
            HashCode = "XYZ78vs9",
            Status = "Tamamlandı"
        }
    };

 

    public static List<CargoProcess> CargoProcesses = new List<CargoProcess>
    {
        new CargoProcess
        {
            CargoProcessId = 1,
            CargoId = 1,
            Process = "Şubeden Gönderildi",
            ProcessDate = DateTime.Now.AddHours(-12)
        },
        new CargoProcess
        {
            CargoProcessId = 2,
            CargoId = 1,
            Process = "Alıcıya Ulaştı",
            ProcessDate = DateTime.Now.AddHours(-1)
        },
        new CargoProcess
        {
            CargoProcessId = 3,
            CargoId = 2,
            Process = "Şubeden Gönderildi",
            ProcessDate = DateTime.Now.AddDays(-2).AddHours(-3)
        }
    };
}
