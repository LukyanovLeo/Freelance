using FluentValidation;
using Freelance.WebApi.Contracts.Requests;
using Freelance.Repositories.Interfaces;
using Freelance.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Freelance.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        private readonly ISessionSaltService _sessionSaltService;
        private readonly IHashService _hashService;
        private readonly IUserRepository _userRepository;
        private readonly IAuthSaltRepository _authSaltRepository;

        public LoginRequestValidator(
            ISessionSaltService sessionSaltService,
            IHashService hashService,
            IUserRepository userRepository,
            IAuthSaltRepository authSaltRepository
            )
        {
            _sessionSaltService = sessionSaltService;
            _hashService = hashService;
            _userRepository = userRepository;
            _authSaltRepository = authSaltRepository;

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage("Email не может  быть пустым")
                .EmailAddress().WithMessage("Не корректный Email");

            RuleFor(m => m.ExpiredAt)
                .Must((x, expiredAt) => IsSaltValid(x)).WithMessage("Соль просроченна")
                .DependentRules(() =>
                 {
                     RuleFor(m => m.HashedPassword)
                         .NotEmpty().WithMessage("Email не может  быть пустым")
                         .MustAsync((x, hashedPassword, cancellation) => VerifyPassword(x)).WithMessage("Логин или Пароль указан неверно");
                 });
        }

        private bool IsSaltValid(LoginRequest request)
        {
            return _sessionSaltService.IsValid(Convert.FromBase64String(request.SessionSaltAsBase64), request.ExpiredAt);
        }

        private async Task<bool> VerifyPassword(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            var passwordSalt = await _authSaltRepository.GetUserPasswordSalt(user.Id);

            return _hashService.VerifyPassword(
                request.HashedPassword,
                Convert.FromBase64String(request.SessionSaltAsBase64),
                Convert.FromBase64String(user.Password),
                Convert.FromBase64String(passwordSalt));
        }
    }
}
