using FluentValidation;

namespace RealEstateManagement.Shareable.Requests.Validations
{
    public class AddFromCsvFileRequestValidator : AbstractValidator<AddFromCsvFileRequest>
    {
        private readonly string[] _allowedExtensions = { ".csv", ".txt" };
        private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB

        public AddFromCsvFileRequestValidator()
        {
            RuleFor(x => x.Files)
                .NotEmpty().WithMessage("At least one file must be uploaded")
                .Must(files => files.Count > 0).WithMessage("File collection cannot be empty")
                .Must(files => files.Count <= 1).WithMessage("You can upload only one file at a time");

            RuleForEach(x => x.Files).ChildRules(file =>
            {
                file.RuleFor(f => f.Length)
                    .NotNull()
                    .LessThanOrEqualTo(MaxFileSize).WithMessage($"File size must be less than {MaxFileSize / (1024 * 1024)} MB");

                file.RuleFor(f => f.FileName)
                    .NotEmpty()
                    .Must(fileName => _allowedExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .WithMessage($"Only the following file types are allowed: {string.Join(", ", _allowedExtensions)}");
            });
        }
    }
}
