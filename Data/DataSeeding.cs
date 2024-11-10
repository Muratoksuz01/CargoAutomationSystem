using System.Collections.Generic;
using CargoAutomationSystem.Entity;

namespace CargoAutomationSystem.Data
{
    public static class DataSeeding
    {
        public static List<Branch> Branches = new List<Branch>
        {
            new Branch { BranchId = 1, Email = "branch1@example.com", Password = "password1", BranchName = "Ankara Branch", Address = "Ankara" },
            new Branch { BranchId = 2, Email = "branch2@example.com", Password = "password2", BranchName = "Istanbul Branch", Address = "Istanbul" }
        };

        public static List<User> Users = new List<User>
        {
            new User { UserId = 1, UserName = "user1", Email = "user1@example.com", Password = "password1", Phone = "1234567890", Address = "Ankara", ImageUrl = "image1.jpg" },
            new User { UserId = 2, UserName = "user2", Email = "user2@example.com", Password = "password2", Phone = "0987654321", Address = "Istanbul", ImageUrl = "image2.jpg" }
        };

        public static List<Cargo> Cargos = new List<Cargo>
        {
            new Cargo { CargoId = 1, SenderEmail = "sender1@example.com", SenderAddress = "Ankara", SenderUserName = "Ali Yılmaz", SenderPhone = "1234567890", ReceiverAddress = "Istanbul", ReceiverEmail = "receiver1@example.com", ReceiverUserName = "Veli Kaya", ReceiverPhone = "0987654321" },
            new Cargo { CargoId = 2, SenderEmail = "sender2@example.com", SenderAddress = "Izmir", SenderUserName = "Merve Çetin", SenderPhone = "2345678901", ReceiverAddress = "Bursa", ReceiverEmail = "receiver2@example.com", ReceiverUserName = "Ayşe Şimşek", ReceiverPhone = "3456789012" }
        };

        public static List<CargoInfo> CargoInfos = new List<CargoInfo>
        {
            new CargoInfo { CargoInfoId = 1, CargoId = 1, Status = "Taşımada", Date = Convert.ToDateTime("2024-11-09") },
            new CargoInfo { CargoInfoId = 2, CargoId = 2, Status = "Tamamlandı", Date = Convert.ToDateTime("2024-11-08") }
        };

        public static List<CargoProcess> CargoProcesses = new List<CargoProcess>
        {
            new CargoProcess { CargoProcessId = 1, CargoInfoId = 1, Process = "Ankara Şubesi çıkış yaptı" },
            new CargoProcess { CargoProcessId = 2, CargoInfoId = 2, Process = "Bursa Şubesi teslim aldı" }
        };
    }
}
