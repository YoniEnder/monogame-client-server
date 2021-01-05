﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameClient
{
    public class Game_Client : Game
    {
        public static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<OtherPlayer> _other_players;
        private Player _player;
        private List<Simple_Enemy> _enemies;
        private EnemyManager _enemyManager;
        private TileManager _tileManager;
        private PlayerManager _playerManager;
        public CollectionManager _collectionManager;
        private NetworkManagerClient _networkManager;
        private InventoryManager _inventoryManager;
        private ItemManager _itemManager;
        private GraphicManager _graphicManager;
        #region Important Functions
        public Game_Client()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            PlayerIndex.One.GetType();
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //graphics.PreferredBackBufferWidth = 1000;  // set this value to the desired width of your window
            //graphics.PreferredBackBufferHeight = 1000;   // set this value to the desired height of your window
            _graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _graphicManager = new GraphicManager(GraphicsDevice,Content);
            _other_players = new List<OtherPlayer>();
            _enemies = new List<Simple_Enemy>();
            _collectionManager = new CollectionManager(_enemies, Content);
            _itemManager = new ItemManager(_collectionManager);
            _playerManager = new PlayerManager(_other_players, Content, GraphicsDevice, _collectionManager);
            _enemyManager = new EnemyManager(GraphicsDevice, _enemies,_collectionManager);
            _tileManager = new TileManager(GraphicsDevice, Content);
            _networkManager = new NetworkManagerClient(_other_players, _player, _playerManager);
            _networkManager.Initialize_connection();
            _tileManager.AddTowerTile(new Vector2(200,200));
            _inventoryManager = new InventoryManager(GraphicsDevice,_itemManager);
            _collectionManager.Initialize(_playerManager, _itemManager);
            _player = _playerManager.AddPlayer(_itemManager, _inventoryManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _enemies.RemoveAll(enemy => enemy._destroy == true);
            _enemyManager.Update(gameTime);
            _playerManager.Update(gameTime, _enemies);
            _networkManager.Update(gameTime);
            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack);

            _tileManager.Draw(_spriteBatch);
            _playerManager.Draw(_spriteBatch);
            _enemyManager.Draw(_spriteBatch);
            _inventoryManager.Draw(_spriteBatch);
            _itemManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}