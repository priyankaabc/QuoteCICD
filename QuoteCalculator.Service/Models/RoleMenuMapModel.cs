namespace QuoteCalculator.Service.Models
{
    public class RoleMenuMapModel
    {
        public int RoleMenuMapId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public short MenuId { get; set; }
        public string MenuName { get; set; }
        public bool IsView { get; set; }
        public bool IsInsert { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsChangeStatus { get; set; }
    }
}
