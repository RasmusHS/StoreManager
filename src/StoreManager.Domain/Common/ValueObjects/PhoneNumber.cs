using System;
using System.Collections.Generic;
using System.Text;

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
            return Result.Fail<PhoneNumber>("Phone number cannot be empty.");
        }

        if (!int.TryParse(number.Trim()))
        {
            return Result.Fail<PhoneNumber>("Phone number must contain only digits.");
        }

        var fullNumber = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(countryCode) && int.TryParse(number.Trim()))
        {
            fullNumber.Append($"{countryCode}");
            fullNumber.Append(number);
        }

        return Result.Ok(new PhoneNumber(fullNumber.ToString()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
    }
}
