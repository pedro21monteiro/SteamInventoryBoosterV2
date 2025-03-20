using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Services
{
    public static class Calculus
    {
        private static readonly decimal MinimumFeeAmount = 0.01m; // Mínimo de taxa
        private static readonly decimal SteamTax = 0.05m; // 5%
        private static readonly decimal CS2Tax = 0.10m; // 10%
        private static readonly decimal TotalTax = 1 + SteamTax + CS2Tax; // 1.15

        public static decimal BuyerPaysToSellerReceives(decimal paidMoney)
        {
            // The minimum value is 0.03
            if (paidMoney < 0.03m)
                return 0;

            paidMoney = GetDecimalWithTwoDecimals(paidMoney);

            decimal closeSellerReceives = GetDecimalWithTwoDecimals(paidMoney / TotalTax);

            //Now we have to find out what the real number is from the SellerReceivesToBuyerPays function
            //Initially we'll see if you've found the right value
            decimal closeBuyerPays = SellerReceivesToBuyerPays(closeSellerReceives);

            if (closeBuyerPays == paidMoney)
            {
                return closeSellerReceives;
            }

            // If it's not the same you have to find the real one
            // If CloseBuyerPays < PaidMoney we will have to increase by 0.01 each time until we find the amount paid
            if (closeBuyerPays < paidMoney)
            {
                while (true)
                {
                    // There are several sales prices at which we receive the same price, such as 0.45, 0.44 and 0.43, at which we receive 0.39.
                    // In other words, to arrive at the value that we will receive, we either find the right match, or we look for the value that exceeds it and add 1 centime less
                    closeSellerReceives += 0.01m;
                    if (SellerReceivesToBuyerPays(closeSellerReceives) == paidMoney)
                    {
                        return closeSellerReceives;
                    }
                    if (SellerReceivesToBuyerPays(closeSellerReceives) > paidMoney)
                    {
                        return closeSellerReceives -= 0.01m;
                    } 
                }
            }

            // If CloseBuyerPays > PaidMoney we will have to reduce by 0.01 each time until we find the amount paid.
            if (closeBuyerPays > paidMoney)
            {
                while (true)
                {
                    closeSellerReceives -= 0.01m;

                    if (SellerReceivesToBuyerPays(closeSellerReceives) == paidMoney)
                    {
                        return closeSellerReceives;
                    }
                }
            }
            return 0;
        }

        public static decimal SellerReceivesToBuyerPays(decimal receivedMoney)
        {
            if (receivedMoney <= 0)
                return 0;

            receivedMoney = GetDecimalWithTwoDecimals(receivedMoney);

            decimal steamTaxAmount = Math.Max(GetDecimalWithTwoDecimals(receivedMoney * SteamTax), MinimumFeeAmount);
            decimal cs2TaxAmount = Math.Max(GetDecimalWithTwoDecimals(receivedMoney * CS2Tax), MinimumFeeAmount);

            return GetDecimalWithTwoDecimals(receivedMoney + steamTaxAmount + cs2TaxAmount);
        }

        public static decimal SellItemProfit(decimal buyPrice, decimal sellPrice)
        {
            return BuyerPaysToSellerReceives(sellPrice) - buyPrice;
        }

        public static decimal SellItemProfitPercentage(decimal buyPrice, decimal sellPrice)
        {
            // Profit/BuyPrice * 100 and BuyPrice must be higger than 0
            return buyPrice > 0 ? SellItemProfit(buyPrice, sellPrice) / buyPrice * 100 : 0;
        }
        internal static decimal GetDecimalWithTwoDecimals(decimal number)
        {
            // return only to 2 decimal places 
            return Math.Truncate(number * 100) / 100;
        }
    }
}
