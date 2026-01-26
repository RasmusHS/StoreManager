using System.Text.RegularExpressions;

namespace StoreManager.Domain.Common.ValueObjects;

public class Email : ValueObject
{
    public Email() { }
    public string Value { get; private set; }

    /// <summary>
    /// Hiding the public constructor to prevent invalid Email objects to be created. Instead use the constructormethod
    /// </summary>
    /// <param name="value"></param>
    private Email(string value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Constructor method to create a new Email object. This constructor method now holds the validation rules.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static Result<Email> Create(string email, bool isRequired = false)
    {
        List<Error> errors = new List<Error>();

        email = (email ?? string.Empty).Trim();
        if (isRequired && string.IsNullOrWhiteSpace(email))
        {
            errors.Add(Errors.General.ValueIsRequired(nameof(email)));
        }
        if (email.Length >= 100)
        {
            errors.Add(Errors.General.ValueTooLarge(nameof(email), 100));
        }
        if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
        {
            errors.Add(Errors.General.UnexpectedValue(nameof(email)));
        }

        if (errors.Any())
            return Result.Fail<Email>(errors);
        else
            return Result.Ok<Email>(new Email(email));
    }

    /// <summary>
    /// Supporting the ability to implicitly convert an Email into a string
    /// </summary>
    /// <param name="email"></param>
    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    /// <summary>
    /// Supporting the ability to explicitly convert a string into an Email
    /// </summary>
    /// <param name="email"></param>
    public static explicit operator Email(string email)
    {
        return Create(email).Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
