using Loch.Shared.Core.Application;
using Loch.Shared.Core.Domain;
using FluentValidation;
using MediatR;

namespace Loch.Shared.Commands.Behaviors
{
    public class FailFastRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : AppResult
    {
        private readonly IValidator _validator;
        private readonly IMessage _message;

        public FailFastRequestBehavior(IValidator<TRequest> validator, IMessage message, IValidationContext validationContext)
        {
            _validator = validator ?? throw new Exception("Register command validator");
            _message = message;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var _valoidateContext = new ValidationContext<TRequest>(request);

                var validationResult = await _validator.ValidateAsync(_valoidateContext);
                if (validationResult.IsValid)
                {
                    return await next();
                }

                var errors = validationResult.Errors
                                             .Select(x => new AppErrorResult(x.ErrorCode, x.ErrorMessage))
                                             .ToList();
                return (TResponse)AppResult.Fail(errors);
            }
            catch (BusinessRuleValidationException exception)
            {
                return (TResponse)AppResult.Fail(exception.Errors.Select(x => new AppErrorResult(x.ErrorCode.ToString(), _message.Messages[x.ErrorCode])).ToList());
            }
        }
    }
}
