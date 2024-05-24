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
        //privatte field
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;


        public PersonService()
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
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
            return _persons.Select(person => person.ToPersonResponse()).ToList();
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

            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilteredPersons(string seachBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrWhiteSpace(seachBy) || string.IsNullOrWhiteSpace(searchString))
            {
                return matchingPersons;
            }
            switch (seachBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons.Where(person =>
                    (!string.IsNullOrWhiteSpace(person.PersonName) ?
                    person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(person =>
                   (!string.IsNullOrWhiteSpace(person.Email) ?
                   person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(person =>
                   (person.DateOfBirth != null) ?
                   person.DateOfBirth.Value.ToString("yyyy-MM-dd").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;


                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(person =>
                   (!string.IsNullOrWhiteSpace(person.Gender) ?
                   person.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.CountryID):
                    matchingPersons = allPersons.Where(person =>
                   (!string.IsNullOrWhiteSpace(person.Country) ?
                   person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.Address):
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
            throw new NotImplementedException();
        }
    }
}
