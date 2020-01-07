using System;
using System.Collections.Generic;
using System.Text;

namespace PSMapa.BindingModel
{
    public class RegisterUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public RegisterUser()
        {
            UserName = "";
            Password = "";
            ConfirmPassword = "";
        }
    }
}
