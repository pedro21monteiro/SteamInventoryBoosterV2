using SteamInventoryBoosterV2.Models.Utilities;
using SteamInventoryBoosterV2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class CalculusTests
    {
        [Theory]
        [InlineData(0.02, 0)]
        [InlineData(0.43, 0.39)]
        [InlineData(0.44, 0.39)]
        [InlineData(0.45, 0.39)]
        [InlineData(0.46, 0.40)]
        [InlineData(0.21, 0.19)]
        [InlineData(0.22, 0.19)]
        [InlineData(1.12, 0.99)]
        public void BuyerPaysToSellerReceives_ShouldReturnExpected(decimal paidMoney, decimal expected)
        {
            // Act
            var result = Calculus.BuyerPaysToSellerReceives(paidMoney);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0.01, 0.03)]
        [InlineData(0.38, 0.42)]
        [InlineData(0.39, 0.43)]
        [InlineData(0.40, 0.46)]
        [InlineData(1.00, 1.15)]
        public void SellerReceivesToBuyerPays_ShouldReturnExpected(decimal receivedMoney, decimal expected)
        {
            // Act
            var result = Calculus.SellerReceivesToBuyerPays(receivedMoney);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
