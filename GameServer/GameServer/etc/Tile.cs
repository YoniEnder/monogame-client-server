﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Tile
    {
        //private Texture2D _texture;

        private Vector2 _position;

        private int _width;

        private int _height;

        public Tile(int width,int height,Vector2 position)
        {
            _width = width;
            _height = height;
            _position = position;
            //_texture = texture;
        }
        //public void Draw(SpriteBatch spriteBatch)
        //{
        //    spriteBatch.Draw(_texture, _position, null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        //}
    }
}
