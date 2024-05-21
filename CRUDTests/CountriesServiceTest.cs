using ServiceContracts;
using ServiceContracts.DTO;
using Services;
namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        //constructor
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry
        //CountryAddRequest 為 null 時 應該拋出ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null!;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act 
                _countriesService.AddCountry(request);
            });
        }
        //當CountryName 為 null 時 應該拋出ArgumentNullException

        [Fact]
        public void AddCountry_CountryIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act 
                _countriesService.AddCountry(request);
            });
        }

        //當CountryName 重複時應該拋出ArgumentNullException

        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act 
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        //當你提供 CountryName 應該要加入 countries list

        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };

            //Act 
            CountryResponse response = _countriesService.AddCountry(request);
            List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountries();

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        //List 在預設值時應該為empty(再加入任何國家之前) 
        public void GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_countries_response_list = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_countries_response_list);
        }

        [Fact]
        //List 在預設值時應該為empty(再加入任何國家之前) 
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
                new CountryAddRequest{ CountryName="USA" },
                new CountryAddRequest{ CountryName="UK" },
            };

            //Act
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();
            foreach (CountryAddRequest country_request in countryAddRequests)
            {
                countries_list_from_add_country.Add(_countriesService.AddCountry(country_request));
            }
            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            //read each element from ountries_list_from_add_country

            //Assert
            foreach (CountryResponse expected_country in countries_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByCountryID

        //如果我們提供的ID為null 它將會回傳 null的CountryResponse
        [Fact]
        public void GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? country_response_from_get_method = _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
        }

        // 如果
        [Fact]

        //如果我們提供有效的CountryID，它應該作為相應的CountryResponse物件作為回應
        public void GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "Taiwan" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            //Act
            CountryResponse? country_response_from_get = _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

            //Assert

            Assert.Equal(country_response_from_add,country_response_from_get);
        }

        #endregion
    }
}
