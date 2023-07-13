using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace coba.Models.Request;

public class ContactRequest
{
    [FromForm(Name = "name")]
    [Required(ErrorMessage = "Nama Tidak Boleh Kosong")]
    public string Name { get; set; } = null!;
    [FromForm(Name = "email")]
    [Required(ErrorMessage = "Email Tidak Boleh Kosong")]
    public string Email { get; set; } = null!;
    [FromForm(Name = "message")]
    public string Message { get; set; } = null!;
}