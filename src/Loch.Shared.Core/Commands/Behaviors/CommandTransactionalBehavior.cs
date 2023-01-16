using System.Data.SqlClient;
using Loch.Shared.Core.Application;
using Loch.Shared.Core.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Loch.Shared.Commands.Behaviors
{
    public class CommandTransactionalBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : AppResult
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessage _message;

        public CommandTransactionalBehavior(IUnitOfWork unitOfWork, IMessage message)
        {
            _unitOfWork = unitOfWork;
            _message = message;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var response = await next();
                await _unitOfWork.CommitAsync(cancellationToken);
                return response;
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;
                var number = sqlException.Number;
                List<AppErrorResult> errors = new List<AppErrorResult>();

                // Delecte Constraint
                if (number == 547)
                {
                    var error = new AppErrorResult(((int)GenericErrorType.EntityRestrictDelete).ToString(), _message.Messages[(long)GenericErrorType.EntityRestrictDelete]);
                    errors.Add(error);
                }
                else
                {
                    // TODO: ALERT: Remove this error on production
                    var error = new AppErrorResult(number.ToString(), sqlException.Message);
                    errors.Add(error);
                }

                // TODO: Hanlde another error types here
                return (TResponse)AppResult.Fail(errors);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
