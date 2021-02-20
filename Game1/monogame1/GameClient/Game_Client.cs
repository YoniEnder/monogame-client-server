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
        private SpriteBatch _UIbatch;
        private SpriteBatch _settingsBatch;
        private List<NetworkPlayer> _networkPlayers;
        private Player _player;
        private List<Simple_Enemy> _enemies;
        private EnemyManager _enemyManager;
        private TileManager _tileManager;
        public PlayerManager _playerManager;
        public CollectionManager _collectionManager;
        public NetworkManagerClient _networkManager;
        private ItemManager _itemManager;
        private CollisionManager _collisionManager;
        private LevelManager _levelManager;
        private MapManager _mapManager;
        private PathFindingManager _pathFindingManager;
        static private InventoryManager _inventoryManager;
        static private MenuManager _menuManager;
        static private UIManager _UIManager;
        static public bool _inMenu = true;
        static public bool _IsMultiplayer = false;
        static public bool _isServer = true;

        #region Important Functions
        public Game_Client()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //Window.AllowUserResizing = true;
            //Scene.SetDefaultDesignResolution(1280, 720, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            PlayerIndex.One.GetType();
            _isServer = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            _graphics.ApplyChanges();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            new GraphicManager(GraphicsDevice, Content,_graphics);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _UIbatch = new SpriteBatch(GraphicsDevice);
            _settingsBatch = new SpriteBatch(GraphicsDevice);
            _mapManager = new MapManager();
            _menuManager = new MenuManager(this, GraphicsDevice);
            _networkPlayers = new List<NetworkPlayer>();
            _enemies = new List<Simple_Enemy>();
            _collisionManager = new CollisionManager();
            _collectionManager = new CollectionManager(_enemies, Content);
            _itemManager = new ItemManager(_collectionManager);
            _inventoryManager = new InventoryManager(GraphicsDevice, _itemManager);
            _UIManager = new UIManager();
            _playerManager = new PlayerManager(_networkPlayers, _collectionManager);
            _enemyManager = new EnemyManager(GraphicsDevice, _enemies, _collectionManager);
            _pathFindingManager = new PathFindingManager();
            _tileManager = new TileManager(GraphicsDevice, Content, _mapManager);
            _networkManager = new NetworkManagerClient();
            _levelManager = new LevelManager(_tileManager);
            _collectionManager.Initialize(_playerManager, _itemManager);
            _player = _playerManager.AddPlayer(_itemManager, _inventoryManager, GraphicsDevice, _UIManager);
            _collisionManager.Initialize(_networkPlayers, _player, _enemies);
            _levelManager.Initialize(_player,null);
            _inventoryManager.Initialize(_player);
            _mapManager.Initialize(_player,null);
            _UIManager.Initialize(Content, _inventoryManager, _graphics, _player);
            _networkManager.Initialize(_networkPlayers, _player, _playerManager, _enemies, _enemyManager);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (_inMenu)
            {
                _menuManager.Update(gameTime);
                _UIManager.Update(gameTime);
            }
            else
            {
                _enemyManager.Update(gameTime);
                
                if(_IsMultiplayer)
                    _networkManager.Update(gameTime);
                _UIManager.Update(gameTime);
                _playerManager.Update(gameTime);
                _mapManager.Update();
                _levelManager.Update();
                _pathFindingManager.Update();
            }
            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            
            _UIbatch.Begin(SpriteSortMode.FrontToBack);
            _spriteBatch.Begin(SpriteSortMode.FrontToBack,transformMatrix: GraphicManager.GetSpriteBatchMatrix());
            _settingsBatch.Begin(SpriteSortMode.FrontToBack);
            if (_inMenu)
            {
                _menuManager.Draw(_UIbatch);
                _UIManager.Draw(_settingsBatch);
            }
            else
            {
                _UIManager.Draw(_settingsBatch);
                _tileManager.Draw(_spriteBatch);
                _playerManager.Draw(_spriteBatch);
                _enemyManager.Draw(_spriteBatch);
                _itemManager.Draw(_spriteBatch);
                _inventoryManager.Draw(_UIbatch);
            }
            
            _spriteBatch.End();
            _UIbatch.End();
            _settingsBatch.End();
            base.Draw(gameTime);

        }

        static public void ResetGraphics()
        {
            _inventoryManager.ResetGraphics();
            _UIManager.ResetGraphics();
            _menuManager.ResetGraphics();
        }
        #endregion
    }
}