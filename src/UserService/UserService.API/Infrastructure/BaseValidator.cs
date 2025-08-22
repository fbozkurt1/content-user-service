using FluentValidation;

namespace UserService.API.Infrastructure
{
    public class BaseValidator<T> : AbstractValidator<T>
    {
        public ValidationResponse ValidateCommand(T req, ValidationResponse? baseValidationResponse = default)
        {
            var vRes = Validate(req);
            var message = string.Join(", ", vRes.Errors.Select(x => x.ErrorMessage));

            if (baseValidationResponse == null || baseValidationResponse.Valid)
                return new ValidationResponse(vRes.IsValid, message);

            var messageRes = vRes.IsValid
                ? baseValidationResponse.Message
                : string.Concat(baseValidationResponse.Message, ", ", message);
            return new ValidationResponse(false, messageRes);
        }
    }

    public class ValidationResponse(bool valid, string message)
    {
        public bool Valid { get; } = valid;
        public string Message { get; } = message;
    }
}
