﻿using PhimMoi.Models.User;

namespace PhimMoi.Areas.Admin.Models
{
    public class EditUserModel
    {
        public List<string> RoleList { get; set; }
        public string UserRole { get; set; }
        public bool IsLock { get; set; }
        public UserViewModel User { get; set; }
    }
}