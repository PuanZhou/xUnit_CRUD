using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;


namespace CRUDTests
{
    public class PersonServiceTest
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        //constructor

        public PersonServiceTest()
        {
            _personsService = new PersonService();
            _countriesService = new CountriesService();
        }

        #region AddPerson

        //假如我們提供null值，應該丟出ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }

        //假如我們提供PersonName為null值，應該丟出ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            //Act
            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }

        //假如我們提供Person正確的屬性，應該將其加入 persons list 並且返回一個PersonResponse物件，包含了新生成的ID
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "John", Email = "person@example.com", Address = "sample address", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceviceNewsLetters = true };

            //Act

            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = _personsService.GetAllPerson();
            //Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            Assert.Contains(person_response_from_add, persons_list);
        }
        #endregion

        #region GetPersonByPersonID

        //如果我們提供　PersonID 為 null，他應該回傳一個null PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act 
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(personID);

            //Asser
            Assert.Null(person_response_from_get);
        }

        //如果我們提供了一個有效的Person id 他應該回傳給我們一個有效的person details as PersonResponse 物件
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Cnada" };
            CountryResponse country_response = _countriesService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = new PersonAddRequest() { PersonName = "Kane", Email = "email@sample.com", Address = "address", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceviceNewsLetters = false };

            PersonResponse person_response_from_add = _personsService.AddPerson(person_request);

            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);
            
            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);

        }

        #endregion
    }
}
