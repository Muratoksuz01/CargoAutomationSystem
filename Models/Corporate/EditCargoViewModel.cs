using System.Collections.Generic;
using CargoAutomationSystem.Entity;

namespace CargoAutomationSystem.Models.Corporate
{
    public class EditCargoViewModel
    {
        public string HashCode { get; set; }
        public string Status { get; set; }
        public int? NewBranchId { get; set; }
        public List<Branch> Branches { get; set; }
    }
}
