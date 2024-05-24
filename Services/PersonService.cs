using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonService : IPersonsService
    {
        //private field
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;


        public PersonService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
            if (initialize)
            {
                _persons.AddRange(new List<Person>(){

                new Person() { PersonID = Guid.Parse("57B2855F-6DEA-443C-81DD-CF46208BC489"), PersonName = "Richart", Email = "rprettyjohns0@mapy.cz", DateOfBirth = DateTime.Parse("1992-11-17"), Gender = "Male", Address = "92 Graceland Trail", ReceiveNewsLetters = true, CountryID = Guid.Parse("CBCEF3EE-7125-4113-B1E4-63045D53A159") },

                new Person() { PersonID = Guid.Parse("68780E55-B8C7-4F9E-96ED-87675BBD3284"), PersonName = "Lovell", Email = "llapides1@pen.io", DateOfBirth = DateTime.Parse("1998-11-13"), Gender = "Male", Address = "118 Ohio Junction", ReceiveNewsLetters = true, CountryID = Guid.Parse("512671BD-7C05-4C75-96F8-E2279FE11896") },

                new Person() { PersonID = Guid.Parse("F13AFAF1-5FB2-467C-87E7-E91DB58D83FB"), PersonName = "Drusilla", Email = "dlafaye2@comcast.net", DateOfBirth = DateTime.Parse("1996-12-18"), Gender = "Female", Address = "0 Upham Avenue", ReceiveNewsLetters = false, CountryID = Guid.Parse("B50D9ABE-9183-49FD-B5B7-2082AB990735") },

                new Person() { PersonID = Guid.Parse("49CD89A0-B049-42F6-B6D2-17217E906390"), PersonName = "Verge", Email = "vlaible3@disqus.com", DateOfBirth = DateTime.Parse("1992-03-26"), Gender = "Male", Address = "72176 Logan Lane", ReceiveNewsLetters = false, CountryID = Guid.Parse("C802CF14-5842-4082-95BC-14A924A8389D") },

                new Person() { PersonID = Guid.Parse("64660C3B-91E2-4706-BB50-1C4F37239948"), PersonName = "Eloisa", Email = "egaltone4@gizmodo.com", DateOfBirth = DateTime.Parse("1990-01-26"), Gender = "Female", Address = "09 Cody Alley", ReceiveNewsLetters = false, CountryID = Guid.Parse("8E311F26-1845-49C4-A359-E156EEE5BC6F") },

                new Person() { PersonID = Guid.Parse("64B82A7D-0D71-4031-9B85-EF8838982AED"), PersonName = "Alastair", Email = "alanghorne5@phoca.cz", DateOfBirth = DateTime.Parse("1994-09-07"), Gender = "Male", Address = "54658 Arizona Court", ReceiveNewsLetters = true, CountryID = Guid.Parse("B50D9ABE-9183-49FD-B5B7-2082AB990735") },

                new Person() { PersonID = Guid.Parse("F7A440D8-5B2B-4C3A-8AF6-308091EA9D6D"), PersonName = "Serge", Email = "stuberfield6@yellowbook.com", DateOfBirth = DateTime.Parse("1991-06-20"), Gender = "Male", Address = "509 Russell Pass", ReceiveNewsLetters = true, CountryID = Guid.Parse("CBCEF3EE-7125-4113-B1E4-63045D53A159") },

                new Person() { PersonID = Guid.Parse("03530734-D7B1-4C20-8647-B5468CCC3542"), PersonName = "Jefferson", Email = "jlambkin7@behance.net", DateOfBirth = DateTime.Parse("1994-06-25"), Gender = "Male", Address = "465 Cambridge Plaza", ReceiveNewsLetters = false, CountryID = Guid.Parse("8E311F26-1845-49C4-A359-E156EEE5BC6F") },

                new Person() { PersonID = Guid.Parse("61511681-A95E-475D-8019-60B881FFB276"), PersonName = "Shellysheldon", Email = "shanington8@google.cn", DateOfBirth = DateTime.Parse("2000-04-28"), Gender = "Male", Address = "51875 Thierer Alley", ReceiveNewsLetters = true, CountryID = Guid.Parse("C802CF14-5842-4082-95BC-14A924A8389D") },

                new Person() { PersonID = Guid.Parse("67BC2CE2-A94C-4F02-B22B-59C20633488D"), PersonName = "Fredric", Email = "fnapoleon9@msu.edu", DateOfBirth = DateTime.Parse("1998-07-15"), Gender = "Male", Address = "53390 Golf Plaza", ReceiveNewsLetters = false, CountryID = Guid.Parse("512671BD-7C05-4C75-96F8-E2279FE11896") },
                });
            }
        }
        private PersonResponse CovertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;

            return personResponse;
        }
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            // 檢查PersonAddRequest 是否為null
            if (personAddRequest is null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }


            //Model validation

            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            // add person object to persons list
            _persons.Add(person);

            //convert the Person object into PersonResponse type

            return CovertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(person => CovertPersonToPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }

            Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);

            if (person == null)
            {
                return null;
            }

            return CovertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrWhiteSpace(searchBy) || string.IsNullOrWhiteSpace(searchString))
            {
                return matchingPersons;
            }
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(person =>
                    (!string.IsNullOrWhiteSpace(person.PersonName) ?
                    person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(person =>
                   (!string.IsNullOrWhiteSpace(person.Email) ?
                   person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(person =>
                   (person.DateOfBirth != null) ?
                   person.DateOfBirth.Value.ToString("yyyy-MM-dd").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;


                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(person =>
                   (!string.IsNullOrWhiteSpace(person.Gender) ?
                   person.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(person =>
                   (!string.IsNullOrWhiteSpace(person.Country) ?
                   person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(person =>
                   (!string.IsNullOrWhiteSpace(person.Address) ?
                   person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default:
                    matchingPersons = allPersons;
                    break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceviceNewsLetters), SortOrderOptions.ASC)
                => allPersons.OrderBy(person => person.ReceviceNewsLetters).ToList(),

                (nameof(PersonResponse.ReceviceNewsLetters), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(person => person.ReceviceNewsLetters).ToList(),

                _ => allPersons
            };
            return sortedPersons;
        }

        public PersonResponse UpdaterPerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest is null)
            {
                throw new ArgumentNullException(nameof(Person));
            }

            //驗證資料
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person object to update
            Person? matchingPerson = _persons.FirstOrDefault(person => person.PersonID == personUpdateRequest.PersonID);

            if (matchingPerson is null)
            {
                throw new ArgumentException("Given Person id doesn't exit");
            }

            //update all details

            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            return CovertPersonToPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID is null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);

            if (person is null)
            {
                return false;
            }

            _persons.RemoveAll(person => person.PersonID == personID);

            return true;
        }
    }
}
