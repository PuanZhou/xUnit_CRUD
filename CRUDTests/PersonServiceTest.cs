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

        //constructor

        public PersonServiceTest()
        {
            _personsService = new PersonService();
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
            #endregion
        }

        //假如我們提供PersonName為null值，應該丟出ArgumentException
        [Fact]
        public void AddPerson_PersonNIsNull()
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
    }
}
