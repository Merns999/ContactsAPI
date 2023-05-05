using ContactsAPI.Data;
using ContactsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]//same as writting [Route("api/contacts")] ie its taking the controller name and injecting it into the route
    public class ContactsController : Controller
    {
        private readonly ContactsAPIDbContext contactsAPIDbContext;

        public ContactsController(ContactsAPIDbContext contactsAPIDbContext)
        {
            this.contactsAPIDbContext = contactsAPIDbContext;
        }

        [HttpGet]//for swaggerUI 
        public async Task<IActionResult> GetAllContacts()
        {
            return Ok(await contactsAPIDbContext.Contacts.ToListAsync());///because we cannot return a generic type(List) we wrap inside an Ok response which creates an a
        }
        
        
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetSingleContact([FromRoute] Guid id)
        {
            var contact = await contactsAPIDbContext.Contacts.FindAsync(id);

            if(contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }


        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteSingleContact([FromRoute] Guid id, UpdateContactRequest updateContactRequest)
        {
            var contact = await contactsAPIDbContext.Contacts.FindAsync(id);

            if(contact != null)
            {
                contactsAPIDbContext.Remove(contact);

                await contactsAPIDbContext.SaveChangesAsync();

                return Ok(contact);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(AddContactRequest addContactRequest)
        {
            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                Address = addContactRequest.Address,
                Email = addContactRequest.Email,
                FullName = addContactRequest.FullName,
                PhoneNumber = addContactRequest.PhoneNumber
            };
            await contactsAPIDbContext.Contacts.AddAsync(contact);
            await contactsAPIDbContext.SaveChangesAsync();
            return Ok(contact);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateContact([FromRoute] Guid id, UpdateContactRequest updateContactRequest)
        {
            var contact = await contactsAPIDbContext.Contacts.FindAsync(id);
            if(contact != null)
            {
                contact.FullName = updateContactRequest.FullName;
                contact.Address = updateContactRequest.Address;
                contact.PhoneNumber = updateContactRequest.PhoneNumber;
                contact.Email = updateContactRequest.Email;

                await contactsAPIDbContext.SaveChangesAsync();

                return Ok(contact);
            }
            return NotFound();
        }
    }
}
