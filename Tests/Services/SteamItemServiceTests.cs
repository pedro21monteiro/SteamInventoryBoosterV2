using FluentAssertions;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ExceptionExtensions;
using SteamInventoryBoosterV2.Dao;
using SteamInventoryBoosterV2.Interfaces.IDao;
using SteamInventoryBoosterV2.Interfaces.IServices;
using SteamInventoryBoosterV2.Models.Domain;
using SteamInventoryBoosterV2.Models.Dtos;
using SteamInventoryBoosterV2.Models.Utilities;
using SteamInventoryBoosterV2.Services;

namespace Tests.Services
{
    public class SteamItemServiceTests
    {
        private readonly ISteamItemDao _steamItemDao;
        private readonly IWebScraper _webScraper;
        private readonly SteamItemService _steamItemService;

        public SteamItemServiceTests()
        {
            _steamItemDao = Substitute.For<ISteamItemDao>();
            _webScraper = Substitute.For<IWebScraper>();
            _steamItemService = new SteamItemService(_steamItemDao, _webScraper);
        }

        [Fact]
        public async Task CreateSteamItemAsync_ReturnsFailure_WhenItemAlreadyExists()
        {
            // Arrange
            var link = "https://steamitem.com/item1";
            _steamItemDao.ItemExistsByLinkAsync(link).Returns(true);

            // Act
            var result = await _steamItemService.CreateSteamItemAsync(link, 10, 20);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Message.Should().Be(Error.SteamItemAlreadyExists.Message);
            result.Value.Should().BeNull();
        }

        [Fact]
        public async Task CreateSteamItemAsync_ReturnsFailure_WhenScraperFails()
        {
            var link = "https://steamitem.com/item1";
            _steamItemDao.ItemExistsByLinkAsync(link).Returns(false);
            _webScraper.CollectBasicSteamItemInformation(link).Returns(Result<SteamItemInfoDto>.Failure(Error.InternalError));

            var result = await _steamItemService.CreateSteamItemAsync(link, 10, 20);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Message.Should().Be(Error.InternalError.Message);
            result.Value.Should().BeNull();
        }

        [Fact]
        public async Task CreateSteamItemAsync_ReturnsSuccess_WhenItemCreated()
        {
            var link = "https://steamitem.com/item1";
            var steamItemDto = new SteamItemInfoDto { Link = link };

            _steamItemDao.ItemExistsByLinkAsync(link).Returns(false);
            _webScraper.CollectBasicSteamItemInformation(link).Returns(Result<SteamItemInfoDto>.Success(steamItemDto));

            var result = await _steamItemService.CreateSteamItemAsync(link, 10, 20);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Link.Should().Be(link);
        }

        [Fact]
        public async Task CreateSteamItemAsync_ShouldReturnFailure_WhenCreateAsyncThrowsException()
        {
            // Arrange
            var link = "https://steamitem.com/item1";
            var steamItemDto = new SteamItemInfoDto { Link = link };

            _webScraper.CollectBasicSteamItemInformation(link).Returns(Result<SteamItemInfoDto>.Success(steamItemDto));
            _steamItemDao.CreateAsync(Arg.Any<SteamItem>())
                        .Throws(new Exception("Error while saving steam item"));

            var service = new SteamItemService(_steamItemDao, _webScraper);

            // Act
            var result = await service.CreateSteamItemAsync(link, 10, 20);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be(Error.InternalError.Code);
            result.Error.Message.Should().Be(Error.InternalError.Message);
        }

        [Fact]
        public async Task GetAllItemsAsync_ReturnsItems()
        {
            // Arrange
            var items = new List<SteamItem> { new SteamItem { Link = "https://steamitem.com/item1" } };
            _steamItemDao.GetAllAsync().Returns(items);

            // Act
            var result = await _steamItemService.GetAllItemsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllItemsAsync_ShouldReturnFailure_WhenGetAllAsyncThrowsException()
        {
            // Arrange
            _steamItemDao.GetAllAsync().Throws(new Exception());

            // Act
            var result = await _steamItemService.GetAllItemsAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be(Error.InternalError.Code);
            result.Error.Message.Should().Be(Error.InternalError.Message);
        }

        [Fact]
        public async Task EditItemBuySellPricesAsync_ReturnsFailure_WhenItemNotFound()
        {
            // Arrange
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns((SteamItem)null!);

            // Act
            var result = await _steamItemService.EditItemBuySellPricesAsync("1", 10, 20);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Message.Should().Be(Error.SteamItemNotFound.Message);
        }

        [Fact]
        public async Task EditItemBuySellPricesAsync_ReturnsSuccess_WhenItemUpdated()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1", BuyPrice = 5, SellPrice = 15 };
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);

            // Act
            var result = await _steamItemService.EditItemBuySellPricesAsync("1", 10, 20);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value!.BuyPrice.Should().Be(10);
            result.Value!.SellPrice.Should().Be(20);
        }

        [Fact]
        public async Task EditItemBuySellPricesAsync_ShouldReturnFailure_WhenUpdateAsyncThrowsException()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1", BuyPrice = 5, SellPrice = 15 };
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);
            _steamItemDao.UpdateAsync(Arg.Any<SteamItem>()).Throws(new Exception());

            // Act
            var result = await _steamItemService.EditItemBuySellPricesAsync("1", 10, 20);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be(Error.InternalError.Code);
            result.Error.Message.Should().Be(Error.InternalError.Message);
        }

        [Fact]
        public async Task DeleteSteamItemAsync_ReturnsFailure_WhenItemNotFound()
        {
            // Arrange
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns((SteamItem)null!);

            // Act
            var result = await _steamItemService.DeleteSteamItemAsync("1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Message.Should().Be(Error.SteamItemNotFound.Message);
        }

        [Fact]
        public async Task DeleteSteamItemAsync_ReturnsSuccess_WhenItemDeleted()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1" };
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);

            // Act
            var result = await _steamItemService.DeleteSteamItemAsync("1");

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteSteamItemAsync_ShouldReturnFailure_WhenDeleteAsyncThrowsException()
        {
            // Arrange
            var item = new SteamItem { ClassId= "1" };
            _steamItemDao.GetSteamItemByClassIdAsync("1").Throws(new Exception());

            // Act
            var result = await _steamItemService.DeleteSteamItemAsync("1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be(Error.InternalError.Code);
            result.Error.Message.Should().Be(Error.InternalError.Message);
        }

        [Fact]
        public async Task UpdateSteamItemInfoAsync_ReturnsFailure_WhenItemNotFound()
        {
            // Arrange
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns((SteamItem)null!);

            // Act
            var result = await _steamItemService.UpdateSteamItemInfoAsync("1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Message.Should().Be(Error.SteamItemNotFound.Message);
        }

        [Fact]
        public async Task UpdateSteamItemInfoAsync_ReturnsFailure_WhenCollectItemInfoIsFailure()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1", Link = "https://steamitem.com/item1" };
            var steamItemDto = new SteamItemInfoDto { Link = "https://steamitem.com/item1" };

            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);
            _webScraper.CollectBasicSteamItemInformation(item.Link).Returns(Result<SteamItemInfoDto>.Failure(Error.InternalError));

            // Act
            var result = await _steamItemService.UpdateSteamItemInfoAsync("1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Message.Should().Be(Error.InternalError.Message);
        }

        [Fact]
        public async Task UpdateSteamItemInfoAsync_ReturnsSuccess_WhenItemUpdated()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1", Link = "https://steamitem.com/item1" };
            var steamItemDto = new SteamItemInfoDto { Link = "https://steamitem.com/item1" };

            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);
            _webScraper.CollectBasicSteamItemInformation(item.Link).Returns(Result<SteamItemInfoDto>.Success(steamItemDto));

            // Act
            var result = await _steamItemService.UpdateSteamItemInfoAsync("1");

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateSteamItemInfoAsync_ShouldReturnFailure_WhenCollectBasicSteamItemInformationThrowsException()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1", Link = "https://steamitem.com/item1" };
            var steamItemDto = new SteamItemInfoDto { Link = "https://steamitem.com/item1" };

            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);
            _webScraper.CollectBasicSteamItemInformation(item.Link).Throws(new Exception());

            // Act
            var result = await _steamItemService.UpdateSteamItemInfoAsync("1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be(Error.InternalError.Code);
            result.Error.Message.Should().Be(Error.InternalError.Message);
        }

        [Fact]
        public async Task ChangeVisibilityAsync_ReturnsFailure_WhenItemNotFound()
        {
            // Arrange
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns((SteamItem)null);

            // Act
            var result = await _steamItemService.ChangeVisibilityAsync("1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Message.Should().Be(Error.SteamItemNotFound.Message);
        }

        [Fact]
        public async Task ChangeVisibilityAsync_ReturnsSuccess_WhenVisibilityToggled()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1", IsVisible = true };
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);

            // Act
            var result = await _steamItemService.ChangeVisibilityAsync("1");

            // Assert
            result.IsSuccess.Should().BeTrue();
            item.IsVisible.Should().BeFalse();
        }

        [Fact]
        public async Task ChangeVisibilityAsync_ShouldReturnFailure_WhenUpdateAsyncThrowsException()
        {
            // Arrange
            var item = new SteamItem { ClassId = "1", IsVisible = true };
            _steamItemDao.GetSteamItemByClassIdAsync("1").Returns(item);
            _steamItemDao.UpdateAsync(Arg.Any<SteamItem>()).Throws(new Exception());

            // Act
            var result = await _steamItemService.ChangeVisibilityAsync("1");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be(Error.InternalError.Code);
            result.Error.Message.Should().Be(Error.InternalError.Message);
        }
    }
}
