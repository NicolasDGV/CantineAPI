using CantineAPI.DTOs;
using CantineAPI.Models;
using CantineAPI.Models.Enums;
using CantineAPI.Services;
using CantineAPI.Repositories;
using Moq;
using Xunit;

namespace CantineAPI.Tests;

public class CantineServiceTests
{
    private readonly Mock<IClientRepository> _mockClientRepository;
    private readonly Mock<IRepasRepository> _mockRepasRepository;
    private readonly Mock<ITicketRepository> _mockTicketRepository;
    private readonly Mock<Repas> _mockRepas;
    private readonly ClientRepository _clientRepository;
    private readonly RepasRepository _repasRepository;
    private readonly TicketRepository _ticketRepository;
    private readonly CantineService _cantineService;

    public CantineServiceTests()
    {
        _mockClientRepository = new Mock<IClientRepository>();
        _cantineService = new CantineService(_mockClientRepository.Object, _mockRepasRepository.Object, _mockTicketRepository.Object);

        var produits = new List<Repas>
        {
            new Repas { Id = 1, Name = "Entrée", Price  = 2.0m, RepasType = RepasType.Entree },
            new Repas { Id = 2, Name = "Plat", Price    = 5.0m, RepasType = RepasType.Plat },
            new Repas { Id = 3, Name = "Dessert", Price = 2.0m, RepasType = RepasType.Dessert },
            new Repas { Id = 4, Name = "Pain", Price    = 1.0m, RepasType = RepasType.Pain }
        }.AsQueryable();

        // Configurer un mock d'IQueryable<Produit>
        _mockRepas = new Mock<Repas>();
        _mockRepas.As<IQueryable<Repas>>().Setup(m => m.Provider).Returns(produits.Provider);
        _mockRepas.As<IQueryable<Repas>>().Setup(m => m.Expression).Returns(produits.Expression);
        _mockRepas.As<IQueryable<Repas>>().Setup(m => m.ElementType).Returns(produits.ElementType);
        _mockRepas.As<IQueryable<Repas>>().Setup(m => m.GetEnumerator()).Returns(() => produits.GetEnumerator());
    }
    
    [Fact]
    public void CreerClient_DoitAjouterClient()
    {
        // Arrange
        var clientDto = new ClientDto
        {
            Name    = "Client Test",
            Type    = ClientType.Interne,
            BudgetCantine = 20.0m
        };

        var addedClient = new Client
        {
            Id         = Guid.NewGuid(),
            Name       = clientDto.Name,
            ClientType = clientDto.Type,
            BudgetCantine    = clientDto.BudgetCantine
        };

        _mockClientRepository
            .Setup(repo => repo.Add(It.IsAny<Client>())) 
            .Callback<Client>(c => addedClient = c);

        // Act
        var result = _cantineService.CreateClient(clientDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientDto.Name, result.Name);
        Assert.Equal(clientDto.Type, result.ClientType);
        Assert.Equal(clientDto.BudgetCantine, result.BudgetCantine);
        _mockClientRepository.Verify(repo => repo.Add(It.Is<Client>(c => c.Name == clientDto.Name)), Times.Once);
    }


    [Fact]
    public void ObtenirClientParId_RetourneClientSiExiste()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId,
            Name = "Client Test",
            ClientType = ClientType.Interne,
            BudgetCantine = 50.0m
        };

        _mockClientRepository
            .Setup(repo => repo.GetClientById(clientId))
            .Returns(client);

        // Act
        var result = _cantineService.GetClientById(clientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(client.Id, result?.Id);
        Assert.Equal(client.Name, result?.Name);
    }

    [Fact]
    public void CrediterCompte_CrediteAvecSucces()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new Client
        {
            Id = clientId,
            Name = "Client Test",
            ClientType = ClientType.Interne,
            BudgetCantine = 10.0m
        };

        _mockClientRepository
            .Setup(repo => repo.GetClientById(clientId))
            .Returns(client);

        // Act
        var success = _cantineService.CreditAccount(clientId, 5.0m);

        // Assert
        Assert.True(success);
        Assert.Equal(15.0m, client.BudgetCantine);
        _mockClientRepository.Verify(repo => repo.UpdateClient(client), Times.Once);
    }

    [Fact]
    public void CrediterCompte_ClientInexistantRetourneFaux()
    {
        // Arrange
        var clientId = Guid.NewGuid();

        _mockClientRepository
            .Setup(repo => repo.GetClientById(clientId))
            .Returns((Client?)null);

        // Act
        var success = _cantineService.CreditAccount(clientId, 5.0m);

        // Assert
        Assert.False(success);
        _mockClientRepository.Verify(repo => repo.UpdateClient(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Test_PlateauAvecPrixFixe()
    {
        // Arrange
        var client = new Client
        {
            Id         = Guid.NewGuid(),
            Name       = "Client Interne",
            ClientType = ClientType.Interne,
            BudgetCantine    = 10.0m
        };

        _clientRepository.Add(client);

        var ticketDto = new TicketDto
        {
            Repas = new List<Repas>
            {
                new Repas { Id = 1, Name = "Entrée", RepasType  = RepasType.Entree, Price  = 2.0m },
                new Repas { Id = 2, Name = "Plat", RepasType    = RepasType.Plat, Price    = 5.0m },
                new Repas { Id = 3, Name = "Dessert", RepasType = RepasType.Dessert, Price = 2.0m },
                new Repas { Id = 4, Name = "Pain", RepasType    = RepasType.Pain, Price    = 1.0m }
            }
        };

        // Act
        var result = _cantineService.Pay(client.Id, ticketDto);

        // Assert
        //Assert.True(result.Succes);
        //Assert.Equal(10.0m, result.Ticket.Total);
        //Assert.Equal(2.5m, result.Ticket.TotalFinal);
        //Assert.Equal(7.5m, client.BudgetCantine);
    }
    
    [Fact]
    public void Test_PlatAvecSupplements()
    {
        // Arrange
        var client = new Client
        {
            Id         = Guid.NewGuid(),
            Name       = "Client Prestataire",
            ClientType = ClientType.Prestataire,
            BudgetCantine    = 20.0m
        };

        _clientRepository.Add(client);

        var ticket = new TicketDto
        {
            Repas = new List<Repas>
            {
                new Repas { Id = 1, Name = "Entrée", RepasType = RepasType.Entree, Price = 2.0m },
                new Repas { Id = 2, Name = "Plat", RepasType = RepasType.Plat, Price = 5.0m },
                new Repas { Id = 3, Name = "Dessert", RepasType = RepasType.Dessert, Price = 2.0m },
                new Repas { Id = 5, Name = "Boisson", RepasType = RepasType.Boisson, Price = 1.0m },               // Supplément
                new Repas { Id = 6, Name = "Grand Salade Bar", RepasType = RepasType.GrandSaladeBar, Price = 6.0m } // Supplément
            }
        };

        // Act
        var result = _cantineService.Pay(client.Id, ticket);

        // Assert
        //Assert.True(result.Succes);
        //Assert.Equal(16.0m, result.Ticket.Total);
        //Assert.Equal(10.0m, result.Ticket.TotalFinal);
        //Assert.Equal(10.0m, client.Balance);
    }
    
    [Fact]
    public void Test_SoldeInsuffisant_Visiteur()
    {
        // Arrange
        var client = new Client
        {
            Id         = Guid.NewGuid(),
            Name       = "Client Visiteur",
            ClientType = ClientType.Visiteur,
            BudgetCantine    = 5.0m // Solde insuffisant
        };

        _clientRepository.Add(client);

        var ticket = new TicketDto
        {
            Repas = new List<Repas>
            {
                new Repas { Id = 1, Name = "Entrée", RepasType = RepasType.Entree, Price = 2.0m },
                new Repas { Id = 2, Name = "Plat", RepasType   = RepasType.Plat, Price   = 5.0m }
            }
        };

        // Act
        var result = _cantineService.Pay(client.Id, ticket);

        // Assert
        //Assert.False(result.Succes);        // Paiement refusé
        //Assert.Equal("Solde insuffisant", result.Message);
        //Assert.Equal(5.0m, client.Balance); // Solde inchangé
    }
    
    [Fact]
    public void Test_SoldeInsuffisant_InterneDecouvert()
    {
        // Arrange
        var client = new Client
        {
            Id         = Guid.NewGuid(),
            Name       = "Client Interne",
            ClientType = ClientType.Interne,
            BudgetCantine    = 2.0m // Solde insuffisant, mais découvert autorisé
        };

        _clientRepository.Add(client);

        var ticket = new TicketDto
        {
            Repas = new List<Repas>
            {
                new Repas { Id = 1, Name = "Entrée", RepasType  = RepasType.Entree, Price  = 2.0m },
                new Repas { Id = 2, Name = "Plat", RepasType    = RepasType.Plat, Price    = 5.0m },
                new Repas { Id = 3, Name = "Dessert", RepasType = RepasType.Dessert, Price = 2.0m },
                new Repas { Id = 4, Name = "Pain", RepasType    = RepasType.Pain, Price    = 1.0m }
            }
        };

        // Act
        var result = _cantineService.Pay(client.Id, ticket);

        // Assert
        //Assert.True(result.Succes);
        //Assert.Equal(10.0m, result.Ticket.Total);
        //Assert.Equal(2.5m, result.Ticket.TotalFinal);
        //Assert.Equal(-0.5m, client.BudgetCantine);
    }
    
    [Fact]
    public void Test_PlateauIncomplet()
    {
        // Arrange
        var client = new Client
        {
            Id         = Guid.NewGuid(),
            Name       = "Client Stagiaire",
            ClientType = ClientType.Stagiaire,
            BudgetCantine    = 15.0m
        };

        _clientRepository.Add(client);

        var ticket = new TicketDto
        {
            Repas = new List<Repas>
            {
                new Repas { Id = 1, Name = "Entrée", RepasType = RepasType.Entree, Price = 2.0m },
                new Repas { Id = 2, Name = "Plat", RepasType   = RepasType.Plat, Price   = 5.0m }
            }
        };

        // Act
        var result = _cantineService.Pay(client.Id, ticket);

        // Assert
        //Assert.True(result.Succes);
        //Assert.Equal(7.0m, result.Ticket.Total);
        //Assert.Equal(0.0m, result.Ticket.TotalFinal);
        //Assert.Equal(15.0m, client.Balance);
    }
}