using Application.Abstractions;
using Domain.Users;
using SharedKernel;

namespace Application.Users.SignUp;

public class UserSignUpHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordService _passwordService;
    private readonly IAuthorizationTokenService _tokenService;

    public UserSignUpHandler(IUnitOfWork uow, IPasswordService passwordService, IAuthorizationTokenService tokenService)
    {
        _uow = uow;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }
    
    public async Task<Result<string>> Handle(UserSignUp dto)
    {
        var emailResult = UserEmail.Create(dto.Email);
        var usernameResult = UserName.Create(dto.Username);
        var passwordResult = UserRawPassword.Create(dto.Password);

        if (!emailResult.IsSuccess) return emailResult.Error!;
        if (!usernameResult.IsSuccess) return usernameResult.Error!;
        if (!passwordResult.IsSuccess) return passwordResult.Error!;
        
        var isUserExists = await _uow.Users.IsExists(emailResult.Value!, usernameResult.Value!);
        if (isUserExists)
        {
            return UserErrors.AlreadyExists;
        }

        var password = await _passwordService.Hash(passwordResult.Value!);
        
        var newUser = await _uow.Users.Create(emailResult.Value!, usernameResult.Value!, password);
        await _uow.Commit();
        return await _tokenService.GenerateToken(new AuthorizationTokenPayload(newUser.Id));
    }
}