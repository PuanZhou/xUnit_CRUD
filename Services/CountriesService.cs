using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field 
        private readonly List<Country> _countries;

        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country>()
                {
                    new Country() { CountryID = Guid.Parse("CBCEF3EE-7125-4113-B1E4-63045D53A159"), CountryName = "USA" },
                    new Country() { CountryID = Guid.Parse("8E311F26-1845-49C4-A359-E156EEE5BC6F"), CountryName = "Japan" },
                    new Country() { CountryID = Guid.Parse("512671BD-7C05-4C75-96F8-E2279FE11896"), CountryName = "Canada" },
                    new Country() { CountryID = Guid.Parse("B50D9ABE-9183-49FD-B5B7-2082AB990735"), CountryName = "Taiwan" },
                    new Country() { CountryID = Guid.Parse("C802CF14-5842-4082-95BC-14A924A8389D"), CountryName = "China" }
                });
            }
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
