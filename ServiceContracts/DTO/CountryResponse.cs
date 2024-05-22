
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }

        // It compares the current object to anther object of CountryResponse type and returns, if both values are same; otherwise returns false
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }
            CountryResponse country_to_compare = (CountryResponse)obj;
            return this.CountryID == country_to_compare.CountryID && this.CountryName == country_to_compare.CountryName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName,
            };
        }
    }
}
