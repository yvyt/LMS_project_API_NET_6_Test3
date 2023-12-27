using UserService.Data;
using UserService.Model;

namespace UserService.Repository
{
    public interface IUserRepository
    {
        List<UserVM> getAll();
        UserVM getById(string id);
        UserVM Add(UserVM user);
        void Update(UserVM user); 
        void Delete(string id);
        User Validate(UserLogin model);
        Task<TokenModel> GenerateToken(User u, string secretKey);
        Task<string> GenerateRefreshToken(TokenModel model, string secretKey);

        UserVM Register(RegisterUser register);
    }
}
