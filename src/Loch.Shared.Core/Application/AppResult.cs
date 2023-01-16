namespace Loch.Shared.Core.Application
{
    public class AppResult
    {
        public List<AppErrorResult> Errors { get; }
        public object Data { get; }
        public bool IsSuccess { get; }

        protected AppResult(bool isSuccess, object data)
        {
            Errors = new List<AppErrorResult>();
            IsSuccess = isSuccess;
            Data = data;
        }

        protected AppResult(List<AppErrorResult> errors)
        {
            if (errors != null)
            {
                Errors = errors;
            }
        }

        public static AppResult Ok(object data)
        {
            return new AppResult(true, data);
        }

        public static AppResult Ok()
        {
            return new AppResult(true, null);
        }

        public static AppResult NotFound()
        {
            return new AppResult(true, null);
        }

        public static AppResult Fail(List<AppErrorResult> errors = null)
        {
            return new AppResult(errors);
        }

        public AppResult AddError(AppErrorResult error)
        {
            Errors.Add(error);
            return this;
        }
    }
}
