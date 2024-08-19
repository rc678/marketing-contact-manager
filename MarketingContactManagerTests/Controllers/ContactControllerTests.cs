using MarketingContactManager.Contexts;
using MarketingContactManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace MarketingContactManager.Controllers.Tests
{
    [TestClass()]
    public class ContactControllerTests
    {
        private Mock<ILogger<ContactController>> mockLogger;

        [TestInitialize]
        public void Setup()
        {
            mockLogger = new Mock<ILogger<ContactController>>();
        }

        [TestMethod()]
        public async Task GetContacts_ReturnsAllContacts()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                .Options;

            using (var context = new ContactContext(options))
            {
                context.Contacts.AddRange(
                    new ContactModel { Id = 1, FirstName = "Homer", LastName = "Simpson", Email = "homer@gmail.com", PhoneNumber = "1234567890" },
                    new ContactModel { Id = 2, FirstName = "Marge", LastName = "Simpson", Email = "marge@gmail.com", PhoneNumber = "0987654321" }
                );
                context.SaveChanges();

            }

            using (var context = new ContactContext(options))
            {
                var controller = new ContactController(context, mockLogger.Object);
                var result = await controller.GetContacts();
                var contacts = result.Value?.ToList();

                Assert.AreEqual(2, contacts?.Count);
            }
        }

        [TestMethod()]
        public async Task PutContactModel_UpdatesExistingContactFields()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                            .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                            .Options;

            using (var context = new ContactContext(options))
            {
                context.Contacts.AddRange(
                    new ContactModel { Id = 99, FirstName = "Homer", LastName = "Simpson", Email = "homer@gmail.com", PhoneNumber = "1234567890" }
                );
                context.SaveChanges();

            }


            using (var context = new ContactContext(options))
            {
                var controller = new ContactController(context, mockLogger.Object);
                var updatedContact = new ContactModel { FirstName = "Homer J.", LastName = "Simpson", Email = "homerj@gmail.com", PhoneNumber = "0987654321" };
                var result = await controller.PutContactModel(99, updatedContact);

                var contactInDb = context.Contacts.FirstOrDefault(c => c.Id == 99);
                Assert.IsNotNull(contactInDb);
                Assert.AreEqual("Homer J.", contactInDb.FirstName);
                Assert.AreEqual("Simpson", contactInDb.LastName);
                Assert.AreEqual("homerj@gmail.com", contactInDb.Email);
                Assert.AreEqual("0987654321", contactInDb.PhoneNumber);
            }
        }

        [TestMethod()]
        public async Task PostContactModel_CreatesNewRecord()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                            .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                            .Options;

            using (var context = new ContactContext(options))
            {
                var controller = new ContactController(context, mockLogger.Object);
                var newContact = new ContactModel
                {
                    FirstName = "Lisa",
                    LastName = "Simpson",
                    Email = "lisa.simpson@gmail.com",
                    PhoneNumber = "1234567890"
                };

                var result = await controller.PostContactModel(newContact);

                var contactInDb = context.Contacts.FirstOrDefault(c => c.Email == newContact.Email);

                Assert.IsNotNull(contactInDb);
                Assert.AreEqual("Lisa", contactInDb.FirstName);
                Assert.AreEqual("Simpson", contactInDb.LastName);
                Assert.AreEqual("lisa.simpson@gmail.com", contactInDb.Email);
                Assert.AreEqual("1234567890", contactInDb.PhoneNumber);
            }
        }

        [TestMethod()]
        public async Task PostContactModel_MissingFields_ReturnsBadRequest()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                            .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                            .Options;

            using (var context = new ContactContext(options))
            {
                var controller = new ContactController(context, mockLogger.Object);
                var newContact = new ContactModel
                {
                    FirstName = "Bart",
                    LastName = "",
                    Email = "bart.simpson@gmail.com",
                    PhoneNumber = ""
                };

                var result = await controller.PostContactModel(newContact);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
            }
        }

        [TestMethod()]
        public async Task PostContactModel_InvalidEmailFormat_ReturnsBadRequest()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                            .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                            .Options;

            using (var context = new ContactContext(options))
            {
                var controller = new ContactController(context, mockLogger.Object);
                var newContact = new ContactModel
                {
                    FirstName = "Marge",
                    LastName = "Simpson",
                    Email = "email",
                    PhoneNumber = "1234567890"
                };

                var result = await controller.PostContactModel(newContact);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
            }
        }

        [TestMethod()]
        public async Task PostContactModel_InvalidPhoneFormat_ReturnsBadRequest()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                            .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                            .Options;

            using (var context = new ContactContext(options))
            {
                var controller = new ContactController(context, mockLogger.Object);
                var newContact = new ContactModel
                {
                    FirstName = "Marge",
                    LastName = "Simpson",
                    Email = "marge@gmail.com",
                    PhoneNumber = "123"
                };

                var result = await controller.PostContactModel(newContact);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
            }
        }

        [TestMethod()]
        public async Task GetContactModel_ReturnsContact_WhenContactExists()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                            .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                            .Options;

            using (var context = new ContactContext(options))
            {
                var contact = new ContactModel
                {
                    Id = 1,
                    FirstName = "Lisa",
                    LastName = "Simpson",
                    Email = "lisa.simpson@gmail.com",
                    PhoneNumber = "1234567890"
                };
                context.Contacts.Add(contact);
                await context.SaveChangesAsync();

                var controller = new ContactController(context, mockLogger.Object);

                var result = await controller.GetContactModel(1);

                var returnedResult = result.Value;

                Assert.IsNotNull(returnedResult);
                Assert.AreEqual(contact.Id, returnedResult.Id);
                Assert.AreEqual(contact.FirstName, returnedResult.FirstName);
                Assert.AreEqual(contact.LastName, returnedResult.LastName);
                Assert.AreEqual(contact.Email, returnedResult.Email);
                Assert.AreEqual(contact.PhoneNumber, returnedResult.PhoneNumber);
            }

        }

        [TestMethod()]
        public async Task DeleteContactModel_RemovesContact_WhenContactExists()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                            .UseInMemoryDatabase(databaseName: "db" + Guid.NewGuid().ToString())
                            .Options;


            using (var context = new ContactContext(options))
            {
                var contact = new ContactModel
                {
                    Id = 1,
                    FirstName = "Bart",
                    LastName = "Simpson",
                    Email = "bart.simpson@gmail.com",
                    PhoneNumber = "1234567890"
                };
                context.Contacts.Add(contact);
                await context.SaveChangesAsync();
            }

            using (var context = new ContactContext(options))
            {
                var controller = new ContactController(context, mockLogger.Object);
                var result = await controller.DeleteContactModel(1);

                var contactInDb = await context.Contacts.FindAsync(1);
                Assert.IsNull(contactInDb);
            }
        }

    }
}