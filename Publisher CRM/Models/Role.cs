using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Publisher_CRM.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string Name { get; set; } = string.Empty;

        // Self-referencing property to establish who reports to whom
        public int? ParentRoleId { get; set; }
        public virtual Role? ParentRole { get; set; }
        public virtual ICollection<Role> SubordinateRoles { get; set; } = new List<Role>();

        public virtual ICollection<Users> Users { get; set; } = new List<Users>();


        // New fields added from the UI requirements
        public bool ShareDataWithPeers { get; set; }
        public string? Description { get; set; }
    }

    // This is the missing class added right here!
    public class RoleHierarchyNode
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<RoleHierarchyNode> Children { get; set; } = new List<RoleHierarchyNode>();
    }
}