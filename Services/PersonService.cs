using ServiceContracts;
using ServiceContracts.DTO;
using System;

namespace Services
{
    public class PersonService : IPersonsService
    {
        public PersonResponse AddPerson(PersonAddRequest personAddRequest)
        {
            throw new NotImplementedException();
        }

        public List<PersonResponse> GetAllPerson()
        {
            throw new NotImplementedException();
        }
    }
}
