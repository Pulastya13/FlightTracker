using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace FlightStatus.Api.Helpers;

/// <summary>
/// Validates flight number format.
/// Valid format: 2 uppercase letters followed by 1-4 digits (e.g., SK1234, BA456).
/// </summary>
public static partial class FlightNumberValidator
{
    [GeneratedRegex(@"^[A-Z]{2}\d{1,4}$", RegexOptions.Compiled)]
    private static partial Regex FlightNumberPattern();

    public static bool IsValid([NotNullWhen(true)]string? flightNumber)
    {
        return !string.IsNullOrWhiteSpace(flightNumber) && FlightNumberPattern().IsMatch(flightNumber.Trim().ToUpperInvariant());
    }
}
