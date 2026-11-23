using basic_ecommerce_platform.Models;

namespace basic_ecommerce_platform.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) CreateToken(User user, IList<string> roles);
    }
}
