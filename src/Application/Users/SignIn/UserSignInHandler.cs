using Application.Abstractions;
using Domain.Users;
using SharedKernel;

namespace Application.Users.SignIn;

public class UserSignInHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordService _passwordService;
    private readonly IAuthorizationTokenService _tokenService;

    public UserSignInHandler(IUnitOfWork uow, IPasswordService passwordService, IAuthorizationTokenService tokenService)
    {
        _uow = uow;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> Handle(UserSignIn dto)
    {
        var usernameResult = UserName.Create(dto.Username);
        var passwordResult = UserRawPassword.Create(dto.Password);

        if (!usernameResult.IsSuccess) return usernameResult.Error!;
        if (!passwordResult.IsSuccess) return passwordResult.Error!;
        
        var foundUser = await _uow.Users.Fetch(usernameResult.Value!);
        if (foundUser is null) return UserApplicationErrors.NotFound;

        var isPasswordCorrect = await _passwordService.Verify(passwordResult.Value!, foundUser.Password);
        if (!isPasswordCorrect) return UserApplicationErrors.LoginFailed;
        
        return await _tokenService.GenerateToken(new AuthorizationTokenPayload(foundUser.Id));
    }
}