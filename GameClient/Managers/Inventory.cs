﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace GameClient
{
    public class Inventory
    {
        ItemManager _itemManager;
        Player _player;
        Texture2D _inventoryBlockNormal, _inventoryBlockSelected;
        GraphicsDevice _graphicsDevice;
        Vector2 _position;
        public static SpriteFont _font;
        public (Rectangle, ItemStock)[] _inventory_rectangles;
        int width = 55;
        int height = 35;
        int _itemBlockAmount = 8;
        MouseState _previousMouse, _currentMouse;
        public Item EquippedGun = null;
        int _gamePadPointer = 0;
        public int _gold = 0;
        bool _usingGamePad;
        public Inventory(GraphicsDevice graphicsDevice, ItemManager itemManager)
        {
            _graphicsDevice = graphicsDevice;
            _itemManager = itemManager;
            SetInventoryTextures();
            Vector2 fixedPosition = GetInventoryPosition();
            Rectangle Dest_rectangle;
            _inventory_rectangles = new (Rectangle, ItemStock)[_itemBlockAmount];
            for (int i = 0; i < _itemBlockAmount; i++)
            {
                Dest_rectangle = new Rectangle((int)fixedPosition.X + width * i + i, (int)fixedPosition.Y, width, height);
                _inventory_rectangles[i] = ((Dest_rectangle, null));
            }
        }
        public void SetInventoryTextures()
        {
            _inventoryBlockSelected = new Texture2D(_graphicsDevice, width, height);
            Color[] data1 = new Color[width * height];
            for (int i = 0; i < data1.Length; ++i)
            {
                if (i >= data1.Length - width || i <= width || i % width == 0 || (i + 1) % width == 0)
                    data1[i] = Color.AntiqueWhite;
                else
                    data1[i] = Color.Blue;
            }
            _inventoryBlockSelected.SetData(data1);
            _inventoryBlockNormal = new Texture2D(_graphicsDevice, width, height);
            Color[] data2 = new Color[width * height];
            for (int i = 0; i < data2.Length; ++i)
            {
                if (i >= data2.Length - width || i <= width || i % width == 0 || (i + 1) % width == 0)
                    data2[i] = Color.White;
                else
                    data2[i] = Color.SaddleBrown;
            }
            _inventoryBlockNormal.SetData(data2);
        }
        public Vector2 GetInventoryPosition()
        {
            _position = new Vector2((_graphicsDevice.Viewport.Bounds.Width / 2),
            _graphicsDevice.Viewport.Bounds.Height - (_graphicsDevice.Viewport.Bounds.Height / 20));
            return new Vector2(_position.X - _itemBlockAmount / 2 * width - (_itemBlockAmount / 2 - 1), _position.Y);
        }
        public void Initialize(Player player)
        {
            _player = player;
        }
        public void Update()
        {
            MouseClick();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 text_pos;
            for (int i = 0; i < _itemBlockAmount; i++)
            {
                if (_player._input._isGamePad && i == _gamePadPointer)
                {
                    spriteBatch.Draw(_inventoryBlockSelected, _inventory_rectangles[i].Item1, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0.98f);
                }
                else
                {
                    spriteBatch.Draw(_inventoryBlockNormal, _inventory_rectangles[i].Item1, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0.98f);
                }
            }
            foreach (var tuple in _inventory_rectangles)
            {
                if (tuple.Item2 != null)
                {
                    tuple.Item2._item.DrawInventory(spriteBatch, new Vector2(tuple.Item1.X, tuple.Item1.Y));
                    if (tuple.Item2._amount > 1)
                    {
                        if (tuple.Item2._amount > 9)
                            text_pos = new Vector2(tuple.Item1.X, tuple.Item1.Y) + new Vector2(width - 19, height - 17);
                        else
                            text_pos = new Vector2(tuple.Item1.X, tuple.Item1.Y) + new Vector2(width - 9, height - 17);
                        spriteBatch.DrawString(_font, tuple.Item2._amount.ToString(), text_pos, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0.991f);
                    }
                }
            }
        }
        public void ResetGraphics()
        {
            Vector2 fixedPosition = GetInventoryPosition();
            for (int i = 0; i < _inventory_rectangles.Length; i++)
            {
                _inventory_rectangles[i].Item1 = new Rectangle((int)fixedPosition.X + width * i + i, (int)fixedPosition.Y, width, height);
            }
        }
        //used for removing an item from the inventory automaticly, example: key when opening door
        public bool RemoveItemFromInventory(int itemID)
        {
            int index = 0;
            bool foundItem = false;
            foreach (var tuple in _inventory_rectangles)
            {
                ItemStock itemStock = tuple.Item2;
                if (itemStock != null)
                {
                    if (itemStock._item._itemId == itemID)
                    {
                        foundItem = true;
                        break;
                    }
                }
                index++;
            }
            if (foundItem)
            {
                if (--_inventory_rectangles[index].Item2._amount == 0)
                {
                    _inventory_rectangles[index].Item2 = null;
                }
                return true;
            }
            return false;
        }
        public void AddItemToInventory(Item itemToAdd, bool sound = true, int amount = 1)
        {
            if (itemToAdd._itemId == 10)
            {
                _gold += amount;
                DmgMassageManager.CreateDmgMessage(amount, itemToAdd._position, Color.Gold);
                _itemManager.RemoveItemFromFloor(itemToAdd);
                if (sound)
                {
                    AudioManager.PlaySound("PickingItem");
                }
                return;
            }
            int index = 0;
            foreach (var tuple in _inventory_rectangles)
            {
                ItemStock itemStock = tuple.Item2;
                if (itemStock != null)
                {
                    if (itemStock._item._itemId == itemToAdd._itemId)
                    {
                        if (itemStock._amount < tuple.Item2._item._invenotryAmountAllowed)
                        {
                            itemStock._amount += amount;
                            _itemManager.RemoveItemFromFloor(itemToAdd);
                            if (sound)
                            {
                                AudioManager.PlaySound("PickingItem");
                            }
                            return;
                        }
                    }
                }
            }
            foreach (var tuple in _inventory_rectangles)
            {
                if (tuple.Item2 == null)
                {
                    _itemManager.RemoveItemFromFloor(itemToAdd);
                    itemToAdd.MakeInventoryItem(new Vector2(tuple.Item1.X, tuple.Item1.Y));
                    ItemStock itemStock = new ItemStock(amount, itemToAdd);
                    _inventory_rectangles[index] = (tuple.Item1, itemStock);
                    if (sound)
                    {
                        AudioManager.PlaySound("PickingItem");
                    }
                    return;
                }
                index++;
            }
        }
        public void ResetInventory()
        {
            for (int i = 0; i < _itemBlockAmount; i++)
            {
                _inventory_rectangles[i].Item2 = null;
            }
            _gold = 0;
        }
        public void MouseClick()
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            MouseLeftClick();
            MouseRightClick();
        }
        public bool MouseLeftClick()
        {
            for (int i = 0; i < _itemBlockAmount; i++)
            {
                if (CollisionManager.isMouseCollidingRectangle(_inventory_rectangles[i].Item1))
                {
                    Player._mouseIntersectsUI = true;
                    if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                    {
                        if (_inventory_rectangles[i].Item2 != null)
                        {
                            Gun gun = _inventory_rectangles[i].Item2._item._gun;
                            if (gun != null)
                            {
                                if (_player._gun != null)
                                    _player._gun._bullets.Clear();
                                _player.EquipGun(gun);
                                Item EquippedGunTemp = _inventory_rectangles[i].Item2._item;
                                _inventory_rectangles[i].Item2 = null;
                                if (EquippedGun != null)
                                    AddItemToInventory(EquippedGun, false);
                                AudioManager.PlaySound("SwitchingWeapon");
                                EquippedGun = EquippedGunTemp;
                            }
                            else if (_inventory_rectangles[i].Item2._item._itemHealing > 0)
                            {
                                AudioManager.PlaySound("UsingPotion");
                                int heal = _inventory_rectangles[i].Item2._item._itemHealing;
                                DmgMassageManager.CreateDmgMessage(heal, _player.Position_Head + new Vector2(5, 0), Color.LightGreen);
                                _player._health._health_left += heal;
                                if (--_inventory_rectangles[i].Item2._amount == 0)
                                {
                                    _inventory_rectangles[i].Item2 = null;
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public bool MouseRightClick()
        {
            for (int i = 0; i < _itemBlockAmount; i++)
            {
                if (CollisionManager.isMouseCollidingRectangle(_inventory_rectangles[i].Item1))
                {
                    Player._mouseIntersectsUI = true;
                    if (_currentMouse.RightButton == ButtonState.Released && _previousMouse.RightButton == ButtonState.Pressed)
                    {
                        if (_inventory_rectangles[i].Item2 != null)
                        {
                            if (_inventory_rectangles[i].Item2._amount > 0)
                            {
                                AudioManager.PlaySound("DroppingItem");
                                ItemManager.DropItem(_inventory_rectangles[i].Item2._item._itemId, _player.Position_Feet, true);
                                if (--_inventory_rectangles[i].Item2._amount == 0)
                                {
                                    _inventory_rectangles[i].Item2 = null;
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public void MoveInventoryPointerRight()
        {
            _gamePadPointer++;
            if (_gamePadPointer >= _itemBlockAmount)
            {
                _gamePadPointer = 0;
            }
        }
        public void MoveInventoryPointerLeft()
        {
            _gamePadPointer--;
            if (_gamePadPointer < 0)
            {
                _gamePadPointer = _itemBlockAmount - 1;
            }
        }
        public void DropInventoryItemGamePad()
        {
            if (_inventory_rectangles[_gamePadPointer].Item2 != null)
            {
                if (_inventory_rectangles[_gamePadPointer].Item2._amount > 0)
                {
                    AudioManager.PlaySound("DroppingItem");
                    ItemManager.DropItem(_inventory_rectangles[_gamePadPointer].Item2._item._itemId, _player.Position_Feet, true);
                    if (--_inventory_rectangles[_gamePadPointer].Item2._amount == 0)
                    {
                        _inventory_rectangles[_gamePadPointer].Item2 = null;
                    }
                }
            }
        }
        public bool HasSpaceForItem(Item itemToAdd)
        {
            return true;
            foreach (var tuple in _inventory_rectangles)
            {
                ItemStock itemStock = tuple.Item2;
                if (itemStock != null)
                {
                    if (itemStock._item._itemId == itemToAdd._itemId)
                    {
                        if (itemStock._amount < tuple.Item2._item._invenotryAmountAllowed)
                        {
                            return true;
                        }
                    }
                }
            }
            foreach (var tuple in _inventory_rectangles)
            {
                if (tuple.Item2 == null)
                {
                    return true;
                }
            }
            return false;
        }
        public void ClickInventoryItemGamePad()
        {
            if (_inventory_rectangles[_gamePadPointer].Item2 != null)
            {
                Gun gun = _inventory_rectangles[_gamePadPointer].Item2._item._gun;
                if (gun != null)
                {
                    if (_player._gun != null)
                        _player._gun._bullets.Clear();
                    _player.EquipGun(gun);
                    Item EquippedGunTemp = _inventory_rectangles[_gamePadPointer].Item2._item;
                    _inventory_rectangles[_gamePadPointer].Item2 = null;
                    if (EquippedGun != null)
                        AddItemToInventory(EquippedGun, false);
                    AudioManager.PlaySound("SwitchingWeapon");
                    EquippedGun = EquippedGunTemp;
                }
                else if (_inventory_rectangles[_gamePadPointer].Item2._item._itemHealing > 0)
                {
                    int heal = _inventory_rectangles[_gamePadPointer].Item2._item._itemHealing;
                    DmgMassageManager.CreateDmgMessage(heal, _player.Position_Head + new Vector2(5, 0), Color.LightGreen);
                    _player._health._health_left += heal;
                    AudioManager.PlaySound("UsingPotion");
                    if (--_inventory_rectangles[_gamePadPointer].Item2._amount == 0)
                    {
                        _inventory_rectangles[_gamePadPointer].Item2 = null;
                    }
                }
            }
        }
    }
}
