﻿using ServiceContracts;
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
            List<PersonResponse> persons_list = _personsService.GetAllPersons();
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

        #region GetAllPersons
        //The GetAllPersons() 應該回傳一個empty list 在預設的狀況下
        [Fact]
        public void GetAllpersonsEmptyList()
        {
            //Act
            List<PersonResponse> persons_from_get = _personsService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }

        //一開始我們先加入少許person 當我們使用GetAllPersons()方法時，應該回傳所有我們添加過的person
        [Fact]
        public void GetAllpersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "Taiwan" };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceviceNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2003-02-02"), ReceviceNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Ron", Email = "ron@example.com", Gender = GenderOptions.Male, Address = "address of ron", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("1993-03-02"), ReceviceNewsLetters = true };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                person_request_1, person_request_2, person_request_3
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //Act
            List<PersonResponse> persons_list_from_get = _personsService.GetAllPersons();

            //Assert
            foreach(PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get)
            }
        }
        #endregion
    }
}
