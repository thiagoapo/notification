using Notification.Dto.Model;

namespace Notification.Application.Service.Interface
{
    public interface IAuthenticationService
    {
        TokenDto CreateToken(string login);
    }
}
