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
        static private GameOverScreen _gameOverScreen;
        private Player _player;
        private List<SimpleEnemy> _enemies;
        private EnemyManager _enemyManager;
        private TileManager _tileManager;
        public PlayerManager _playerManager;
        public CollectionManager _collectionManager;
        public NetworkManagerClient _networkManager;
        private ItemManager _itemManager;
        private CollisionManager _collisionManager;
        public LevelManager _levelManager;
        private MapManager _mapManager;
        private PathFindingManager _pathFindingManager;
        private AudioManager _audioManager;
        static private InventoryManager _inventoryManager;
        static private MainMenuManager _menuManager;
        static private SettingsScreen _settingsScreen;
        private InGameUI _inGameUI;
        private ProgressManager _progressManager;
        private SettingsDataManager _settingsDataManager;
        private BulletReachManager _bulletReachManager;

        static public bool _inMenu = true;
        static public bool _isMultiplayer = false;
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
            //graphics
            new GraphicManager(GraphicsDevice, Content,_graphics);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _UIbatch = new SpriteBatch(GraphicsDevice);
            _settingsBatch = new SpriteBatch(GraphicsDevice);
            //menu and ui
            _menuManager = new MainMenuManager();
            _settingsScreen = new SettingsScreen();
            _inventoryManager = new InventoryManager(GraphicsDevice);
            _gameOverScreen = new GameOverScreen();
            _inGameUI = new InGameUI(GraphicsDevice);
            //game content
            _audioManager = new AudioManager(Content);
            _mapManager = new MapManager();
            _tileManager = new TileManager(GraphicsDevice, Content, _mapManager);
            _levelManager = new LevelManager(this,_tileManager);
            _collectionManager = new CollectionManager();
            _itemManager = new ItemManager(_collectionManager);
            //data from files
            _settingsDataManager = new SettingsDataManager();
            _progressManager = new ProgressManager();
            //players and enemies
            _networkPlayers = new List<NetworkPlayer>();
            _enemies = new List<SimpleEnemy>();
            _playerManager = new PlayerManager(GraphicsDevice,_networkPlayers, _collectionManager);
            _enemyManager = new EnemyManager(GraphicsDevice, _enemies, _collectionManager);
            //calculations
            _collisionManager = new CollisionManager();
            _pathFindingManager = new PathFindingManager();
            _bulletReachManager = new BulletReachManager();
            //network
            _networkManager = new NetworkManagerClient();
            //initializers
            _collectionManager.Initialize(_enemies, Content, _playerManager, _itemManager);
            _player = _playerManager.AddPlayer( _itemManager, _inventoryManager, _settingsScreen);
            _bulletReachManager.Initialize(_player, _networkPlayers);
            _collisionManager.Initialize(_networkPlayers, _player, _enemies);
            _levelManager.Initialize(_player,_progressManager);
            _inventoryManager.Initialize(_player,_itemManager);
            _mapManager.Initialize(_player);
            _settingsScreen.Initialize(this, Content, _inventoryManager, GraphicsDevice, _progressManager,_settingsDataManager);
            _progressManager.Initialize(_player,_inventoryManager, _playerManager, _levelManager, _collectionManager);
            _gameOverScreen.Initialize(this,Content, GraphicsDevice, _progressManager);
            _menuManager.Initialize(this, GraphicsDevice, _progressManager, _settingsDataManager);
            _networkManager.Initialize(_networkPlayers, _player, _playerManager, _enemies, _enemyManager, _inventoryManager, _levelManager, _menuManager._multiplayerMenu);
            _settingsDataManager.Initialize(_menuManager._characterSelectMenu, _menuManager._multiplayerMenu, _settingsScreen);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _settingsScreen.Update(gameTime);
            if (_inMenu && !SettingsScreen._showSettings)
            {
                _menuManager.Update(gameTime);
            }
            else
            {
                if (GameOverScreen._showScreen)
                {
                    Player player = null;
                    _gameOverScreen.Update(gameTime);
                    _player = player;
                }
                else
                {
                    if (_tileManager._levelLoaded)
                    {
                        _enemyManager.Update(gameTime);
                        _playerManager.Update(gameTime);
                        _mapManager.Update();
                        _levelManager.Update();
                        _bulletReachManager.Update();
                        _pathFindingManager.Update();
                        _inGameUI.Update();
                    }
                }
                if (_isMultiplayer)
                    _networkManager.Update(gameTime);
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
                _settingsScreen.Draw(_settingsBatch);
            }
            else
            {
                if (GameOverScreen._showScreen)
                {
                    _gameOverScreen.Draw(_UIbatch);
                }
                _settingsScreen.Draw(_settingsBatch);
                if (_tileManager._levelLoaded)
                {
                    _tileManager.Draw(_spriteBatch);
                    _playerManager.Draw(_spriteBatch);
                    _enemyManager.Draw(_spriteBatch);
                    _itemManager.Draw(_spriteBatch);
                    _inventoryManager.Draw(_UIbatch);
                    _mapManager.Draw(_UIbatch);
                    _inGameUI.Draw(_UIbatch);
                }
            }           
            _spriteBatch.End();
            _UIbatch.End();
            _settingsBatch.End();
            base.Draw(gameTime);
        }
        public void ResetGame(bool resetWholeGame = true)
        {
            if(resetWholeGame)
            {
                Game_Client._inMenu = true;
                SettingsScreen._showSettings = false;
                GameOverScreen._showScreen = false;
                _playerManager.Reset(true);
                if (!_isServer)
                {
                    AudioManager.PlaySong(menu: true);
                }
                _networkManager.CloseConnection();
                LevelManager._currentLevel = -1;
            }
            else
            {
                _playerManager.Reset(false);
            }
            ItemManager.Reset();
            EnemyManager.Reset();
            PathFindingManager.Reset();
            BulletReachManager.Reset();
            MapManager.ResetMap();
        }

        static public void ResetGraphics()
        {
            _inventoryManager.ResetGraphics();
            _settingsScreen.ResetGraphics();
            _menuManager.ResetGraphics();
            _gameOverScreen.ResetGraphics();
        }
        #endregion
    }
}
