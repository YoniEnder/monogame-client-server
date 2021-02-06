﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
    public class LevelManager
    {
        private Player _player;
        private readonly TileManager _tileManager;
        Coord _coord_Player;
        Vector2 _spawnPoint;

        public LevelManager( TileManager tileManager)
        {
            this._tileManager = tileManager;
            _spawnPoint = _tileManager.LoadMap(11);
        }
        public void Initialize(Player player)
        {
            _player = player;
            _player._position = _spawnPoint;
        }
        public void Update()
        {
            _coord_Player = TileManager.GetCoordTile(_player.Position_Feet);
            if (_coord_Player.X + 2 >=TileManager._map.Width)
            {
                _player._position = _tileManager.LoadMap(10);
                EnemyManager.Reset();
            }
        }
        public void Draw()
        {

        }
    }
}
