using FluentValidation;
using Freelance.WebApi.Contracts.Requests;
using Freelance.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Freelance.Validators
{
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        private readonly IUserRepository _userRepository;

        public RegistrationRequestValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage("Email не может  быть пустым")
                .MustAsync((x, email, cancellation) => UniqueEmail(x, email)).WithMessage("Email уже зарегистрирован")
                .EmailAddress().WithMessage("Не корректный Email");

            RuleFor(m => m.Password)
                .NotEmpty().WithMessage("Пароль не может быть пустым");
        }

        private async Task<bool> UniqueEmail(RegistrationRequest request, string email)
        {
            return !(await _userRepository.IsUserExist(request.Email));
        }
    }
}
