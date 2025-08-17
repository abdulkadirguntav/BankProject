using BankProject2.Data;
using BankProject2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;
using Moq;

public class BankDbContextTests : IDisposable
{
    private readonly BankDbContext _context;

    public BankDbContextTests()
    {
        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BankDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void CanConnectToDatabase_InMemory_Success()
    {
        Assert.True(_context.Database.CanConnect());
    }

    [Fact]
    public void DbSets_AreCorrectlyInitialized()
    {
        Assert.NotNull(_context.transactions);
        Assert.NotNull(_context.accounts);
        Assert.NotNull(_context.currency);
    }

    [Fact]
    public void Currency_EntityConfiguration_HasCorrectKey()
    {
        var entity = _context.Model.FindEntityType(typeof(Currency));
        var primaryKey = entity.FindPrimaryKey();

        Assert.Single(primaryKey.Properties);
        Assert.Equal("CurrencyID", primaryKey.Properties[0].Name);
        Assert.Equal(ValueGenerated.OnAdd, entity.FindProperty("CurrencyID").ValueGenerated);
    }
}