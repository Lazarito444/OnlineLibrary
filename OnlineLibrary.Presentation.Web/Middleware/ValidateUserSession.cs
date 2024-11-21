using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Presentation.Web.Extensions;

namespace OnlineLibrary.Presentation.Web.Middleware;

public class ValidateUserSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ValidateUserSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool HasUser()
    {
        User? user = _httpContextAccessor.HttpContext?.Session.Get<User>("LoggedUser");

        return (user != null);
    }   
}