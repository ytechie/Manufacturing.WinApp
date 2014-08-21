using System.Collections.Generic;
using System.Linq;

namespace Manufacturing.WinApp.Common
{
    public class User
    {
        private static User _user;

        public User()
        {
            Roles = new List<string>();
        }

        public static User Current
        {
            get { return _user ?? (_user = new User()); }
        }

        public List<string> Roles { get; set; } 

        public string Name { get; set; }

        public bool IsInRole(string role)
        {
            var matchingRole = this.Roles.FirstOrDefault(currentRole => currentRole == role);
            return matchingRole != null;
        }
    }
}