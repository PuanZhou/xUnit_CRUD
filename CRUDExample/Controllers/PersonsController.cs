using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    //[Route("[controller]")]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }
        //Route("[action]
        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //Searching
            ViewBag.SerchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName),"Person Name" },
                { nameof(PersonResponse.Email),"Email" },
                { nameof(PersonResponse.DateOfBirth),"Date of Birth" },
                { nameof(PersonResponse.Gender),"Gender" },
                { nameof(PersonResponse.CountryID),"Country" },
                { nameof(PersonResponse.Address),"Address" }
            };
            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();


            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        //Executes when the user clicks on "Create Person" hyperlink(while opening the create view)
        [Route("persons/create")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country =>
                new SelectListItem() { Text = country.CountryName, Value = country.CountryID.ToString() }
            );

            return View();
        }

        [Route("persons/create")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(country =>
                         new SelectListItem() { Text = country.CountryName, Value = country.CountryID.ToString() });


                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("persons/edit/{personID}")]
        public IActionResult Edit(Guid personID)
        {
            PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);
            if (personResponse is null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country =>
                     new SelectListItem() { Text = country.CountryName, Value = country.CountryID.ToString() }
            );

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("persons/edit/{personID}")]
        public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if (personResponse is null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = _personsService.UpdaterPerson(personUpdateRequest);

                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(country =>
                         new SelectListItem() { Text = country.CountryName, Value = country.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }

        [HttpGet]
        [Route("persons/delete/{personID}")]
        public IActionResult Delete(Guid? personID)
        {
            PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);

            if (personResponse is null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("persons/delete/{personID}")]
        public IActionResult Delete(PersonUpdateRequest personUpdateRequest)
        {
           PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            if(personResponse is null)
            {
                return RedirectToAction("Index");
            }

            _personsService.DeletePerson(personUpdateRequest.PersonID);

            return RedirectToAction("Index");
        }
    }
}
