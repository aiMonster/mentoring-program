using Binance.API.Csharp.Client;
using MentoringProgram.Common.Models;
using System.Collections.Generic;
using System.Linq;
using NativeBinanceClient = Binance.API.Csharp.Client.BinanceClient;

namespace MentoringProgram.MarketClients
{
    public class BinanceClient 
    {
        private readonly NativeBinanceClient _binanceClient;

        public List<TradingPair> _aviablePairs { get; }
        public string Name { get; }

        //TODO: Load keys from configuration manager
        public BinanceClient()
        {
            Name = "Binance market";

            var apiClient = new ApiClient("qjMHqLPUExOSMyFaiIJRPSTyOaYSLNAcbDqiUVi3xfmZfcoHdIAYS2J6Kmv2sIWF", "QJ49oIh2NF77hO7gyBTywIZ4h8QVbZbpk5ZRML4qXhCqIJX5hYn2Egp8IvpSzj0h");
            _binanceClient = new NativeBinanceClient(apiClient);

            var symbolPrices = _binanceClient.GetAllPrices().Result;           
        }
       
        public TradingPair GetPairByName(string name) =>       
            _aviablePairs.FirstOrDefault(p => p.Name == name);  
    }
}
