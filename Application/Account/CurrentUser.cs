namespace Application.Account;

public record CurrentUser(string Id,
    string Email, 
    IEnumerable<string> Roles,
    string FirstName,
    string LastName,
    string Username,
    string PhoneNumber = ""
    )
{
    public bool IsInRole(string role) => Roles.Contains(role);
}
