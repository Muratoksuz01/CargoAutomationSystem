using CargoAutomationSystem.Entity;

public static class DataSeeding
{
    public static List<User> Users = new List<User>
    {
        new User
        {
            UserId = 1,
            Username = "john_doe",
            Email = "john@example.com",
            Password = "hashedpassword3",
            Address = "789 West St, Westside",
            Phone = "123-456-7890",
            ImageUrl = "/img/john_doe.png"
        },
        new User
        {
            UserId = 2,
            Username = "jane_smith",
            Email = "jane@example.com",
            Password = "hashedpassword4",
            Address = "101 South St, Southside",
            Phone = "987-654-3210",
            ImageUrl = "/img/jane_smith.png"
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
            Password = "hashedpassword2",
            BranchName = "East Branch",
            Address = "456 East Rd, East Town"
        }
    };

    public static List<Cargo> Cargos = new List<Cargo>
    {
        new Cargo
        {
            CargoId = 1,
            SenderId = 1,
            ReceiverId = 2,
            CurrentBranchId = 1,
            HashCode = "ABC123",
            Status = "Taşımada"
        },
        new Cargo
        {
            CargoId = 2,
            SenderId = 2,
            ReceiverId = 1,
            CurrentBranchId = 2,
            HashCode = "XYZ789",
            Status = "Tamamlandı"
        }
    };

    public static List<CargoInfo> CargoInfos = new List<CargoInfo>
    {
        new CargoInfo
        {
            CargoInfoId = 1,
            CargoId = 1,
            Status = "Taşımada",
            Date = DateTime.Now.AddDays(-1)
        },
        new CargoInfo
        {
            CargoInfoId = 2,
            CargoId = 2,
            Status = "Tamamlandı",
            Date = DateTime.Now.AddDays(-2)
        }
    };

    public static List<CargoProcess> CargoProcesses = new List<CargoProcess>
    {
        new CargoProcess
        {
            CargoProcessId = 1,
            CargoInfoId = 1,
            Process = "Şubeden Gönderildi",
            ProcessDate = DateTime.Now.AddHours(-12)
        },
        new CargoProcess
        {
            CargoProcessId = 2,
            CargoInfoId = 1,
            Process = "Alıcıya Ulaştı",
            ProcessDate = DateTime.Now.AddHours(-1)
        },
        new CargoProcess
        {
            CargoProcessId = 3,
            CargoInfoId = 2,
            Process = "Şubeden Gönderildi",
            ProcessDate = DateTime.Now.AddDays(-2).AddHours(-3)
        }
    };
}
