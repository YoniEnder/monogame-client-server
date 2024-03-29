﻿using GameClient.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
namespace GameClient
{
    public class TextInputBox
    {
        Texture2D _texture;
        SpriteFont _font;
        Color _background;
        Rectangle _rectangle;
        KeyboardState _keyboard;
        KeyboardState oldKeyboard;
        char key;
        ScreenPoint _refPoint;
        Vector2 _refPosition, _position;
        public string _text;
        bool _numbersOnly;
        Button _typeButton, _randomNameButton;
        static TextInputBox _currentBoxTyped;
        bool _allowRandomName;
        public TextInputBox(Vector2 refPosition, ScreenPoint refPoint, bool numbersOnly, int width = 250, bool clickToType = false, bool allowRandomName = false)
        {
            _numbersOnly = numbersOnly;
            _keyboard = Keyboard.GetState();
            _refPosition = refPosition;
            _refPoint = refPoint;
            _texture = GraphicManager.getRectangleTexture(width, 40, Color.White);
            ResetPositionToRefrence();
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, width, 40);
            _background = new Color(Color.White, 1f);
            _font = GraphicManager.GetBasicFont("basic_22");
            if (clickToType)
            {
                _typeButton = new Button(GraphicManager.getRectangleTexture(width, 40, Color.DarkMagenta), refPosition + new Vector2(0, -40), refPoint, Color.Green, Color.Gray, "enter text");
            }
            if (allowRandomName)
            {
                _randomNameButton = new Button(GraphicManager.getRectangleTexture(width, 40, Color.DarkMagenta), refPosition + new Vector2(0, 40), refPoint, Color.Green, Color.Gray, "random name");
            }
        }
        public void Update(GameTime gameTime)
        {
            if (_typeButton != null)
            {
                if (_typeButton.Update(gameTime))
                {
                    _currentBoxTyped = this;
                }
                if (_currentBoxTyped == this)
                {
                    ReadText();
                }
            }
            else
            {
                ReadText();
            }
            if (_randomNameButton != null)
            {
                if (_randomNameButton.Update(gameTime))
                {
                    _text = NameGenerator.GenerateRandomName();
                }
            }
        }
        public void ReadText()
        {
            _keyboard = Keyboard.GetState();
            bool back;
            if (_numbersOnly)
                ConvertKeyboardInputNumbers(_keyboard, oldKeyboard, out key, out back);
            else
                ConvertKeyboardInput(_keyboard, oldKeyboard, out key, out back);
            oldKeyboard = _keyboard;
            if (key != (char)0 && !back)
            {
                _text = _text + key;
            }
            else if (back && _text.Length > 0)
            {
                _text = _text.Remove(_text.Length - 1, 1);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_typeButton != null && _currentBoxTyped != this)
            {
                _typeButton.Draw(spriteBatch);
            }
            if (_randomNameButton != null)
            {
                _randomNameButton.Draw(spriteBatch);
            }
            spriteBatch.Draw(_texture, _rectangle, null, _background, 0, Vector2.Zero, SpriteEffects.None, 0.51f);
            if (!string.IsNullOrEmpty(_text))
            {
                spriteBatch.DrawString(_font, _text, _position + new Vector2(5, 5), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.6f);
            }
        }
        public static bool ConvertKeyboardInput(KeyboardState keyboard, KeyboardState oldKeyboard, out char key, out bool back)
        {
            back = false;
            Keys[] keys = keyboard.GetPressedKeys();
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            key = (char)0;
            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                switch (keys[0])
                {
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;
                    //Decimal keys
                    case Keys.D0: if (shift) { key = ')'; } else { key = '0'; } return true;
                    case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } return true;
                    case Keys.D2: if (shift) { key = '@'; } else { key = '2'; } return true;
                    case Keys.D3: if (shift) { key = '#'; } else { key = '3'; } return true;
                    case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } return true;
                    case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } return true;
                    case Keys.D6: if (shift) { key = '^'; } else { key = '6'; } return true;
                    case Keys.D7: if (shift) { key = '&'; } else { key = '7'; } return true;
                    case Keys.D8: if (shift) { key = '*'; } else { key = '8'; } return true;
                    case Keys.D9: if (shift) { key = '('; } else { key = '9'; } return true;
                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; return true;
                    case Keys.NumPad1: key = '1'; return true;
                    case Keys.NumPad2: key = '2'; return true;
                    case Keys.NumPad3: key = '3'; return true;
                    case Keys.NumPad4: key = '4'; return true;
                    case Keys.NumPad5: key = '5'; return true;
                    case Keys.NumPad6: key = '6'; return true;
                    case Keys.NumPad7: key = '7'; return true;
                    case Keys.NumPad8: key = '8'; return true;
                    case Keys.NumPad9: key = '9'; return true;
                    //Special keys
                    case Keys.OemTilde: if (shift) { key = '~'; } else { key = '`'; } return true;
                    case Keys.OemSemicolon: if (shift) { key = ':'; } else { key = ';'; } return true;
                    case Keys.OemQuotes: if (shift) { key = '"'; } else { key = '\''; } return true;
                    case Keys.OemQuestion: if (shift) { key = '?'; } else { key = '/'; } return true;
                    case Keys.OemPlus: if (shift) { key = '+'; } else { key = '='; } return true;
                    case Keys.OemPipe: if (shift) { key = '|'; } else { key = '\\'; } return true;
                    case Keys.OemPeriod: if (shift) { key = '>'; } else { key = '.'; } return true;
                    case Keys.OemOpenBrackets: if (shift) { key = '{'; } else { key = '['; } return true;
                    case Keys.OemCloseBrackets: if (shift) { key = '}'; } else { key = ']'; } return true;
                    case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; } return true;
                    case Keys.OemComma: if (shift) { key = '<'; } else { key = ','; } return true;
                    case Keys.Space: key = ' '; return true;
                    case Keys.Back: back = true; return true;
                }
            }
            return false;
        }
        public static bool ConvertKeyboardInputNumbers(KeyboardState keyboard, KeyboardState oldKeyboard, out char key, out bool back)
        {
            back = false;
            Keys[] keys = keyboard.GetPressedKeys();
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            key = (char)0;
            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                switch (keys[0])
                {
                    //Decimal keys
                    case Keys.D0: if (shift) { key = '0'; } else { key = '0'; } return true;
                    case Keys.D1: if (shift) { key = '1'; } else { key = '1'; } return true;
                    case Keys.D2: if (shift) { key = '2'; } else { key = '2'; } return true;
                    case Keys.D3: if (shift) { key = '3'; } else { key = '3'; } return true;
                    case Keys.D4: if (shift) { key = '4'; } else { key = '4'; } return true;
                    case Keys.D5: if (shift) { key = '5'; } else { key = '5'; } return true;
                    case Keys.D6: if (shift) { key = '6'; } else { key = '6'; } return true;
                    case Keys.D7: if (shift) { key = '7'; } else { key = '7'; } return true;
                    case Keys.D8: if (shift) { key = '8'; } else { key = '8'; } return true;
                    case Keys.D9: if (shift) { key = '9'; } else { key = '9'; } return true;
                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; return true;
                    case Keys.NumPad1: key = '1'; return true;
                    case Keys.NumPad2: key = '2'; return true;
                    case Keys.NumPad3: key = '3'; return true;
                    case Keys.NumPad4: key = '4'; return true;
                    case Keys.NumPad5: key = '5'; return true;
                    case Keys.NumPad6: key = '6'; return true;
                    case Keys.NumPad7: key = '7'; return true;
                    case Keys.NumPad8: key = '8'; return true;
                    case Keys.NumPad9: key = '9'; return true;
                    //Special keys
                    case Keys.Decimal: if (shift) { key = '.'; } else { key = '.'; } return true;
                    case Keys.OemPeriod: if (shift) { key = '.'; } else { key = '.'; } return true;
                    case Keys.Back: back = true; return true;
                }
            }
            return false;
        }
        public void ResetGraphics()
        {
            ResetPositionToRefrence();
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, 250, 40);
            if (_typeButton != null)
            {
                _typeButton.ResetGraphics();
            }
            if (_randomNameButton != null)
            {
                _randomNameButton.ResetGraphics();
            }
        }
        public void ResetPositionToRefrence()
        {
            _position = _refPosition + _refPoint.vector2;
        }
    }
}
