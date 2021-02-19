﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using TiledSharp;
namespace GameClient
{
    public class TileManager
    {
        static private GraphicsDevice _graphicDevice;
        private ContentManager _contentManager;
        private MapManager _mapManager;
        static List<TileSet> _tileSets;
        public static TmxMap _map;
        public static List<Rectangle> _walls;
        public Grid _grid = PathFinder.s_grid;

        public TileManager(GraphicsDevice graphicDevice, ContentManager contentManager, MapManager mapManager)
        {
            _graphicDevice = graphicDevice;
            _contentManager = contentManager;
            _mapManager = mapManager;
            _tileSets = new List<TileSet>();
        }
        public Vector2 LoadMap(int mapNum)
        {
            _walls = new List<Rectangle>();

            string mapName = Directory.GetCurrentDirectory() + "/Content/maps/" + "map" + mapNum.ToString() + ".tmx";
            _map = new TmxMap(mapName);
            int tilesetIndex = 0;

            Vector2 spawnPoint = Vector2.Zero;

            for (int i = 0; i < _map.Tilesets.Count; i++)
            {
                _tileSets.Add(new TileSet(_contentManager.Load<Texture2D>("maps/" + _map.Tilesets[i].Name.ToString()),
                _map.Tilesets[i].TileWidth, _map.Tilesets[i].TileHeight));
            }
            _grid = new Grid(_map.Width,_map.Height);

            for (int i = 0; i < _map.TileLayers[1].Tiles.Count; i++)
            {
                InitializeGid(i,1,ref spawnPoint);

            }
            for (int i = 0; i < _map.TileLayers[2].Tiles.Count; i++)
            {
                InitializeGid(i, 2, ref spawnPoint);

            }

            PathFinder.UpdateGrid(_grid);
            return spawnPoint;

        }
        public void InitializeGid(int i, int tilesetIndex,ref Vector2 spawnPoint)
        {
            int gid = _map.TileLayers[tilesetIndex].Tiles[i].Gid;
            if (gid != 0)
            {

                if (_map.Tilesets[tilesetIndex].FirstGid > 0)
                {
                    if (gid == 325)//grave normal
                    {
                        Rectangle rectangle = AddWall(i);
                        if(!Game_Client._IsMultiplayer)
                            _mapManager._graves.Add(new Grave(rectangle, false));
                    }
                    else if (gid == 326)//grave broken
                    {
                        Rectangle rectangle = AddWall(i);
                        if (!Game_Client._IsMultiplayer)
                            _mapManager._graves.Add(new Grave(rectangle, true));
                    }
                    else if (gid == 134)//spawn point
                    {
                        spawnPoint = GetPositionFromCoord(i % _map.Width, i / _map.Width);
                    }
                    else if (gid == 469 || gid == 467)//normal chest
                    {
                        if (!Game_Client._IsMultiplayer)
                            MapManager._chests.Add(new Chest(GetRectangleFromCoord(i % _map.Width, i / _map.Width,2), i, tilesetIndex));
                    }
                    else if (gid == 468 ||gid == 465)
                    {
                        MapManager._boxes.Add(new Box(GetRectangleFromCoord(i % _map.Width, i / _map.Width,1.5f), i, tilesetIndex));
                        AddWall(i,1.5f);
                    }
                    else//normal walls
                    {
                        AddWall(i);
                    }
                }
            }
        }
        public bool ManageGid(SpriteBatch spriteBatch,int i,int gid,int tileLayer)
        {
            if (gid == 468 || gid == 465)
            {
                gid = gid - _map.Tilesets[2].FirstGid + 1;
                DrawTile(gid, _tileSets[2], spriteBatch, i, 0.01f * tileLayer, 1.5f);
                return true;
            }
                if (gid == 469 || gid == 467)//normal chest
            {
                gid = gid - _map.Tilesets[2].FirstGid + 1;
                DrawTile(gid, _tileSets[2], spriteBatch, i, 0.01f * tileLayer, 2);
                return true;
            }
            return false;
        }
        public Rectangle AddWall(int i,float scale = 1)
        {
            float x = (i % _map.Width) * _map.TileWidth;
            float y = (float)Math.Floor(i / (double)_map.Width) * _map.TileHeight;
            Rectangle rectangle = new Rectangle((int)x, (int)y, (int)(_tileSets[0]._tileWidth * scale),(int)( _tileSets[0]._tileHeight* scale));
            _walls.Add(rectangle);
            _grid.SetCell(i % _map.Width, i / _map.Width, Enums.CellType.Solid);
            return rectangle;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //for (int i = 0; i < _walls.Count; i++)
            //{
            //    GraphicManager.DrawRectangle(spriteBatch,_walls[i],0.8f);
            //}
            for (int tileLayer = 0; tileLayer < 3; tileLayer++)
            {
                for (int i = 0; i < _map.TileLayers[tileLayer].Tiles.Count; i++)
                {
                    int gid = _map.TileLayers[tileLayer].Tiles[i].Gid;
                    TileSet tileset=null;
                    if (ManageGid(spriteBatch, i, gid, tileLayer))
                    {

                    }
                    else
                    {
                        if (gid < _map.Tilesets[1].FirstGid)
                        {
                            tileset = _tileSets[0];
                        }
                        else if (gid > _map.Tilesets[1].FirstGid && gid < _map.Tilesets[2].FirstGid)
                        {
                            tileset = _tileSets[1];
                            gid = gid - _map.Tilesets[1].FirstGid + 1;
                        }
                        else if (gid > _map.Tilesets[2].FirstGid)
                        {
                            tileset = _tileSets[2];
                            gid = gid - _map.Tilesets[2].FirstGid + 1;
                        }
                        if (gid != 0)
                            DrawTile(gid, tileset, spriteBatch, i, 0.01f * tileLayer);
                    }
                }
            }
        }
        public void DrawTile(int gid, TileSet tileset, SpriteBatch spriteBatch, int i, float layer,float scale = 1)
        {
            int tileFrame = gid - 1;
            int column = tileFrame % tileset._tilesetTilesWide;
            int row = (int)Math.Floor((double)tileFrame / (double)tileset._tilesetTilesWide);

            float x = (i % _map.Width) * _map.TileWidth;
            float y = (float)Math.Floor(i / (double)_map.Width) * _map.TileHeight;

            Rectangle tilesetRec = new Rectangle(tileset._tileWidth * column, tileset._tileHeight * row, (int)(tileset._tileWidth), (int)(tileset._tileHeight));
            spriteBatch.Draw(tileset._texture, new Rectangle((int)x, (int)y, (int)(tileset._tileWidth * scale),(int)( tileset._tileHeight * scale)), tilesetRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, layer);
        }


        static public float GetLayerDepth(float y)
        {
            return (y / _graphicDevice.Viewport.Height * GraphicManager.ScreenScale.Y) + 0.1f;
        }
        static public Coord GetCoordTile(Vector2 _position)
        {
            Coord coord = new Coord((int)(_position.X / 1920 * _map.Width), (int)(_position.Y / 1080 * _map.Height));
            if (coord.X >= _map.Width)
                coord.X = _map.Width - 1;
            if (coord.Y >= _map.Height)
                coord.Y = _map.Height - 1;
            return coord;
        }
        static public Vector2 GetPositionFromCoord(Coord coord)
        {
            return new Vector2(coord.X * _tileSets[0]._tileWidth, coord.Y * _tileSets[0]._tileHeight);
        }
        static public Vector2 GetPositionFromCoord(int x, int y)
        {
            return new Vector2(x * _tileSets[0]._tileWidth, y * _tileSets[0]._tileHeight);
        }
        static public Rectangle GetRectangleFromCoord(int x, int y,float scale = 1)
        {
            return new Rectangle(x * _tileSets[0]._tileWidth, y * _tileSets[0]._tileHeight,(int)( 16 * scale),(int)( 16 * scale));
        }
    }
}
