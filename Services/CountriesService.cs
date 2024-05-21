using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field 
        private readonly List<Country> _countries;

        public CountriesService()
        {
            _countries = new List<Country>();
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //CountryAddRequest 參數不能為null

            if (countryAddRequest is null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //驗證 CountryName 不可為null

            if (string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //驗證CountryName不可重複

            if (_countries.Any(country => country.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException("Given country name already exists");
            }

            // 將countryAddRequest 轉型為Country
            Country country = countryAddRequest.ToCountry();

            //賦予country Guid

            country.CountryID = Guid.NewGuid();

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        //從Country物件轉型為CountryResponse
        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (!countryID.HasValue)
            {
                return null;
            }

            Country? country_response_from_list = _countries.FirstOrDefault(country => country.CountryID == countryID);

            if (country_response_from_list == null)
            {
                return null;
            }

            return country_response_from_list.ToCountryResponse();
        }
    }
}
