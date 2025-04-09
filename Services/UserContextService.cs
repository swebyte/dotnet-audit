public interface IUserContextService
{
    string? UserId { get; set; }
    string? Fullname { get; set; }
}

public class UserContextService : IUserContextService
{
    public string? UserId { get; set; }
    public string? Fullname { get; set; }
}