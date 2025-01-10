
namespace WebAPI.Interface
{
    public interface IAuthService
    {
        public UserConfModel LoginUser(string username, string password);
        public void LogoutUser(UserConfModel user);
    }
}
