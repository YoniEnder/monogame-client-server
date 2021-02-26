﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameClient
{
    public class MenuManager
    {
        Button _singlePlayer, _multiPlayerButton, _exit, _howToPlay,_highScores;
        int _buttonHeight = 30;
        int _buttonWidth = 100;
        Vector2 _buttonPosition;
        Game_Client _game_client;
        Texture2D _menuBackgroundImage;
        GraphicsDevice _graphicsDevice;
        CharacterSelectMenu _characterSelectMenu;
        MultiplayerMenu _multiplayerMenu;
        public bool _showChooseCharacterMenu, _showMultiplayerMenu;
        public static bool _connected;
        public MenuManager(Game_Client game_client, GraphicsDevice graphicsDevice)
        {
            _multiplayerMenu = new MultiplayerMenu(graphicsDevice, game_client, this);
            _characterSelectMenu = new CharacterSelectMenu(graphicsDevice, game_client,this);
            _graphicsDevice = graphicsDevice;
            _game_client = game_client;
            _menuBackgroundImage = GraphicManager.getImage("backgroundUnboxingrave");
            _buttonPosition = new Vector2(7 * (graphicsDevice.Viewport.Bounds.Width / 10), graphicsDevice.Viewport.Bounds.Height / 2);
            _singlePlayer = new Button(GraphicManager.getRectangleTexture(_buttonWidth, _buttonHeight, Color.White),GraphicManager.GetBasicFont(), _buttonPosition, Color.Green,Color.Gray,"Single player");
            _multiPlayerButton = new Button(GraphicManager.getRectangleTexture(_buttonWidth, _buttonHeight, Color.White), GraphicManager.GetBasicFont(),_buttonPosition + new Vector2(0,_buttonHeight +2), Color.Green, Color.Gray, "Multiplayer");
            _howToPlay = new Button(GraphicManager.getRectangleTexture(_buttonWidth, _buttonHeight, Color.White), GraphicManager.GetBasicFont(), _buttonPosition + new Vector2(0, _buttonHeight * 2 + 4), Color.Green, Color.Gray, "How to play");
            _highScores = new Button(GraphicManager.getRectangleTexture(_buttonWidth, _buttonHeight, Color.White), GraphicManager.GetBasicFont(), _buttonPosition + new Vector2(0, _buttonHeight * 3 + 6), Color.Green, Color.Gray, "Scores");
            _exit = new Button(GraphicManager.getRectangleTexture(_buttonWidth, _buttonHeight, Color.White), GraphicManager.GetBasicFont(), _buttonPosition + new Vector2(0, _buttonHeight*4 + 8), Color.Green, Color.Gray, "Exit game");
        }
        public void Update(GameTime gameTime)
        {
            
            if(_showChooseCharacterMenu)
            {
                _characterSelectMenu.Update(gameTime);
            }
            else if(_showMultiplayerMenu)
            {
                _multiplayerMenu.Update(gameTime);
            }
            else
            {
                if (!UIManager._showSettings)
                {
                    if (_singlePlayer.Update(gameTime))
                    {
                        //_game_client._inMenu = false;
                        _showChooseCharacterMenu = true;
                    }
                    if (_multiPlayerButton.Update(gameTime))
                    {
                        _showMultiplayerMenu = true;
                    }
                    if (_howToPlay.Update(gameTime))
                    {
                        //Game_Client._inMenu = false;
                    }
                    if (_highScores.Update(gameTime))
                    {
                        //Game_Client._inMenu = false;
                    }
                    if (_exit.Update(gameTime))
                    {
                        //Game_Client._inMenu = false;
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_menuBackgroundImage, new Rectangle(0, 0, GraphicManager.screenWidth, GraphicManager.screenHeight), Color.White);

            if (_showChooseCharacterMenu)
            {
                _characterSelectMenu.Draw(spriteBatch);
            }
            else if (_showMultiplayerMenu)
            {
                _multiplayerMenu.Draw(spriteBatch);
            }
            else
            {
                _singlePlayer.Draw(spriteBatch);
                _multiPlayerButton.Draw(spriteBatch);
                _howToPlay.Draw(spriteBatch);
                _highScores.Draw(spriteBatch);
                _exit.Draw(spriteBatch);
            }
        }
        public void ResetGraphics()
        {
            _buttonPosition = new Vector2(7 * (_graphicsDevice.Viewport.Bounds.Width / 10), _graphicsDevice.Viewport.Bounds.Height / 2);
            _singlePlayer.ResetGraphics(_buttonPosition);
            _multiPlayerButton.ResetGraphics(_buttonPosition + new Vector2(0, _buttonHeight + 2));
            _howToPlay.ResetGraphics(_buttonPosition + new Vector2(0, _buttonHeight * 2 + 4));
            _highScores.ResetGraphics(_buttonPosition + new Vector2(0, _buttonHeight * 3 + 6));
            _exit.ResetGraphics(_buttonPosition + new Vector2(0, _buttonHeight * 4 + 8));
        }
    }
}
