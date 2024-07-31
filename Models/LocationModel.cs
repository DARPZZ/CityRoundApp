public class LocationModel
{
    public string PostalCode { get; set; }
    public string CountryCode { get; set; }
    public string FeatureName { get; set; }

    public LocationModel(string postalCode, string countryCode, string featureName)
    {
        PostalCode = postalCode;
        CountryCode = countryCode;
        FeatureName = featureName;
    }
}
