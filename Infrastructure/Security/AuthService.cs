using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AppCore;
using AppCore.Interfaces;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

	public class AuthService : IAuthService
	{
	    private readonly UserManager<CrmUser> _userManager;
	    private readonly ContactsDbContext _context;
	    private readonly JwtSettings _jwtOptions;
	
	    public AuthService(
	        UserManager<CrmUser> userManager,
	        ContactsDbContext context,
	        JwtSettings jwtOptions)
	    {
	        _userManager = userManager;
	        _context     = context;
	        _jwtOptions  = jwtOptions;
	    }
	
	    // ── Logowanie ─────────────────────────────────────────
	
	    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
	    {
	        // Weryfikacja użytkownika
	        var user = await _userManager.FindByEmailAsync(dto.Email)
	            ?? throw new Exception("Nieprawidłowy email lub hasło.");
	
	        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
	        {
	            // Rejestracja nieudanej próby — obsługa blokady konta
	            await _userManager.AccessFailedAsync(user);
	            throw new Exception("Nieprawidłowy email lub hasło.");
	        }
	
	        if (user.Status != SystemUserStatus.Active)
	            throw new Exception("Konto jest nieaktywne.");
	
	        if (await _userManager.IsLockedOutAsync(user))
	            throw new Exception("Konto jest zablokowane.");
	
	        // Reset licznika nieudanych prób
	        await _userManager.ResetAccessFailedCountAsync(user);
	
	        // Rejestracja czasu ostatniego logowania
	        
	        await _userManager.UpdateAsync(user);
	
	        return await GenerateAuthResponseAsync(user);
	    }
	
	    // ── Odświeżenie tokenu ────────────────────────────────
	
	    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
	    {
	        var principal = GetPrincipalFromExpiredToken(dto.AccessToken);
	
	        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
	            ?? throw new Exception("Nieprawidłowy token.");
	
	        var user = await _userManager.FindByIdAsync(userId)
	            ?? throw new Exception("Użytkownik nie istnieje.");
	
	        // Weryfikacja refresh tokenu
	        var refreshToken = await _context.RefreshTokens
	            .FirstOrDefaultAsync(t =>
	                t.Token == dto.RefreshToken &&
	                t.UserId == userId)
	            ?? throw new Exception("Nieprawidłowy refresh token.");
	
	        if (!refreshToken.IsActive)
	            throw new Exception("Refresh token wygasł lub został odwołany.");
	
	        // Generowanie nowej pary tokenów
	        var newResponse = await GenerateAuthResponseAsync(user);
	
	        // Unieważnienie starego refresh tokenu
	        refreshToken.Revoke(newResponse.RefreshToken);
	        await _context.SaveChangesAsync();
	
	        return newResponse;
	    }
	
	    // ── Odwołanie tokenu (wylogowanie) ────────────────────
	
	    public async Task RevokeTokenAsync(string refreshToken)
	    {
	        var token = await _context.RefreshTokens
	            .FirstOrDefaultAsync(t => t.Token == refreshToken)
	            ?? throw new Exception(nameof(RefreshToken) + "Nie istnieje " + refreshToken);
	
	        if (!token.IsActive)
	            throw new Exception("Token jest już nieaktywny.");
	
	        token.Revoke();
	        await _context.SaveChangesAsync();
	    }
	
	    // ── Metody pomocnicze ─────────────────────────────────
	
	    private async Task<AuthResponseDto> GenerateAuthResponseAsync(CrmUser user)
	    {
	        var roles       = await _userManager.GetRolesAsync(user);
	        var accessToken = GenerateAccessToken(user, roles);
	        var refreshToken = await GenerateRefreshTokenAsync(user.Id);
	
	        return new AuthResponseDto
	        {
	            AccessToken  = accessToken,
	            RefreshToken = refreshToken.Token,
	            ExpiresAt    = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
	            User = new UserDto()
	            {
	                Id = user.Id,
	                FirstName = user.FirstName,
	                LastName = user.LastName,
	                Status = user.Status,
	                Email = user.Email,
	                Department = user.Department,
	                Roles = roles.ToList()
	            }
	        };
	    }
	    //  ── Metoda generująca token ────────────────
	    private string GenerateAccessToken(CrmUser user, IList<string> roles)
	    {
	        // Claims — informacje zakodowane w tokenie
	        var claims = new List<Claim>
	        {
	            new(ClaimTypes.NameIdentifier, user.Id),
	            new(ClaimTypes.Email,          user.Email!),
	            new(ClaimTypes.GivenName,      user.FirstName),
	            new(ClaimTypes.Surname,        user.LastName),
	            new("department",              user.Department),
	            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
	            new(JwtRegisteredClaimNames.Iat,
	                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
	        };
	
	        // Dodanie ról jako claims
	        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
	        // Szyfrowanie podpisu
	        var credentials = new SigningCredentials(
	            _jwtOptions.GetSymmetricKey(),
	            SecurityAlgorithms.HmacSha256);
	        // Tworzenie tokenu
	        var token = new JwtSecurityToken(
	            issuer:             _jwtOptions.Issuer,
	            audience:           _jwtOptions.Audience,
	            claims:             claims,
	            notBefore:          DateTime.UtcNow,
	            expires:            DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
	            signingCredentials: credentials);
	
	        return new JwtSecurityTokenHandler().WriteToken(token);
	    }
	    //  ── Metoda generująca nowy token odświażania ────────────────
	    private async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
	    {
	        // Unieważnienie poprzednich aktywnych tokenów użytkownika
	        var activeTokens = await _context.RefreshTokens
	            .Where(t => t.UserId == userId && t.RevokedAt == null)
	            .ToListAsync();
	
	        foreach (var token in activeTokens)
	            token.Revoke();
	
	        // Generowanie nowego tokenu
	        var refreshToken = new RefreshToken
	        {
	            UserId    = userId,
	            Token     = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
	            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
	        };
	
	        await _context.RefreshTokens.AddAsync(refreshToken);
	        await _context.SaveChangesAsync();
	
	        return refreshToken;
	    }
	
	    // Odczytanie claims z wygasłego access tokenu
	    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
	    {
	        var parameters = new TokenValidationParameters
	        {
	            ValidateIssuer           = true,
	            ValidateAudience         = true,
	            ValidateLifetime         = false, // ignorujemy wygaśnięcie
	            ValidateIssuerSigningKey = true,
	            ValidIssuer              = _jwtOptions.Issuer,
	            ValidAudience            = _jwtOptions.Audience,
	            IssuerSigningKey         = _jwtOptions.GetSymmetricKey()
	        };
	
	        var handler   = new JwtSecurityTokenHandler();
	        var principal = handler.ValidateToken(
	            accessToken, parameters, out var securityToken);
	
	        // Weryfikacja algorytmu podpisu
	        if (securityToken is not JwtSecurityToken jwtToken ||
	            !jwtToken.Header.Alg.Equals(
	                SecurityAlgorithms.HmacSha256,
	                StringComparison.OrdinalIgnoreCase))
	        {
	            throw new Exception("Nieprawidłowy token.");
	        }
	
	        return principal;
	    }
	}