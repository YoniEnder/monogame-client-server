﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameClient
{
    public class NetworkManagerClient
    {
        Socket _socket;
        byte[] _buffer = new byte[10000];
        PacketHandlerClient _packetHandler;
        float _timer_short = 0;
        float _timer_long = 0;
        bool _connect_again = false;
        PlayerManager _playerManager;
        Player _player;
        ushort packetType;
        PacketShort_Client _packet_short;
        public static bool _updateOtherPlayerTexture = false;
        public NetworkManagerClient(List<OtherPlayer> _other_players, Player player, PlayerManager playerManager)
        {
            _playerManager = playerManager;
            _player = player;
            _packetHandler = new PacketHandlerClient(_other_players, player, playerManager);
            _packet_short = new PacketShort_Client(_player);
        }
        public void Update(GameTime gameTime)
        {
            if (_socket.Connected)
            {
                _timer_short += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer_short >= 0.1f)
                {
                    _timer_short = 0;
                    _packet_short.UpdatePacket();
                    _socket.Send(_packet_short.Data());
                    packetType = _packet_short.ReadUShort();
                    if (packetType != 0)
                        Console.WriteLine("client: packet left Length: {0} | type: {1}", packetType, _packet_short.ReadUShort());
                }
                _timer_long += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer_long >= 1.5f)
                {
                    _timer_long = 0;
                    PacketLong_Client packet_long = new PacketLong_Client(_player);
                    _socket.Send(packet_long.Data());
                }
                _packetHandler.Update();
            }
            else if (_connect_again)
            {
                _connect_again = false;
                Initialize_connection();
            }
            if (NetworkManagerClient._updateOtherPlayerTexture)
            {
                _playerManager.updateOtherPlayerTexture();
            }
        }
        public void Initialize_connection()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.2.108"), 1994);
            _socket.BeginConnect(endPoint, ConnectCallBack, _socket);

        }
        private void ConnectCallBack(IAsyncResult result)
        {
            if (_socket.Connected)
            {
                Receive();
            }
            else
            {
                _connect_again = true;
            }
        }
        private void Receive()
        {
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceivedCallBack, null);
        }
        private void ReceivedCallBack(IAsyncResult result)
        {
            int buffer_size = _socket.EndReceive(result);
            _packetHandler.Handle(_buffer);
            Receive();
        }
    }
}