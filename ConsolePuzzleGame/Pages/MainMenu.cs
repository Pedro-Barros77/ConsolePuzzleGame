using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsolePuzzleGame.Pages
{
    public class MainMenu : Page
    {
        string optionDescription = MenuController.Language == 0 ? "Open tutorials list" : "Abrir lista de tutoriais";
        public MainMenu(Page pgToClose)
        {
            GameController.CurrentFrame = 0;
            pgToClose.RunningThisPage = false;
            MenuController.PushPageInHistory(MenuController.Pages.MainMenu);
            Console.Clear();
            addContent();
            Run();
        }
        Panel MenuPanel;
        void addContent()
        {
            HeaderRows = 7;
            for (int lines = 0; lines < HeaderRows; lines++)
                Header.Line.Add(new List<Text>());

            Header.Line[0].Add(new Text(MenuController.CenterText(optionDescription, bothSides: true), ConsoleColor.DarkMagenta, ConsoleColor.White));
            Header.Line[1].Add(new Text(MenuController.CenterText("", true, true)));
            Header.Line[2].Add(new Text(MenuController.Language == 0 ? "Tutorials" : "Tutoriais", centerLine: true, alignRows: true));
            Header.Line[3].Add(new Text(MenuController.Language == 0 ? "Levels" : "Níveis", centerLine: true, alignRows: true, active: false));
            Header.Line[4].Add(new Text(MenuController.Language == 0 ? "Survival Mode" : "Modo Sobrevivência", centerLine: true, alignRows: true, active: false));
            Header.Line[5].Add(new Text(MenuController.Language == 0 ? "Level Creator" : "Criar Nível", centerLine: true, alignRows: true, active: false));
            Header.Line[6].Add(new Text(MenuController.Language == 0 ? "Options" : "Opções", centerLine: true, alignRows: true));

            firstOptionIndex = 2;


            FooterRows = 5;
            for (int lines = 0; lines < FooterRows; lines++)
                Footer.Line.Add(new List<Text>());

            Footer.Line[0].Add(new Text(new string('_', Console.WindowWidth - 1), fgColor: ConsoleColor.DarkGray));

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


            MenuPanel = new Panel(MenuController.Language == 0 ? "welcome to console puzzle game!" : "bem-vindo ao console puzzle game!", delay: 50, width: -1, panelLeft: -1, textAlign: "hidden", slide: true, loop: true);
            MenuPanel.TextColor = ConsoleColor.Gray;
            MenuPanel.BgColor = ConsoleColor.Black;
        }
        void Run()
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write(new string(' ', Console.WindowWidth - 1));
            while (RunningThisPage)
            {
                Console.CursorTop = 1;

                try
                {
                    MenuPanel.SlidePanel();
                }
                catch
                {
                    Console.SetCursorPosition(0, 1);
                    MenuPanel = new Panel("Console Puzzle Game", 0, slide: false, loop: false);
                    MenuPanel.TextColor = ConsoleColor.Gray;
                    MenuPanel.BgColor = ConsoleColor.Black;
                    MenuPanel.SlidePanel();
                }
                cursorTop = MenuPanel.lastCursorTop;
                WriteHeader();

                WriteFooter();

                Console.SetCursorPosition(0, 0);
                GameController.CurrentFrame++;
                Console.Title = "Frame: " + GameController.CurrentFrame + " " + MenuController.Language;
                GetInput();
                //Thread.Sleep(20);
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
                            MenuController.OpenPage(MenuController.Pages.Tutorials, this, MenuController.nullLevel);
                            break;


                        case 4:
                            MenuController.OpenPage(MenuController.Pages.Options, this, MenuController.nullLevel);
                            break;
                    }
                    break;

                case ConsoleKey.NumPad0:
                    GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
                    GameController.consoleKey = ConsoleKey.K;
                    GameController.Play(GameController.TutorialsList[GameController.TutorialsList.Count - 1], new Pages.Blank());
                    break;
            }
            GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
            GameController.consoleKey = ConsoleKey.K;

            switch (MenuController.CurrentOptionIndex)
            {
                case 0:
                    optionDescription = MenuController.Language == 0 ? "Open Tutorials List" : "Abrir Lista de Tutoriais";
                    break;
                case 1:
                    optionDescription = MenuController.Language == 0 ? "Open Levels List" : "Abrir Lista de Níveis";
                    break;
                case 2:
                    optionDescription = MenuController.Language == 0 ? "Start Survival Mode" : "Começar Modo Sobrevivência";
                    break;
                case 3:
                    optionDescription = MenuController.Language == 0 ? "Create Custom Levels" : "Criar Níveis Personalizados";
                    break;
                case 4:
                    optionDescription = MenuController.Language == 0 ? "Change Options and Settings" : "Mudar Opções e Configurações";
                    break;
            }
            Header.Line[0][0].text = MenuController.CenterText(optionDescription, bothSides: true);
        }
    }
}
