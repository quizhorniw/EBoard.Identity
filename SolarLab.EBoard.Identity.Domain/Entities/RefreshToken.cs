namespace SolarLab.EBoard.Identity.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? Revoked { get; set; }

    public RefreshToken(Guid userId, string token, DateTime createdAt, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Invalid refresh token", nameof(token));
        }

        if (createdAt >= expiresAt)
        {
            throw new ArgumentException("Invalid expiration date and time", nameof(expiresAt));
        }
        
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        Revoked = null;
    }
    
    public bool IsActive(DateTime isActiveAt) => Revoked == null && ExpiresAt > isActiveAt;
    
    public void Revoke(DateTime revokedAt) => Revoked = revokedAt;
}