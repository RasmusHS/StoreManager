namespace StoreManager.Domain.Common.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string CountryCode { get; private set; }
    public string Number { get; private set; }

    private PhoneNumber(string countryCode, string number)
    {
        CountryCode = countryCode;
        Number = number;
    }

    public PhoneNumber() { } //for ORM

    public static Result<PhoneNumber> Create(string countryCode, string number)
    {
        List<Error> errors = new List<Error>();

        // Basic validation for phone number format
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            errors.Add(Errors.General.ValueIsRequired(countryCode));
        }
        if (string.IsNullOrWhiteSpace(number))
        {
            errors.Add(Errors.General.ValueIsRequired(number));
        }

        var cleanedCountryCode = countryCode.Trim().Replace("+", "").Replace("(", "").Replace(")", "");
        if (!Int64.TryParse(cleanedCountryCode, out _))
        {
            errors.Add(Errors.General.UnexpectedValue(nameof(countryCode)));
        }
        if (!Int64.TryParse(number.Trim(), out _))
        {
            errors.Add(Errors.General.UnexpectedValue(nameof(number)));
        }

        if (errors.Any())
            return Result.Fail<PhoneNumber>(errors);
        else
        {
            return Result.Ok(new PhoneNumber($"+{cleanedCountryCode}", number));
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
    }
}
