using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using Xunit.Abstractions;


namespace CRUDTests
{
    public class PersonServiceTest
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        //constructor

        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personsService = new PersonService(false);
            _countriesService = new CountriesService(false);
            _testOutputHelper = testOutputHelper;
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
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "John", Email = "person@example.com", Address = "sample address", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };

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
            PersonAddRequest person_request = new PersonAddRequest() { PersonName = "Kane", Email = "email@sample.com", Address = "address", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };

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

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2003-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "Rahman@example.com", Gender = GenderOptions.Male, Address = "address of Rahman", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("1993-03-02"), ReceiveNewsLetters = true };

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

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_get = _personsService.GetAllPersons();


            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }
        #endregion

        #region GetFilteredPersons

        // 如果search text 是empty  search by PersonName 那他會回傳所有的persons
        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "Taiwan" };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2003-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "Rahman@example.com", Gender = GenderOptions.Male, Address = "address of Rahman", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("1993-03-02"), ReceiveNewsLetters = true };

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

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), string.Empty);


            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }

        //我們一開始會加入一些persons 然後我們將搜尋person的人名，search string將會是一些文字，應該要回傳匹配的person
        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "Taiwan" };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2003-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "Rahman@example.com", Gender = GenderOptions.Male, Address = "address of Rahman", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("1993-03-02"), ReceiveNewsLetters = true };

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

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma");


            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName is not null)
                {
                    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, persons_list_from_search);
                    }
                }
            }
        }
        #endregion

        #region GetSortedPersons
        // 當我們排序利用PersonName 在DESC，他應該用降序返回 PersonName
        [Fact]
        public void GetSortedPersons()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "Taiwan" };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2003-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "Rahman@example.com", Gender = GenderOptions.Male, Address = "address of Rahman", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("1993-03-02"), ReceiveNewsLetters = true };

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

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> allPersons = _personsService.GetAllPersons();
            List<PersonResponse> persons_list_from_sort = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(person => person.PersonName).ToList();

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            for (int i = 0; i < person_response_list_from_add.Count(); i++)
            {
                Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson
        //當我們提供 一個Null PersonUpdateRequest ，應該丟出 ArgumentNullException
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdaterPerson(personUpdateRequest);
            });
        }

        //當我們提供 一個無效的PersonID ，應該丟出 ArgumentException
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            { PersonID = Guid.NewGuid() };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.UpdaterPerson(personUpdateRequest);
            });
        }

        //當我們提供PersonName為null，應該丟出 ArgumentException
        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };

            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", Email = "john@example.com", Address = "address...", Gender = GenderOptions.Male, CountryID = country_response_from_add.CountryID };

            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = null;

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act

                _personsService.UpdaterPerson(person_update_request);
            });
        }

        //首先我們將添加一人，之後嘗試更新他的name 和 email
        [Fact]
        public void UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };

            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID, Address = "example address", DateOfBirth = DateTime.Parse("2000-01-01"), Email = "john@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };

            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            person_update_request.PersonName = "William";
            person_add_request.Email = "william@example.com";

            //Act 
            PersonResponse person_response_from_update = _personsService.UpdaterPerson(person_update_request);

            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
        }
        #endregion

        #region DeletePerson
        //如果提供存在的PersonID，應該回傳 true
        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "Japan" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "Jones", Address = "address", CountryID = country_response_from_add.CountryID, DateOfBirth = Convert.ToDateTime("2010-01-01"), Email = "jones@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };
            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

            //Act
            bool isDeleted = _personsService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        //如果提供不存在的PersonID，應該回傳 false
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
           
            //Act
            bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }
        #endregion
    }
}
