using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Helpers;

public class ResponseDtos
{
}

// Response wrapper for Chain operations
// Envelope wrapper for Chain operations
public class ChainResponseDto
{
    [JsonPropertyName("result")]
    public ChainData? Result { get; set; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("timeGenerated")]
    public DateTime TimeGenerated { get; set; }

    // Helper property to easily get ChainId
    public Guid ChainId => Result?.Id ?? Guid.Empty;
}

// The actual Chain data (matching QueryChainDto)
public class ChainData
{
    [JsonPropertyName("id")] // Changed from "chainId" to "id"
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("stores")]
    public List<StoreData>? Stores { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("modifiedOn")]
    public DateTime ModifiedOn { get; set; }
}

// Envelope wrapper for Store operations
public class StoreResponseDto
{
    [JsonPropertyName("result")]
    public StoreData? Result { get; set; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("timeGenerated")]
    public DateTime TimeGenerated { get; set; }

    // Helper property to easily get StoreId
    public Guid Id => Result?.Id ?? Guid.Empty;
}

// The actual Store data (matching QueryStoreDto)
public class StoreData
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("chainId")]
    public Guid? ChainId { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("modifiedOn")]
    public DateTime ModifiedOn { get; set; }
}
