namespace UrlShortener.Models;

public enum UserRole
{
    User,
    Admin
}

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public ICollection<ShortUrl> ShortUrls { get; set; } = new List<ShortUrl>();
}
