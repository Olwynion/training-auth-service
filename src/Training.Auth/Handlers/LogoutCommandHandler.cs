using MediatR;
using Training.Auth.Domain.Repositories;

namespace Training.Auth.Handlers;

public class LogoutCommandHandler(IUserRepository userRepository) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user != null)
        {
            user.ClearRefreshToken();
            userRepository.Update(user);
        }
    }
}
