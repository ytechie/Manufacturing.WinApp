using System;

namespace Manufacturing.WinApp.Common
{
    public class RequiredRoleAttribute : Attribute
    {
        private readonly string _roleName;

        public RequiredRoleAttribute(string roleName)
        {
            _roleName = roleName;
        }

        public string RoleName
        {
            get
            {
                return _roleName;
            }
        }
    }
}
