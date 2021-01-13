﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
namespace GameClient
{
    public enum Direction { Up = 0, Down, Left, Right };
    public class Simple_Enemy
    {
        int _id;
        private MeleeWeapon _meleeWeapon;
        private Gun _gun;
        private AnimationManager _animationManager;
        private PlayerManager _playerManager;
        public HealthManager _health;
        private ItemManager _itemManager;
        private Vector2 _velocity;
        public Vector2 _position;
        private Vector2 _shootingDirection;
        public bool _destroy = false;
        private bool _hide_weapon;
        private bool stopMoving;
        private bool _isStopingToShot=false;
        private int[] _items_drop_list;
        private int _moving_direction;
        private int _width;
        private int _height;
        private float _speed;
        private float _scale;
        private float _sniperTimer = 0;
        private float _sniperStopTime = 1.5f;
        private float _movingBetweenShotsTime = 3f;
        private float _movingToPlayerMaxDistance = 40;
        public Vector2 Position_Feet { get => _position + new Vector2(_width / 2, _height * 2 / 3); }
        public Rectangle Rectangle { get => new Rectangle((int)_position.X, (int)_position.Y, (int)(_width * _scale), (int)(_height * _scale)); }

        public Simple_Enemy(AnimationManager animationManager, int id, Vector2 position, float speed, PlayerManager playerManager, ItemManager itemManager, int health, int[] items_drop_list, MeleeWeapon meleeWeapon, Gun gun)
        {
            _id = id;
            _animationManager = animationManager;
            _position = position;
            _animationManager._position = _position;
            _playerManager = playerManager;
            _items_drop_list = items_drop_list;
            _itemManager = itemManager;
            _speed = speed;
            _meleeWeapon = meleeWeapon;
            _scale = animationManager._scale;
            if (meleeWeapon != null)
                _meleeWeapon._holderScale = _scale;
            _health = new HealthManager(health, position + new Vector2(8, 10), _scale);
            _width = _animationManager.Animation._frameWidth;
            _height = _animationManager.Animation._frameHeight;
            _gun = gun;
            if (gun != null)
            {
                _movingToPlayerMaxDistance = Math.Min(_gun._bullet._maxTravelDistance - 30, 500);
                _gun._holderScale = _scale;
            }
        }
        public void Update(GameTime gameTime)
        {
            Move();

            SetAnimations();

            _velocity = _velocity * _speed;

            _animationManager.Update(gameTime, _position);

            if (_meleeWeapon != null)
            {
                _meleeWeapon.Update(_moving_direction, gameTime, _position);
                if (!_meleeWeapon._swing_weapon)
                    _position += _velocity;
            }

            _health.Update(_position);
            if (_gun != null)
            {
                _sniperTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_gun._isSniper)
                {
                    if (_isStopingToShot)
                    {
                        _gun.Update(gameTime, _shootingDirection, false, true);
                        if (_sniperTimer >= _sniperStopTime)
                        {
                            _sniperTimer = 0;
                            _gun.Shot();
                            _isStopingToShot = false;
                        }
                    }
                    else
                    {
                        _gun.Update(gameTime, _shootingDirection, false, false);
                        _position += _velocity;
                        if (_sniperTimer >= _movingBetweenShotsTime)
                        {
                            _sniperTimer = 0;
                            _isStopingToShot = true;
                        }
                    }
                }
                else
                {
                    _position += _velocity;
                    _gun.Update(gameTime, _shootingDirection, false, false);
                    if (stopMoving)
                        _gun.Shot();
                }

            }
            _velocity = Vector2.Zero;

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            _animationManager.Draw(spriteBatch, TileManager.GetLayerDepth(_position.Y));
            _health.Draw(spriteBatch, TileManager.GetLayerDepth(_position.Y));
            if (_hide_weapon)
            {
                if (_meleeWeapon != null)
                    _meleeWeapon.Draw(spriteBatch, TileManager.GetLayerDepth(_position.Y) - 0.01f);
                if (_gun != null)
                    _gun.Draw(spriteBatch, _position, TileManager.GetLayerDepth(_position.Y) - 0.01f);
            }
            else
            {
                if (_meleeWeapon != null)
                    _meleeWeapon.Draw(spriteBatch, TileManager.GetLayerDepth(_position.Y) + 0.01f);
                if (_gun != null)
                    _gun.Draw(spriteBatch, _position, TileManager.GetLayerDepth(_position.Y) + 0.01f);
            }
        }
        public Simple_Enemy Copy(float scale, Gun gun, MeleeWeapon meleeWeapon)
        {

            return new Simple_Enemy(_animationManager.Copy(scale), _id, _position, _speed, _playerManager, _itemManager, _health._total_health, _items_drop_list, meleeWeapon, gun);
        }

        public void Move()
        {
            Vector2 closest_player = _playerManager.getClosestPlayerToPosition(Position_Feet);
            if(!_isStopingToShot)
                _shootingDirection = closest_player - _position;
            if (Vector2.Distance(closest_player, Position_Feet) > _movingToPlayerMaxDistance)
            {
                stopMoving = false;
            }
            else
            {
                stopMoving = true;
                if(_meleeWeapon!=null)
                    _meleeWeapon.SwingWeapon();
            }
            _velocity = Vector2.Normalize(closest_player - Position_Feet);
            if (_velocity.X > Math.Abs(_velocity.Y))
            {
                _hide_weapon = false;
                _moving_direction = (int)Direction.Right;
            }
            else if (-_velocity.X > Math.Abs(_velocity.Y))
            {
                _hide_weapon = false;
                _moving_direction = (int)Direction.Left;
            }
            else if (_velocity.Y > 0)
            {
                _hide_weapon = false;
                _moving_direction = (int)Direction.Down;
            }
            else if (_velocity.Y < 0)
            {
                _hide_weapon = true;
                _moving_direction = (int)Direction.Up;
            }
        }
        protected void SetAnimations()
        {
            if (stopMoving)
            {
                if (_meleeWeapon != null)
                {
                    if (!_meleeWeapon._swing_weapon)
                        _animationManager.Animation = _animationManager._animations[_moving_direction];
                }
                else
                {
                    _animationManager.Animation = _animationManager._animations[_moving_direction];
                }
                _velocity = new Vector2(0, 0);
                _animationManager.Stop();
            }
            if (!stopMoving)
            {
                _animationManager.Play(_moving_direction);
            }

        }

        public void dealDamage(int dmg)
        {
            _health._health_left -= dmg;
            if (_health._health_left <= 0)
            {
                _destroy = true;
                _itemManager.DropItem(_items_drop_list, _position);
            }
        }

    }
}
