using System;
using System.Collections.Generic;


namespace ConsolePuzzleGame.Pages
{
    public class Pause : Page
    {
        Level gameLevel;
        bool isTutorial;
        string LevelName;
        string optionDescription;

        public Pause(Page pgToClose, Level gameLvl)
        {
            gameLevel = gameLvl;
            isTutorial = gameLevel.ToString().Substring(18, 9) == "Tutorials" ? true : false;
            LevelName = gameLevel.ToString().Substring(gameLevel.ToString().IndexOf("Tutorials") + 10);
            optionDescription = MenuController.Language == 0 ? "Continue " + LevelName : "Continuar " + LevelName;

            MenuController.CurrentOptionIndex = 0;
            pgToClose.RunningThisPage = false;
            MenuController.PushPageInHistory(MenuController.Pages.Pause);
            Console.Clear();
            addContent();
            Run();
        }

        Panel PausePanel;
        void addContent()
        {
            HeaderRows = 7;
            for (int lines = 0; lines < HeaderRows; lines++)
                Header.Line.Add(new List<Text>());

            string levelsList = "";
            if (MenuController.Language == 0)
            {
                levelsList = isTutorial ? "Select Tutorial" : "Select Level";
            }
            else
            {
                levelsList = isTutorial ? "Selecionar Tutorial" : "Selecionar Nível";
            }

            Header.Line[0].Add(new Text(MenuController.CenterText(optionDescription, bothSides: true), ConsoleColor.DarkMagenta, ConsoleColor.White));
            Header.Line[1].Add(new Text(MenuController.CenterText("", true, true)));
            Header.Line[2].Add(new Text(MenuController.Language == 0 ? "Continue" : "Continuar", centerLine: true, alignRows: true));
            Header.Line[3].Add(new Text(MenuController.Language == 0 ? "Restart" : "Reiniciar", centerLine: true, alignRows: true));
            Header.Line[4].Add(new Text(levelsList, centerLine: true, alignRows: true));
            Header.Line[5].Add(new Text(MenuController.Language == 0 ? "Options" : "Opções", centerLine: true, alignRows: true));
            Header.Line[6].Add(new Text(MenuController.Language == 0 ? "Main Menu" : "Menu Principal", centerLine: true, alignRows: true));

            firstOptionIndex = 2;


            FooterRows = 5;
            for (int lines = 0; lines < FooterRows; lines++)
                Footer.Line.Add(new List<Text>());

            Footer.Line[0].Add(new Text(new string('_', Console.BufferWidth), fgColor: ConsoleColor.DarkGray));

            Footer.Line[1].Add(new Text(MenuController.Language == 0 ? "Navigate: " : "Navegar: ", fgColor: ConsoleColor.DarkGray));
            Footer.Line[1].Add(new Text(MenuController.Language == 0 ? "up/down arrows" : "setas para cima/baixo", fgColor: ConsoleColor.Magenta));
            Footer.Line[1].Add(new Text(new string(' ', Console.WindowWidth - (Footer.Line[1][0].text.Length + Footer.Line[1][1].text.Length) - 1)));

            Footer.Line[2].Add(new Text(MenuController.Language == 0 ? "Select: " : "Selecionar: ", fgColor: ConsoleColor.DarkGray));
            Footer.Line[2].Add(new Text(MenuController.Language == 0 ? "enter/SpaceBar/right arrow" : "enter/barra de espaço/seta para direita", fgColor: ConsoleColor.Magenta));
            Footer.Line[2].Add(new Text(new string(' ', Console.WindowWidth - (Footer.Line[2][0].text.Length + Footer.Line[2][1].text.Length) - 1)));

            Footer.Line[3].Add(new Text(MenuController.Language == 0 ? "Go back: " : "Voltar: ", fgColor: ConsoleColor.DarkGray));
            Footer.Line[3].Add(new Text(MenuController.Language == 0 ? "BackSpace/left arrow" : "BackSpace/seta para esquerda", fgColor: ConsoleColor.Magenta));
            Footer.Line[3].Add(new Text(new string(' ', Console.WindowWidth - (Footer.Line[3][0].text.Length + Footer.Line[3][1].text.Length) - 1)));

            Footer.Line[4].Add(new Text(" ", centerLine: true));

            PausePanel = new Panel(MenuController.Language == 0 ? "Game Paused" : "Jogo Pausado", width: -1, panelLeft: -1, textAlign: "center", slide: false, loop: false);
            PausePanel.TextColor = ConsoleColor.Gray;
            PausePanel.BgColor = ConsoleColor.Black;
        }
        void Run()
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write(new string(' ', Console.WindowWidth - 1));
            while (RunningThisPage)
            {
                Console.CursorTop = 1;

                PausePanel.SlidePanel();

                cursorTop = PausePanel.lastCursorTop;
                WriteHeader();

                WriteFooter();

                Console.SetCursorPosition(0, 0);

                GetInput();
            }
        }
        void GetInput()
        {
            ConsoleKey dirKey = GameController.consoleKey;
            switch (dirKey)
            {
                case ConsoleKey.UpArrow:
                    if (MenuController.CurrentOptionIndex > 0)
                        MenuController.CurrentOptionIndex--;
                    break;

                case ConsoleKey.DownArrow:
                    if (MenuController.CurrentOptionIndex < (Header.Line.Count - 1) - firstOptionIndex)
                        MenuController.CurrentOptionIndex++;
                    break;

                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                case ConsoleKey.RightArrow:
                    GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
                    GameController.consoleKey = ConsoleKey.K;
                    switch (MenuController.CurrentOptionIndex)
                    {
                        case 0:
                            GameController.ContinueGame(this, gameLevel);
                            break;
                        case 1:
                            GameController.RestartLevel(gameLevel);
                            break;
                        case 2:
                            MenuController.OpenPage(MenuController.Pages.Tutorials, this, gameLevel);
                            break;
                        case 3:
                            MenuController.OpenPage(MenuController.Pages.Options, this, gameLevel);
                            Console.SetCursorPosition(0, gameLevel.ActiveLevel.HeaderRows + gameLevel.ActiveLevel.FooterRows + gameLevel.ActiveLevel.BoardHeight + 2);
                            Console.WriteLine(new string(' ', (Console.WindowWidth - Console.CursorLeft) - 1));
                            Console.WriteLine(new string(' ', (Console.WindowWidth - Console.CursorLeft) - 1));
                            Console.WriteLine(new string(' ', (Console.WindowWidth - Console.CursorLeft) - 1));
                            break;
                        case 4:
                            MenuController.OpenPage(MenuController.Pages.MainMenu, this, gameLevel);
                            break;
                    }
                    break;

                case ConsoleKey.Backspace:
                case ConsoleKey.LeftArrow:
                    GameController.ContinueGame(this, gameLevel);
                    break;
            }
            GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
            GameController.consoleKey = ConsoleKey.K;

            string levelType = "";
            if (MenuController.Language == 0)
                levelType = isTutorial ? "Tutorial" : "Level";
            else
                levelType = isTutorial ? "Tutorial" : "Nível";
            switch (MenuController.CurrentOptionIndex)
            {
                case 0:
                    optionDescription = MenuController.Language == 0 ? "Continue " + LevelName : "Continuar " + LevelName;
                    break;
                case 1:
                    optionDescription = MenuController.Language == 0 ? "Restart " + LevelName : "Reiniciar " + LevelName;
                    break;
                case 2:
                    optionDescription = MenuController.Language == 0 ? "Select another " + levelType + " to Start" : "Selecione outro " + levelType + " para Jogar";
                    break;
                case 3:
                    optionDescription = MenuController.Language == 0 ? "Change Options and Settings" : "Mudar Opções e Configurações";
                    break;
                case 4:
                    optionDescription = MenuController.Language == 0 ? "Go back to Main Menu" : "Voltar para o Menu Principal";
                    break;
            }
            Header.Line[0][0].text = MenuController.CenterText(optionDescription, bothSides: true);
        }
    }
}
