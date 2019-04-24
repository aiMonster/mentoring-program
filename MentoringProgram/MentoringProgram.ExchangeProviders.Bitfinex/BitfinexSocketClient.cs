using CryptoExchange.Net.Interfaces;
using System;
using SocketClient = Bitfinex.Net.BitfinexSocketClient;

namespace MentoringProgram.ExchangeProviders.Bitfinex
{
    internal class BitfinexSocketClient : SocketClient
    {
        public event Action OnDisconnected;

        protected override IWebsocket CreateSocket(string address)
        {
            var socket = base.CreateSocket(address);
            socket.OnClose += OnDisconnected;

            return socket;
        }
    }
}
