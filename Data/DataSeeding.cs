using CargoAutomationSystem.Entity;

public static class DataSeeding
{
    // Cargos verisinin null olup olmadığını kontrol et
    public static List<Cargo> Cargos = new List<Cargo>
    {
        new Cargo()
        {
            CargoId = 1,
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
            CargoId = 2,
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
            CargoId = 3,
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
            CargoId = 4,
            SenderId = 1,
            CurrentBranchId = 2,
            RecipientName = "user3",
            RecipientAddress = "321 Oak St",
            RecipientPhone = "33333333333",
            HashCode = "XYZ78vs9",
            Status = "Tamamlandı"
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
            Phone = "11111111111",
            ImageUrl = "john_doe.png",
            Cargos = new List<Cargo> { Cargos[0], Cargos[1], Cargos[3] }  // Kargo ID 1, 2, 4
        },
        new User
        {
            UserId = 2,
            Username = "user2",
            Email = "user2@example.com",
            Password = "hashedpassword4",
            Address = "101 South St, Southside",
            Phone = "22222222222",
            ImageUrl = "jane_smith.png",
            Cargos = new List<Cargo> { Cargos[0], Cargos[1], Cargos[2] }  // Kargo ID 1, 2, 3
        },
        new User
        {
            UserId = 3,
            Username = "user3",
            Email = "user3@example.com",
            Password = "hashedpassword4",
            Address = "123 South St, Southside",
            Phone = "33333333333",
            ImageUrl = "jane_smith.png",
            Cargos = new List<Cargo> { Cargos[2], Cargos[3] }  // Kargo ID 3, 4
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
            Password="hashedpassword1",
            Cargos = new List<Cargo> { Cargos[0], Cargos[2] }
        },
        new Branch
        {
            BranchId = 2,
            BranchName = "East Branch",
            Email = "branch2@cargo.com",
            Address = "456 East Rd, East Town",
            Password="hashedpassword1",
            Cargos = new List<Cargo> { Cargos[1], Cargos[3] }
        }
    };

    // Kargo süreçlerini tanımlıyoruz (optional, sadece örnek olması için)
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
