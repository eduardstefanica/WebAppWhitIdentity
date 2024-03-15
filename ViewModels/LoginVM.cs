using System.ComponentModel.DataAnnotations;

namespace WebAppWithIdentity.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "please enter your user name")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "please enter your user name")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
