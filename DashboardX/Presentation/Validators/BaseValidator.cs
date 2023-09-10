using FluentValidation;

namespace Presentation.Validators
{
    public class BaseValidator<T> : AbstractValidator<T>
    {
        /// <summary>
        /// Property terminates if submit button should be in disabled state.
        /// </summary>
        public bool Disabled => !Validated;
        public bool Validated { get; protected set; }
        public BaseValidator()
        {
            Validated = false;
        }

        private DateTime lastChcek = DateTime.Now;

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
