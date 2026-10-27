using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GolBet.Services.Implementations;

public class BetService : IBetService
{
    private readonly IBetRepository _betRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public BetService(
        IBetRepository betRepository,
        IMatchRepository matchRepository,
        UserManager<AppUser> userManager,
        IMapper mapper)
    {
        _betRepository = betRepository;
        _matchRepository = matchRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task PlaceBetAsync(string userId, BetFormDto dto)
    {
        var match = await _matchRepository.GetByIdAsync(dto.MatchId);
        if (match is null || !match.IsActive)
            throw new InvalidOperationException("El partido no existe.");

        if (match.Status != MatchStatus.Scheduled || match.Date <= DateTime.UtcNow)
            throw new InvalidOperationException("Este partido ya no acepta apuestas.");

        if (await _betRepository.ExistsAsync(dto.MatchId, userId, dto.Pick))
            throw new InvalidOperationException("Ya tienes una apuesta con esta selección en este partido.");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Usuario no encontrado.");

        if (dto.Amount > user.Balance)
            throw new InvalidOperationException($"Saldo insuficiente: tienes {user.Balance:N2} FutCoins.");

        var oddsAtPlacement = dto.Pick switch
        {
            BetPick.Home => match.HomeOdds,
            BetPick.Draw => match.DrawOdds,
            BetPick.Away => match.AwayOdds,
            _ => throw new InvalidOperationException("Selección inválida.")
        };

        user.Balance -= dto.Amount;

        var bet = new Bet
        {
            MatchId = dto.MatchId,
            UserId = userId,
            Pick = dto.Pick,
            Amount = dto.Amount,
            OddsAtPlacement = oddsAtPlacement,
            Status = BetStatus.Pending
        };

        await _betRepository.AddAsync(bet);
    }

    public async Task<IEnumerable<BetDto>> GetMyBetsAsync(string userId)
        => _mapper.Map<IEnumerable<BetDto>>(await _betRepository.GetByUserAsync(userId));
}