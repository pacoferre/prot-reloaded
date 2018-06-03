using System;
using System.Collections.Generic;
using System.Text;

namespace PROTR.Core.Security.EF
{
    public class AppUser
    {
        public int IdAppUser { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Su { get; set; } = false;
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Deactivated { get; set; } = false;
    }
}
