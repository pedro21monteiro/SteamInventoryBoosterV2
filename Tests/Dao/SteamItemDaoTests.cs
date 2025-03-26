using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SteamInventoryBoosterV2.Dao;
using SteamInventoryBoosterV2.Data;
using SteamInventoryBoosterV2.Models.Domain;


namespace Tests.Dao
{
    public class SteamItemDaoTests : IAsyncLifetime
    {
        private AppDbContext _context;
        private SteamItemDao _dao;

        public SteamItemDaoTests() { }

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Banco único para cada teste
               .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _dao = new SteamItemDao(_context);
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task ItemExistsByLinkAsync_ReturnsTrue_WhenItemExists()
        {
            // Arrange
            var item = new SteamItem { Id = 1, Link = "https://steamitem.com/item1" };
            _context.SteamItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _dao.ItemExistsByLinkAsync(item.Link);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_AddsItemToDatabase()
        {
            // Arrange
            var steamItem = new SteamItem { Id = 1, Link = "https://steamitem.com/item2" };

            // Act
            await _dao.CreateAsync(steamItem);

            // Assert
            _context.SteamItems.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenAddingDuplicateItem()
        {
            // Arrange
            var steamItem = new SteamItem { Id = 1, Link = "https://steamitem.com/item2" };

            // Act
            await _dao.CreateAsync(steamItem);
            Func<Task> act = async () => await _dao.CreateAsync(steamItem);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            _context.SteamItems.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllItems()
        {
            // Arrange
            _context.SteamItems.AddRange(
                new SteamItem { Link = "https://steamitem.com/item1", Name = "Item1" },
                new SteamItem { Link = "https://steamitem.com/item2", Name = "Item2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _dao.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesItem()
        {
            // Arrange
            var item = new SteamItem { Id = 1, Link = "https://steamitem.com/item1", Name = "Item1" };
            _context.SteamItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            item.Link = "https://steamitem.com/updated";
            await _dao.UpdateAsync(item);

            // Assert
            var updatedItem = await _context.SteamItems.FindAsync(1);
            updatedItem.Should().NotBeNull();
            updatedItem.Link.Should().Be("https://steamitem.com/updated");
        }

        [Fact]
        public async Task GetSteamItemByIdAsync_ReturnsItem_WhenExists()
        {
            // Arrange
            var item = new SteamItem { Id = 4, Link = "https://steamitem.com/item4" };
            _context.SteamItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _dao.GetSteamItemByIdAsync(4);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(4);
            result.Link.Should().Be("https://steamitem.com/item4");
        }

        [Fact]
        public async Task DeleteAsync_RemovesItemFromDatabase()
        {
            // Arrange
            var item = new SteamItem { Id = 5, Link = "https://steamitem.com/item5" };
            _context.SteamItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            await _dao.DeleteAsync(item);

            // Assert
            _context.SteamItems.Should().HaveCount(0);
        }
    }
}
