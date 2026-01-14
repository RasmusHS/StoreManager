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
        // Basic validation for phone number format
        if (string.IsNullOrWhiteSpace(number))
        {
            return Result.Fail<PhoneNumber>(Errors.General.ValueIsRequired(number));
        }

        if (!int.TryParse(number.Trim(), out _))
        {
            return Result.Fail<PhoneNumber>(Errors.General.UnexpectedValue($"Value {number} is not a number"));
        }

        return Result.Ok(new PhoneNumber($"+{countryCode}", number));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
    }
}
