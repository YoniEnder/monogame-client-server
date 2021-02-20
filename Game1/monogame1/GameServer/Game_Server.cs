﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Net.Sockets;
using GameClient;
namespace GameServer
{
    public class Game_Server : Game
    {
        public static GraphicsDeviceManager _graphics;
        private List<NetworkPlayer> _networkPlayers;
        private List<Simple_Enemy> _enemies;
        private EnemyManager _enemyManager;
        private TileManager _tileManager;
        public PlayerManager _playerManager;
        public CollectionManager _collectionManager;
        public NetworkManagerServer _networkManager;
        private ItemManager _itemManager;
        private GraphicManager _graphicManager;
        private CollisionManager _collisionManager;
        private LevelManager _levelManager;
        private MapManager _mapManager;
        private PathFindingManager _pathFindingManager;
        static List<Socket> _socket_list = new List<Socket>();

        #region Important Functions
        public Game_Server()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //Window.AllowUserResizing = true;
            //Scene.SetDefaultDesignResolution(1280, 720, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            PlayerIndex.One.GetType();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _graphicManager = new GraphicManager(GraphicsDevice, Content,_graphics);

            _mapManager = new MapManager();
            _networkPlayers = new List<NetworkPlayer>();
            _enemies = new List<Simple_Enemy>();
            _collisionManager = new CollisionManager();
            _collectionManager = new CollectionManager(_enemies, Content);
            _itemManager = new ItemManager(_collectionManager);
            _playerManager = new PlayerManager(_networkPlayers, _collectionManager);
            _enemyManager = new EnemyManager(GraphicsDevice, _enemies, _collectionManager);
            _pathFindingManager = new PathFindingManager();
            _tileManager = new TileManager(GraphicsDevice, Content, _mapManager);
            _networkManager = new NetworkManagerServer(_socket_list, _networkPlayers, _enemies);
            _levelManager = new LevelManager(_tileManager);
            _collectionManager.Initialize(_playerManager, _itemManager);
            _collisionManager.Initialize(_networkPlayers, null, _enemies);
            _levelManager.Initialize(null,_networkPlayers);
            _mapManager.Initialize(null,_networkPlayers);
            _networkManager.Initialize_connection();

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _enemyManager.Update(gameTime);
            _networkManager.Update(gameTime);
            _playerManager.Update(gameTime);
            _mapManager.Update();
            _levelManager.Update();
            _pathFindingManager.Update();
            base.Update(gameTime);

        }
        #endregion
    }
}