using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sharing.Identity
{
    public class UnassignRoleModel
    {
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}

