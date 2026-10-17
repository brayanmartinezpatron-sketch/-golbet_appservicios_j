using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GolBet.Entities;

public class AppUser : IdentityUser
{
    [MaxLength(100)]
    public string FullName { get; set; } = null!;

    public decimal Balance { get; set; }

    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}