namespace SolarLab.EBoard.Identity.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }

    public RefreshToken(Guid userId, string token, DateTime expires)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Invalid refresh token", nameof(token));
        }

        if (DateTime.UtcNow >= expires)
        {
            throw new ArgumentException("Invalid expiration date and time", nameof(expires));
        }
        
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        Expires = expires;
        Created = DateTime.UtcNow;
        Revoked = null;
    }
    
    public bool IsActive() => Revoked == null && Expires > DateTime.UtcNow;
    
    public void Revoke() => Revoked = DateTime.UtcNow;
}