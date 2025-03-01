using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactProject.Models;

public class t_User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_UserId { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100)]
    public string c_UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(100)]
    public string c_Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100)]
    public string c_Password { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Confirm password is Required!")]
    [Compare("c_Password", ErrorMessage = "Password does not match!")]
    public string c_ConfirmPassword { get; set; }


    [StringLength(500)]
    public string? c_Address { get; set; }


    [StringLength(50)]
    public string? c_Mobile { get; set; }

    [StringLength(10)]
    public string? c_Gender { get; set; }

    [StringLength(4000)]
    public string? c_Image { get; set; }

    public IFormFile? ProfilePicture { get; set; }

    
}