using System;
using System.Collections.Generic;


namespace ConsolePuzzleGame.Pages
{
    public class LevelComplete : Page
    {
        Level gameLevel;
        bool isTutorial;
        string LevelName;
        string optionDescription;

        public LevelComplete(Page pgToClose, Level gameLvl)
        {
            gameLevel = gameLvl;
            isTutorial = gameLevel.ToString().Substring(18, 9) == "Tutorials" ? true : false;
            LevelName = gameLevel.ToString().Substring(gameLevel.ToString().IndexOf("Tutorials") + 10);
            optionDescription = MenuController.Language == 0 ? "Next Level " + LevelName : "Próximo Nível " + LevelName;

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
            if (!GameController.CompletedTutorials.Contains(gameLevel))
            {
                GameController.CompletedTutorials.Add(gameLevel);
            }
            HeaderRows = 6;
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
            Header.Line[2].Add(new Text(MenuController.Language == 0 ? "Next Level" : "Próximo Nível", centerLine: true, alignRows: true));
            Header.Line[3].Add(new Text(MenuController.Language == 0 ? "Restart" : "Reiniciar", centerLine: true, alignRows: true));
            Header.Line[4].Add(new Text(levelsList, centerLine: true, alignRows: true));
            Header.Line[5].Add(new Text(MenuController.Language == 0 ? "Main Menu" : "Menu Principal", centerLine: true, alignRows: true));

            firstOptionIndex = 2;


            FooterRows = 5;
            for (int lines = 0; lines < FooterRows; lines++)
                Footer.Line.Add(new List<Text>());

            Footer.Line[0].Add(new Text(new string('_', Console.WindowWidth-1), fgColor: ConsoleColor.DarkGray));

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

            PausePanel = new Panel(MenuController.Language == 0 ? "Level Completed" : "Nivel Completo", width: -1, panelLeft: -1, textAlign: "center", slide: false, loop: false);
            PausePanel.TextColor = ConsoleColor.Gray;
            PausePanel.BgColor = ConsoleColor.Black;
        }
        void Run()
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write(new string(' ', Console.WindowWidth-1));
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
                            if (GameController.TutorialsList.Count - 1 >= GameController.TutorialsList.IndexOf(gameLevel) + 1 && Header.Line[MenuController.CurrentOptionIndex + firstOptionIndex][0].Active)
                                GameController.Play(GameController.TutorialsList[GameController.TutorialsList.IndexOf(gameLevel)+1], this);
                            break;
                        case 1:
                            GameController.RestartLevel(gameLevel);
                            break;
                        case 2:
                            MenuController.OpenPage(MenuController.Pages.Tutorials, this, gameLevel);
                            break;
                        case 3:
                            MenuController.OpenPage(MenuController.Pages.MainMenu, this, gameLevel);
                            break;
                    }
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
                    optionDescription = MenuController.Language == 0 ? "Start " + LevelName.Remove(LevelName.Length-2) + (GameController.TutorialsList.IndexOf(gameLevel) + 2).ToString("D2") : "Jogar " + LevelName.Remove(LevelName.Length - 2) + (GameController.TutorialsList.IndexOf(gameLevel) + 2).ToString("D2");
                    break;
                case 1:
                    optionDescription = MenuController.Language == 0 ? "Restart " + LevelName : "Reiniciar " + LevelName;
                    break;
                case 2:
                    optionDescription = MenuController.Language == 0 ? "Select another " + levelType + " to Start" : "Selecione outro " + levelType + " para Jogar";
                    break;
                case 3:
                    optionDescription = MenuController.Language == 0 ? "Go back to Main Menu" : "Voltar para o Menu Principal";
                    break;
            }
            Header.Line[0][0].text = MenuController.CenterText(optionDescription, bothSides: true);
        }
    }
}
