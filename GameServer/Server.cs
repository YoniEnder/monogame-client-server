﻿using Microsoft.Xna.Framework;
namespace GameServer
{
    public class Server : Game
    {
        public static Game_Server game;
        static void Main()
        {
            try
            {
                using (game = new Game_Server())
                    game.Run();

            }
            catch
            {
                System.Console.WriteLine("Server crushed?");
            }

        }
    }
}