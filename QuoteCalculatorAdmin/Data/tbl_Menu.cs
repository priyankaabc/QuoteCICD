//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuoteCalculatorAdmin.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Menu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_Menu()
        {
            this.tbl_RoleMenuMap = new HashSet<tbl_RoleMenuMap>();
        }
    
        public short MenuId { get; set; }
        public Nullable<short> ParentMenuId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_RoleMenuMap> tbl_RoleMenuMap { get; set; }
    }
}
