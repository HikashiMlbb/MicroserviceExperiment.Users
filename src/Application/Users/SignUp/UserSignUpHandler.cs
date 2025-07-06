using Application.Abstractions;
using Domain.Users;
using SharedKernel;

namespace Application.Users.SignUp;

public class UserSignUpHandler
{
    private readonly IUserRepository _repo;
    private readonly IPasswordService _passwordService;
    private readonly IAuthorizationTokenService _tokenService;

    public UserSignUpHandler(IUserRepository repo, IPasswordService passwordService, IAuthorizationTokenService tokenService)
    {
        _repo = repo;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }
    
    public async Task<Result<string>> Handle(UserSignUp dto)
    {
        var emailResult = UserEmail.Create(dto.Email);
        var usernameResult = UserName.Create(dto.Username);

        if (!emailResult.IsSuccess) return emailResult.Error!;
        if (!usernameResult.IsSuccess) return usernameResult.Error!;
        // TODO: Create raw password validation

        var isUserExists = await _repo.IsExists(emailResult.Value!, usernameResult.Value!);
        if (isUserExists)
        {
            return new Error("User.AlreadyExists", "User with given Email or Username already exists.");
        }

        var password = await _passwordService.Hash(dto.Password);

        var newUser = await _repo.Create(emailResult.Value!, usernameResult.Value!, password);
        return await _tokenService.GenerateToken(new AuthorizationTokenPayload(newUser.Id.Value));
    }
}