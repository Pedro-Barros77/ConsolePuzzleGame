using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsolePuzzleGame.Tutorials
{
    public class Tutorial_07 : Level
    {
        public int BoardWidth = 8, BoardHeight = 8;
        public Game GameLevel;


        public Tutorial_07() { }

        public override void StartLevel()
        {
            GameLevel = new Game(BoardWidth, BoardHeight, this);
            Console.Clear();
            Start();
        }
        public override void UpdateHeader()
        {
            AddContent();
        }

        void Start()
        {
            base.ActiveLevel = GameLevel;
            GameLevel.Title = this.ToString().Substring(this.ToString().IndexOf("Tutorials") + 10);
            Console.Title = GameLevel.Title;

            GameLevel.ResetLevelData();
            GameLevel.FillBoardWithBlank();

            GameLevel.PlayerXPos = 0; GameLevel.PlayerYPos = 0;
            GameLevel.P1_XField = -1; GameLevel.P1_YField = -1;

            GameLevel.Multiplayer = false;

            GameLevel.Player2XPos = 0; GameLevel.Player2YPos = GameLevel.BoardHeight - 1;
            GameLevel.P2_XField = -1; GameLevel.P2_YField = -1;

            GameLevel.ExitXPos = 5; GameLevel.ExitYPos = 4;

            //GameLevel.Board.BorderColor = ConsoleColor.DarkGray;

            GameLevel.AddObject(new GameObject(GameController.Objects.Player, GameLevel.PlayerXPos, GameLevel.PlayerYPos));
            GameLevel.AddObject(new GameObject(GameController.Objects.Exit, GameLevel.ExitXPos, GameLevel.ExitYPos));

            if (GameLevel.Multiplayer)
                GameLevel.AddObject(new GameObject(GameController.Objects.Player2, GameLevel.Player2XPos, GameLevel.Player2YPos));

            //Add custom game objects________________________________________

            GameLevel.Board.Fill(GameController.Objects.KeyDoor, Game.BoardObjects, 4, 3, 6, 5, BoardBuilder.FillType.HollowBox, "A ");

            GameLevel.AddObject(new GameObject(GameController.Objects.StandingButton, 5, 0, "A"));
           
            GameLevel.Board.Fill(GameController.Objects.Wall, Game.BoardObjects, 1, 2, 1, 7, BoardBuilder.FillType.SolidBox);
            GameLevel.AddObject(new GameObject(GameController.Objects.Box, 0, 7));
            GameLevel.AddObject(new GameObject(GameController.Objects.Wall, 3, 2));
            GameLevel.Board.Fill(GameController.Objects.Wall, Game.BoardObjects, 4, 0, 4, 1, BoardBuilder.FillType.SolidBox);
           

            //End of game objects addition____________________________________

            GameLevel.ObjectUnderPlayer1 = new GameObject(GameController.Objects.Blank, GameLevel.PlayerXPos, GameLevel.PlayerYPos);
            GameLevel.ObjectUnderPlayer2 = new GameObject(GameController.Objects.Blank, GameLevel.Player2XPos, GameLevel.Player2YPos);

            AddContent();
            GameLevel.WriteUI(true); //Writes header
            Console.CursorTop += 2;
            GameLevel.WriteUI(false); //Writes instructions

            GameLevel.BoardVerticalField = (Console.WindowHeight - GameLevel.HeaderRows - GameLevel.FooterRows) - 4;
            GameLevel.BoardHorizontalField = (Console.BufferWidth / 3) - 1;

            GameLevel.boardCursorLeft = Console.CursorLeft;
            GameLevel.boardCursorTop = Console.CursorTop;

            GameController.CurrentFrame = 0;

            DateTime? aSecondHence = null;
            int FPS = 0, currFrame = 0;
            while (GameController.Playing)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(GameLevel.boardCursorLeft, GameLevel.boardCursorTop);

                GameLevel.Board.Build(GameLevel);

                GameLevel.GetPressedKey(GameController.consoleKey);

                GameLevel.RunEnemies();









                if (aSecondHence == null)
                {
                    currFrame = GameController.CurrentFrame;
                    aSecondHence = DateTime.Now.AddSeconds(1);
                }
                else
                {
                    if (DateTime.Now > aSecondHence)
                    {
                        FPS = GameController.CurrentFrame - currFrame;
                        aSecondHence = null;
                    }
                }

                string Space = "          -          ";
                Console.Title = $"Console Puzzle Game - {GameLevel.Title}{Space}FPS: {FPS}{Space}Current Frame: {GameController.CurrentFrame}";

                Console.SetCursorPosition(0, GameLevel.HeaderRows);
                InfoBar();
                GameController.CurrentFrame++;
            }

            if (GameLevel.LevelCompleted)
            {
                Game.BoardObjects[GameLevel.PlayerYPos][GameLevel.PlayerXPos].fgColor = ConsoleColor.Green;
                Game.BoardObjects[GameLevel.PlayerYPos][GameLevel.PlayerXPos].BracketsfgColor = ConsoleColor.DarkGreen;
                Game.BoardObjects[GameLevel.PlayerYPos][GameLevel.PlayerXPos].Brackets = "<>";

                if (GameLevel.Multiplayer)
                {
                    Game.BoardObjects[GameLevel.Player2YPos][GameLevel.Player2XPos].fgColor = ConsoleColor.Green;
                    Game.BoardObjects[GameLevel.Player2YPos][GameLevel.Player2XPos].BracketsfgColor = ConsoleColor.DarkGreen;
                    Game.BoardObjects[GameLevel.Player2YPos][GameLevel.Player2XPos].Brackets = "<>";
                }

                Console.SetCursorPosition(GameLevel.boardCursorLeft, GameLevel.boardCursorTop);
                GameLevel.Board.Build(GameLevel);
                Thread.Sleep(400);
                MenuController.OpenPage(MenuController.Pages.CompletedLevel, new Pages.Blank(), GameLevel.ActiveLevel);
            }


        }

        public void AddContent()
        {
            Header = new UI();
            Footer = new UI();
            int headerRows = 4, footerRows = 6;
            HeaderRows = headerRows;
            FooterRows = footerRows;

            for (int lines = 0; lines < headerRows; lines++)
                Header.Line.Add(new List<Text>());

            Header.Line[0].Add(new Text(MenuController.CenterText(GameLevel.Title.Replace('_', ' '), bothSides: true), ConsoleColor.DarkMagenta, ConsoleColor.White));
            Header.Line[1].Add(new Text(""));
            Header.Line[2].Add(new Text(MenuController.Language == 1 ? "Essa \"" : "This \"", centerLine: true));
            Header.Line[2].Add(new Text("#", fgColor: ConsoleColor.DarkYellow));
            Header.Line[2].Add(new Text(MenuController.Language == 1 ? "\" é uma Caixa." : "\" is a Box."));
            Header.Line[2].Add(new Text(MenuController.Language == 1 ? " Você pode empurra-la ou puxa-la (segurando CTRL)" : " You can push it or pull it (holding CTRL down)."));

            Header.Line[3].Add(new Text(MenuController.Language == 1 ? "Esse \"" : "This \"", centerLine: true));
            Header.Line[3].Add(new Text("[", fgColor: ConsoleColor.DarkCyan));
            Header.Line[3].Add(new Text("-", fgColor: ConsoleColor.Cyan));
            Header.Line[3].Add(new Text("]", fgColor: ConsoleColor.DarkCyan));
            Header.Line[3].Add(new Text(MenuController.Language == 1 ? "\" é um Botão." : "\" is a Button."));
            Header.Line[3].Add(new Text(MenuController.Language == 1 ? " Ele abre portas ao ser pressionado." : " It opens doors when pressed."));




            for (int lines = 0; lines < footerRows; lines++)
                Footer.Line.Add(new List<Text>());


            if (GameLevel.Multiplayer)
            {
                Footer.Line[0].Add(new Text(MenuController.Language == 0 ? "Move Player 1: " : "Mover Jogador 1: ", fgColor: ConsoleColor.DarkGray));
                Footer.Line[0].Add(new Text(MenuController.Language == 0 ? "Arrow keys" : "Setas", fgColor: ConsoleColor.Magenta));
                Footer.Line[0].Add(new Text(MenuController.Language == 0 ? "Move Player 2: " : "Mover Jogador 2: ", fgColor: ConsoleColor.DarkGray));
                Footer.Line[0].Add(new Text(MenuController.Language == 0 ? "WASD" : "WASD", fgColor: ConsoleColor.Magenta));
            }
            else
            {
                Footer.Line[0].Add(new Text(MenuController.Language == 0 ? "Move: " : "Mover: ", fgColor: ConsoleColor.DarkGray));
                Footer.Line[0].Add(new Text(MenuController.Language == 0 ? "Arrow keys/WASD" : "Setas/WASD", fgColor: ConsoleColor.Magenta));
            }

            Footer.Line[1].Add(new Text(MenuController.Language == 0 ? "Grab box: " : "Agarrar caixa: ", fgColor: ConsoleColor.DarkGray));
            Footer.Line[1].Add(new Text(MenuController.Language == 0 ? "Hold CTRL" : "Segure CTRL", fgColor: ConsoleColor.Magenta));

            Footer.Line[2].Add(new Text(MenuController.Language == 0 ? "Pause: " : "Pausar: ", fgColor: ConsoleColor.DarkGray));
            Footer.Line[2].Add(new Text(MenuController.Language == 0 ? "P" : "P", fgColor: ConsoleColor.Magenta));

            Footer.Line[3].Add(new Text(MenuController.Language == 0 ? "Quick Restart: " : "Reinício Rápido: ", fgColor: ConsoleColor.DarkGray));
            Footer.Line[3].Add(new Text(MenuController.Language == 0 ? "R" : "R", fgColor: ConsoleColor.Magenta));

            GameLevel.Header = Header;
            GameLevel.Footer = Footer;
            GameLevel.HeaderRows = HeaderRows;
            GameLevel.FooterRows = footerRows;
        }

        void InfoBar()
        {
            UI Bar = new UI();
            Bar.Line.Add(new List<Text>());
            Bar.Line.Add(new List<Text>());

            string collectedKeys = "";
            string pressedButtons = "";
            foreach (char key in GameController.CollectedKeys) { collectedKeys += ", " + key; }
            foreach (char button in GameController.PressedButtons) { pressedButtons += ", " + button; }
            if (collectedKeys.Length == 0)
                collectedKeys = "000";
            if (pressedButtons.Length == 0)
                pressedButtons = "000";


            Bar.Line[0].Add(new Text(new string('_', Console.WindowWidth - 1), ConsoleColor.Black, ConsoleColor.White));
            Bar.Line[1].Add(new Text(MenuController.Language == 0 ? "Collected Coins: " : "Moedas Coletadas: ", ConsoleColor.DarkGray, ConsoleColor.Black, centerLine: true));
            Bar.Line[1].Add(new Text(GameController.CollectedCoins.ToString(), ConsoleColor.DarkGray, ConsoleColor.Yellow));
            Bar.Line[1].Add(new Text(MenuController.Language == 0 ? " - Collected Keys: " : " - Chaves Coletadas: ", ConsoleColor.DarkGray, ConsoleColor.Black));
            Bar.Line[1].Add(new Text(collectedKeys.Remove(0, 2), ConsoleColor.DarkGray, ConsoleColor.Cyan));
            Bar.Line[1].Add(new Text(MenuController.Language == 0 ? " - Pressed Buttons: " : " - Botões Pressionados: ", ConsoleColor.DarkGray, ConsoleColor.Black));
            Bar.Line[1].Add(new Text(pressedButtons.Remove(0, 2), ConsoleColor.DarkGray, ConsoleColor.Green));
            Bar.Line[1].Add(new Text("", ConsoleColor.DarkGray, ConsoleColor.Green, centerLine: true));


            bool centerLine = false;
            for (int line = 0; line < Bar.Line.Count; line++)
            {
                for (int item = 0; item < Bar.Line[line].Count; item++)
                {
                    int lineLength = 0;
                    if (Bar.Line[line][item].CenterLine)
                    {
                        centerLine = true;
                        foreach (var word in Bar.Line[line])
                        {
                            lineLength += word.text.Length;
                        }
                    }
                    Console.BackgroundColor = Bar.Line[line][item].bgColor;
                    Console.ForegroundColor = Bar.Line[line][item].fgColor;
                    if (centerLine)
                    {
                        Console.Write(MenuController.CenterText(new string(' ', lineLength), spacesOnly: true));
                        centerLine = false;
                    }
                    Console.Write(Bar.Line[line][item].text);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }
    }
}
