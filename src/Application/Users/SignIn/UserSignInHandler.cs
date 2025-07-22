using Application.Abstractions;
using Domain.Users;
using SharedKernel;

namespace Application.Users.SignIn;

public class UserSignInHandler
{
    private readonly IPasswordService _passwordService;
    private readonly IAuthorizationTokenService _tokenService;

    public UserSignInHandler(IPasswordService passwordService, IAuthorizationTokenService tokenService)
    {
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> Handle(UserSignIn dto)
    {
        var usernameResult = UserName.Create(dto.Username);
        var passwordResult = UserRawPassword.Create(dto.Password);

        if (!usernameResult.IsSuccess) return usernameResult.Error!;
        if (!passwordResult.IsSuccess) return passwordResult.Error!;
        
        // TODO:
        // var foundUser = await _userRepository.Fetch(usernameResult.Value!);
        // if (foundUser is null)return UserErrors.NotFound;

        // TODO:
        // var isPasswordCorrect = await _passwordService.Verify(passwordResult.Value!, foundUser.Password);
        // if (!isPasswordCorrect) return UserErrors.LoginFailed;
        // return await _tokenService.GenerateToken(new AuthorizationTokenPayload(foundUser.Id));
        return string.Empty;
    }
}