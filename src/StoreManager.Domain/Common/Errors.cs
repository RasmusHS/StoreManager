using StoreManager.Domain.Common.ValueObjects;

namespace StoreManager.Domain.Common;

public static class Errors
{
    public static class General
    {
        public static Error NotFound<T>(T id) => new Error("entity.not.found", $"Could not find entity with ID {id}.", statusCode: 404);
        public static Error ValueIsRequired(string valueName) => new Error("value.is.required", $"Value '{valueName}' is required.");
        public static Error ValueTooSmall(string valueName, int minValue) => new Error("value.too.small", $"Value '{valueName}' should be at least {minValue}.");
        public static Error ValueTooLarge(string valueName, int maxValue) => new Error("value.too.large", $"Value '{valueName}' should not exceed {maxValue}.");
        public static Error UnexpectedValue(string value) => new Error("unexpected.value", $"Value '{value}' is not valid in this context.");
        public static Error Unauthorized() => new Error("unauthorized", $"Could not authorize access to entity.");

        public static Error ValueOutOfRange(string valueName, int minValue, int maxValue) =>
            new Error("value.out.of.Range", $"Value '{valueName}' should be between {minValue} and {maxValue}.");

        public static Error CreateEntityFailed(object entity) => new Error("create.entity.failed", $"Failed to create entity {entity} due to an unexpected error.");
        public static Error ExceptionThrown(string exceptionMessage) => new Error("exception.thrown", $"An exception was thrown: {exceptionMessage}");
    }

    public static class ChainErrors
    {
        public static Error ChainHasStores() => new Error("unauthorized", $"The chain could not be deleted due to still having stores. Delete all stores first.");
        public static Error ChainNameAlreadyExists(string chainName) => new Error("chain.name.already.exists", $"A chain with name '{chainName}' already exists.");
        public static Error ChainHasNoStores<T>(T id) where T : class => new Error("chain.has.no.stores", $"The chain with ID {id} has no stores.");
        public static Error NoChainsExist() => new Error("no.chains.exist", $"No chains exist in the system.");
    }

    public static class StoreErrors
    {
        public static Error StoreNumberAlreadyExists(int storeNumber) => new Error("store.number.already.exists", $"A store with number '{storeNumber}' already exists.");
        public static Error NoIndependentStoresFound() => new Error("no.independent.stores.found", $"No independent stores (stores without a chain) were found.");
    }

    public static class ValueObjectErrors
    {
        //public static Error NullOrEmptyValue(string valueName) => new Error("value.object.null.or.empty", $"The value object '{valueName}' cannot be null or empty.");
    }
}
