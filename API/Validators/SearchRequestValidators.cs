using Core.DTOs;
using FluentValidation;

namespace API.Validators
{
    
    /// <summary>
    /// Validator for search request DTOs - ensures data integrity
    /// Uses FluentValidation for comprehensive validation rules
    /// </summary>
    public class SearchRequestValidator : AbstractValidator<SearchRequestDto>
    {
        public SearchRequestValidator()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty()
                .WithMessage("Search term is required")
                .MaximumLength(500)
                .WithMessage("Search term cannot exceed 500 characters")
                .Matches(@"^[a-zA-Z0-9\s\-_\+\.]+$")
                .WithMessage("Search term contains invalid characters");

            RuleFor(x => x.TargetUrl)
                .NotEmpty()
                .WithMessage("Target URL is required")
                .MaximumLength(1000)
                .WithMessage("Target URL cannot exceed 1000 characters")
                .Must(BeAValidUrl)
                .WithMessage("Target URL format is invalid");
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var cleanUrl = url.Replace("http://", "")
                             .Replace("https://", "")
                             .Replace("www.", "");
            
            return cleanUrl.Contains('.') && 
                   !cleanUrl.Contains(' ') && 
                   cleanUrl.Length > 3 &&
                   !cleanUrl.StartsWith('.') &&
                   !cleanUrl.EndsWith('.');
        }
    }
}



