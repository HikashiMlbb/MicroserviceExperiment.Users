using Application.Abstractions;
using Domain.Users;
using SharedKernel;

namespace Application.Users.SignIn;

public class UserSignInHandler
{
    private readonly IPasswordService _passwordService;
    private readonly IUserRepository _userRepository;
    private readonly IAuthorizationTokenService _tokenService;

    public UserSignInHandler(IPasswordService passwordService, IUserRepository userRepository, IAuthorizationTokenService tokenService)
    {
        _passwordService = passwordService;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> Handle(UserSignIn dto)
    {
        var usernameResult = UserName.Create(dto.Username);
        var passwordResult = UserRawPassword.Create(dto.Password);

        if (!usernameResult.IsSuccess) return usernameResult.Error!;
        if (!passwordResult.IsSuccess) return passwordResult.Error!;
        
        var foundUser = await _userRepository.Fetch(usernameResult.Value!);
        if (foundUser is null)return UserErrors.NotFound;

        var isPasswordCorrect = await _passwordService.Verify(passwordResult.Value!, foundUser.Password);
        if (!isPasswordCorrect) return UserErrors.LoginFailed;

        return await _tokenService.GenerateToken(new AuthorizationTokenPayload(foundUser.Id));
    }
}