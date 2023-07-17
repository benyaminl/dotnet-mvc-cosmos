using coba.Models.Database;
using coba.Models.Request;
using coba.Repositories.Domain;
using coba.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Azure.Cosmos;
using coba.Repositories.Infrastructure;

namespace IntegrationTest;

public class ContactControllerTest
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    private readonly CosmosClient _cosmosClient;

    public ContactControllerTest()
    {
        // Arrange
        // @see https://github.com/DamianEdwards/MinimalApiPlayground/blob/main/tests/MinimalApiPlayground.Tests/PlaygroundApplication.cs
        // @see https://stackoverflow.com/a/70095604/4906348
        var app = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder =>
        {
            // builder.ConfigureServices(services =>
            // {

            // });
            builder.UseEnvironment("Development");
        });

        _cosmosClient = app.Services.GetService<CosmosClient>();

        _server = app.Server;

        _client = app.CreateClient();
    }

    [Fact]
    public async void IndexPage_Test()
    {
        var res = await _client.GetStringAsync("/");

        Assert.True(res.Length > 0);
    }

    [Fact]
    public async void ContactListPage_Test()
    {
        var res = await _client.GetStringAsync("/Home/ContactList");

        Assert.True(res.IndexOf("<td>") > -1, "Page doesn't contain any data");
    }

    [Fact]
    public async void ContactPage_Test()
    {
        var res = await _client.GetStringAsync("/Home/Contact");

        Assert.True(res.Length > 0, "Page is blank");
    }

    [Fact]
    public async void ContactSubmit_Test()
    {
        var body = new ContactRequest()
        {
            Name = " Botako",
            Email = "Botako@a.com",
            Message = "Hai"
        };

        // This one I always Forget
        // We can use the properties to itterate to all prop, and make it into assoc array
        // Or dictionary C#
        // @see https://stackoverflow.com/a/9210493/4906348
        var bodyDictionary = body
            .GetType()
            .GetProperties()
            .ToDictionary(prop => prop.Name, prop => (string) prop.GetValue(body, null));
        
        var res = await _client.PostAsync("/Home/Contact", new FormUrlEncodedContent(bodyDictionary));

        Assert.True(res.StatusCode == System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public void ContactDelete_Test()
    {
        string id = System.Guid.NewGuid().ToString();

        var body = new ContactMessage()
        {
            Id = id,
            Name = " Botako",
            Email = "Botako@a.com",
            Message = "Hai"
        };

        var contactRepo = new ContactRepository(_cosmosClient);
        // Can't use await, because if it's, then it's broken... 
        // Because await don't wait the process to be done... 
        // and execute the delete without even allow the insert to be done
        contactRepo.SaveMessage(body);

        var res = _client.PostAsync("/Home/Contact/" + id, null).Result;

        Assert.True(res.StatusCode != System.Net.HttpStatusCode.NotFound, "Can't create and delete the Contact Message");
    }

    [Fact]
    public async void HomeAndPrivacyTest()
    {
        var res = await _client.GetStringAsync("/Home/Error");

        Assert.True(res.Length > 0);
        
        res = await _client.GetStringAsync("/Home/Privacy");

        Assert.True(res.Length > 0);
    }
}