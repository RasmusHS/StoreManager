namespace StoreManager.Domain.Common.ValueObjects;

public class FullName : ValueObject
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public FullName() { } //for ORM

    public static Result<FullName> Create(string firstName, string lastName)
    {
        List<Error> errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(firstName))
            errors.Add(Errors.General.ValueIsRequired(nameof(firstName)));
        if (string.IsNullOrWhiteSpace(lastName))
            errors.Add(Errors.General.ValueIsRequired(nameof(lastName)));

        if (errors.Any())
            return Result.Fail<FullName>(errors);
        else
            return Result.Ok<FullName>(new FullName(firstName, lastName));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
