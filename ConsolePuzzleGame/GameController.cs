using System;
using System.Collections.Generic;

namespace ConsolePuzzleGame
{
    public static class GameController
    {
        public enum Objects
        {
            //GameLevel.Board.Fill(GameController.Objects.Wall,Game.BoardObjects,2,2,7,7,BoardBuilder.FillType.HollowBox);
            NullObject,
            Blank,
            Player,
            Player2,
            Exit,
            Wall,
            Coin,
            CoinDoor,
            Key,
            KeyDoor,
            OneWay,
            Portal,
            Box,
            StandingButton,
            PlayerDoor,

            Enemy_Guard,
            //GameLevel.AddObject(new GameObject(GameController.Objects.Enemy_Guard, 6, 0, "Guard_01"));
            //Game.BoardEnemies["Guard_01"].Set(health: 2, speed: 0.7, delay: 1000, direction: "down", startDelay: 0);
            Enemy_Cannon,
            Enemy_CannonBullet,
            //GameLevel.AddObject(new GameObject(GameController.Objects.Enemy_Cannon, 5, 0, "Cannon_01"));
            //Game.BoardEnemies["Cannon_01"].Set(health: 2, direction: "down", bulletSpeed: 4, rateOfFire: 1, startDelay: 0);
            Enemy_Seeker
            //GameLevel.AddObject(new GameObject(GameController.Objects.Enemy_Seeker, 5, 5, "Seeker_01"));
            //Game.BoardEnemies["Seeker_01"].Set(health: 2, speed: 0.3, sightRadius: 3);
        }




        //inimigos + boss
        //ammo + bullet
        //power-up (armor, slow motion etc)
        //bomba

        //Squeezing walls

        public static List<Level> TutorialsList = new List<Level>
        {
            new Tutorials.Tutorial_01(),
            new Tutorials.Tutorial_02(),
            new Tutorials.Tutorial_03(),
            new Tutorials.Tutorial_04(),
            new Tutorials.Tutorial_05(),
            new Tutorials.Tutorial_06(),
            new Tutorials.Tutorial_07(),
            new Tutorials.Tutorial_08(),
            new Tutorials.Tutorial_09(),
            new Tutorials.Tutorial_10(),
            new Tutorials.Tutorial_11(),
            new Tutorials.Tutorial_12(),
        };

        public static List<Enum> ObjToKeepContent = new List<Enum> { Objects.CoinDoor, Objects.KeyDoor, Objects.Portal, Objects.StandingButton, Objects.PlayerDoor };
        public static List<Level> CompletedTutorials = new List<Level> {new Tutorials.Tutorial_01()};

        public static int CurrentFrame = 0, CollectedCoins = 0;
        public static bool Playing = false;
        public static List<char> CollectedKeys = new List<char>();
        public static List<char> PressedButtons = new List<char>();
        public static List<Tuple<string, int, int>> BoardPortals = new List<Tuple<string, int, int>>();

        public static ConsoleKeyInfo keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
        public static ConsoleKey consoleKey;
        public static bool CtrlKeyHeld = false;

        public static void RestartLevel(Level activeLevel)
        {
            Playing = false;
            activeLevel.StartLevel();
        }

        public static void Play(Level lvl, Pages.Page pageToClose)
        {
            MenuController.CurrentOptionIndex = 0;
            pageToClose.RunningThisPage = false;
            MenuController.PagesHistory.Clear();
            lvl.StartLevel();
        }

        public static void ContinueGame(Pages.Page pageToClose, Level activeLevel)
        {
            pageToClose.RunningThisPage = false;
            MenuController.CurrentOptionIndex = 0;
            keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
            consoleKey = ConsoleKey.K;
            MenuController.PagesHistory.Clear();
            Console.Clear();
            activeLevel.UpdateHeader();
            activeLevel.ActiveLevel.WriteUI(true);
            Console.CursorTop += 2;
            activeLevel.ActiveLevel.WriteUI(false);
        }
    }

    public abstract class Level : Pages.Page
    {
        public Game ActiveLevel;
        public abstract void StartLevel();
        public abstract void UpdateHeader();
        public Level() { }
    }
}
