using CryptoExchange.Net.Interfaces;
using System;
using SocketClient = Bittrex.Net.BittrexSocketClient;


namespace MentoringProgram.ExchangeProviders.Bittrex
{
    internal class BittrexSocketClient : SocketClient
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
