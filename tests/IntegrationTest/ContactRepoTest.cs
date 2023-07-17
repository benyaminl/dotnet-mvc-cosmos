using coba.Models.Database;
using coba.Models.Request;
using coba.Repositories.Domain;
using coba.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTest;

public class ContactRepoTest
{

    public Task<List<ContactMessage>> GetContactList()
    {
        List<ContactMessage> data = new List<ContactMessage>()
        {
            new ContactMessage(){
                Email = "a@a.com",
                Message = " Hello World",
                Id = "1",
                Name = "AAA"
            },
            new ContactMessage(){
                Email = "b@b.com",
                Message = "BBB Hello World",
                Id = "2",
                Name = "BBB"
            }
        };

        return Task.FromResult(data);
    }

    [Fact]
    public async void GetContactList_UnitTest()
    {
        var fakeContactRepo = new Mock<IContactRepository>();
        fakeContactRepo.SetReturnsDefault(GetContactList());
        var fakeLogger = new Mock<ILogger<HomeController>>();

        var homeController = new HomeController(fakeLogger.Object, fakeContactRepo.Object);
        var result = await homeController.ContactList();

        var view = Assert.IsType<ViewResult>(result);
        var list = Assert.IsAssignableFrom<List<ContactMessage>>(view.ViewData.Model);

        Assert.True(list.Count > 0, "Returned data shouldn't be 0, there should be more than 0");
    }

    [Fact]
    public async void AddContactMsg_UnitTest()
    {
        var data = await GetContactList();
        var fakeContactRepo = new Mock<IContactRepository>();
        
        fakeContactRepo
            .Setup(r => r.GetListMessageAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(data));
        var oldData = await fakeContactRepo.Object.GetListMessageAsync("");
        int oldCount = oldData.Count;

        // @see https://stackoverflow.com/a/50796051/4906348
        fakeContactRepo
            .Setup(r => r.SaveMessage(It.IsAny<ContactMessage>()))
            .Returns(() => Task.FromResult(data))
            .Callback((ContactMessage m) => data.Add(m));
        
        var fakeLogger = new Mock<ILogger<HomeController>>();

        var homeController = new HomeController(fakeLogger.Object, fakeContactRepo.Object);
        var result = homeController.Contact(new ContactRequest() {
            Name = "C",
            Email = "c@c.com",
            Message = " Hai From C"
        });

        var newData = await fakeContactRepo.Object.GetListMessageAsync("");
        
        Assert.True(newData.Count > oldCount, "New data should be larger than old data");
    }

    [Fact]
    public async void DeleteContactMsg_Test()
    {
        var data = await GetContactList();

        var fakeContactRepo = new Mock<IContactRepository>();
        
        fakeContactRepo
            .Setup(d => d.GetListMessageAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(data));
        
        var oldData = await fakeContactRepo.Object.GetListMessageAsync("");
        int oldCount = oldData.Count;

        fakeContactRepo
            .Setup(d => d.DeleteMessage(It.IsAny<string>()))
            .Callback((string Id) => {
                int index = data.FindIndex(d => d.Id == Id);
                data.RemoveAt(index);
            });

        await fakeContactRepo.Object.DeleteMessage("2");

        Assert.True(data.Count < oldCount, "Data should be lower than old data");
    }
}