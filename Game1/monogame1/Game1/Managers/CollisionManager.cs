﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace GameClient
{
    public class CollisionManager
    {
        static private List<OtherPlayer> _other_players;
        static private Player _player;
        static private List<Simple_Enemy> _enemies;
        public CollisionManager()
        {
        }
        public void Initialize(List<OtherPlayer> other_players, Player player, List<Simple_Enemy> enemies)
        {
            _other_players = other_players;
            _player = player;
            _enemies = enemies;
        }
        //public Simple_Enemy isColidedWithEnemy()
        //{
            
        //}
        static public bool isColidedWithPlayer(Rectangle rectangle, int dmg)
        {

            if (rectangle.X > _player.Rectangle.X && rectangle.X < _player.Rectangle.X + _player.Rectangle.Width)
                if (rectangle.Y > _player.Rectangle.Y && rectangle.Y < _player.Rectangle.Y + _player.Rectangle.Height)
                {
                    _player._health._health_left -= dmg;
                    //_player.DealDamage(dmg);
                    return true;
                }


            return false;
        }
        static public bool isColidedWithEnemies(Rectangle rectangle, int dmg)
        {
            foreach (var enemy in _enemies)
            {
                if (rectangle.X > enemy.Rectangle.X && rectangle.X < enemy.Rectangle.X + enemy.Rectangle.Width)
                    if (rectangle.Y > enemy.Rectangle.Y && rectangle.Y < enemy.Rectangle.Y + enemy.Rectangle.Height)
                    {
                        enemy.dealDamage(dmg);
                        //if (_player._health._health_left <= 0)
                        //{
                        //    _player._destroy = true;
                        //}
                        return true;
                    }
            }
            return false;
        }
        static public bool isMouseCollidingRectangle(Rectangle rectangle)
        {
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (mouse.X > rectangle.X && mouse.X < rectangle.X + rectangle.Width)
                if (mouse.Y > rectangle.Y && mouse.Y < rectangle.Y + rectangle.Height)
                    return true;
            return false;
        }
    }   
}