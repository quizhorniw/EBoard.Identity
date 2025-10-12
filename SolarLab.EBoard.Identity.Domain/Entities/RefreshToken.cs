namespace SolarLab.EBoard.Identity.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? Revoked { get; private set; }

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