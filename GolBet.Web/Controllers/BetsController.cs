using GolBet.Entities;
using GolBet.Repositories.Data;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers;

[Authorize(Roles = DbSeeder.BettorRole)]
public class BetsController : Controller
{
    private readonly IBetService _betService;
    private readonly UserManager<AppUser> _userManager;

    public BetsController(IBetService betService, UserManager<AppUser> userManager)
    {
        _betService = betService;
        _userManager = userManager;
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Place(BetFormDto dto)
    {
        var userId = _userManager.GetUserId(User)!;

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Revisa el monto de la apuesta.";
            return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
        }

        try
        {
            await _betService.PlaceBetAsync(userId, dto);
            TempData["Success"] = $"¡Apuesta registrada! {dto.Amount:N0} FutCoins en juego. Suerte 🍀";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
    }

    public async Task<IActionResult> MyBets()
    {
        var userId = _userManager.GetUserId(User)!;
        return View(await _betService.GetMyBetsAsync(userId));
    }
}