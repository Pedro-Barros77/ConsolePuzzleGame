using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsolePuzzleGame.Pages
{
    public class Options : Page
    {
        Level gameLevel;
        string devCode = "";
        public Options(Page pgToClose, Level gameLvl)
        {
            gameLevel = gameLvl;
            pgToClose.RunningThisPage = false;
            MenuController.CurrentOptionIndex = 0;
            MenuController.PushPageInHistory(MenuController.Pages.Options);
            Console.Clear();
            addContent();
            Run();
        }
        string optionDescription = MenuController.Language == 0 ? "Start Tutorial 01" : "Começar Tutorial 01";
        Panel TutorialsPanel;
        void addContent()
        {
            HeaderRows = 6;
            for (int lines = 0; lines < HeaderRows; lines++)
                Header.Line.Add(new List<Text>());

            Header.Line[0].Add(new Text(MenuController.CenterText(optionDescription, bothSides: true), ConsoleColor.DarkMagenta, ConsoleColor.White));
            Header.Line[1].Add(new Text(MenuController.CenterText("", true, true)));

            Header.Line[2].Add(new Text(MenuController.Language == 0 ? "Language: " : "Idioma: ", centerLine: true, alignRows: true));
            Header.Line[2].Add(new Text(MenuController.Language == 0 ? "English" : "Português", fgColor: ConsoleColor.Green, centerLine: false, alignRows: true));


            Header.Line[3].Add(new Text(MenuController.Language == 0 ? "Sound Effects: " : "Efeitos Sonoros: ", centerLine: true, alignRows: true));
            if (MenuController.SoundFX)
                Header.Line[3].Add(new Text(MenuController.Language == 0 ? "On" : "Ligado", fgColor: ConsoleColor.Green, centerLine: false, alignRows: true));
            else
                Header.Line[3].Add(new Text(MenuController.Language == 0 ? "Off" : "Desligado", fgColor: ConsoleColor.Red, centerLine: false, alignRows: true));


            Header.Line[4].Add(new Text(MenuController.Language == 0 ? "Music: " : "Música: ", centerLine: true, alignRows: true));
            if (MenuController.Music)
                Header.Line[4].Add(new Text(MenuController.Language == 0 ? "On" : "Ligado:", fgColor: ConsoleColor.Green, centerLine: false, alignRows: true));
            else
                Header.Line[4].Add(new Text(MenuController.Language == 0 ? "Off" : "Desligado", fgColor: ConsoleColor.Red, centerLine: false, alignRows: true));


            Header.Line[5].Add(new Text(MenuController.Language == 0 ? "Dev Code: " : "Código: ", centerLine: true, alignRows: true));

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

            TutorialsPanel = new Panel(MenuController.Language == 0 ? "options" : "opcoes", width: -1, panelLeft: -1, textAlign: "center", slide: false, loop: false);
            TutorialsPanel.TextColor = ConsoleColor.Gray;
            TutorialsPanel.BgColor = ConsoleColor.Black;
        }
        void Run()
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write(new string(' ', Console.WindowWidth - 1));
            while (RunningThisPage)
            {
                Console.CursorTop = 1;

                TutorialsPanel.SlidePanel();

                cursorTop = TutorialsPanel.lastCursorTop;
                WriteHeader();

                WriteFooter();

                Console.SetCursorPosition(0, 0);
                Console.Title = Console.Title.Remove(Console.Title.Length - 1) + MenuController.CurrentOptionIndex;

                Console.Title = GameController.consoleKey.ToString();
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
                    Header.Line.Clear();
                    Footer.Line.Clear();
                    switch (MenuController.CurrentOptionIndex)
                    {
                        case 0:
                            if (MenuController.Language == 0)
                                MenuController.Language = 1;
                            else
                                MenuController.Language = 0;
                            break;
                        case 1:
                            if (MenuController.SoundFX)
                                MenuController.SoundFX = false;
                            else
                                MenuController.SoundFX = true;
                            break;


                        case 3:
                            MenuController.ReadInput = false;
                            GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
                            GameController.consoleKey = ConsoleKey.K;
                            Console.ResetColor();
                            Console.SetCursorPosition((Console.WindowWidth / 2), firstOptionIndex + MenuController.CurrentOptionIndex + 6);
                            devCode = Console.ReadLine();
                            MenuController.ReadInput = true;
                            Program.StartReadingInput();

                            switch (devCode)
                            {
                                case "ulckall":
                                    int a = GameController.TutorialsList.Count - 1;
                                    for (int i = 0; i < a; i++)
                                    {
                                        GameController.CompletedTutorials.Add(new ConsolePuzzleGame.Tutorials.Tutorial_01());
                                    }
                                    Console.SetCursorPosition((Console.WindowWidth / 2), firstOptionIndex + MenuController.CurrentOptionIndex + 6);
                                    if (GameController.CompletedTutorials.Count == (a+1))
                                    {
                                        Console.Write("Níveis Desbloqueados!");
                                    }
                                    Thread.Sleep(500);
                                    break;
                            }
                            break;
                    }
                    addContent();
                    break;
                case ConsoleKey.Backspace:
                case ConsoleKey.LeftArrow:
                    MenuController.PagesHistory.Pop();
                    Console.Clear();
                    GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
                    GameController.consoleKey = ConsoleKey.K;
                    MenuController.OpenPage(MenuController.PagesHistory.Peek(), this, gameLevel);
                    break;
            }
            GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
            GameController.consoleKey = ConsoleKey.K;

            switch (MenuController.CurrentOptionIndex)
            {
                case 0:
                    optionDescription = MenuController.Language == 0 ? "Change game language to portuguese" : "Mudar idioma do jogo para inglês";
                    break;
                case 1:
                    optionDescription = MenuController.Language == 0 ? "Enable/Disable game sound effects" : "Ativa/Desativa efeitos sonoros do jogo";
                    break;
                case 2:
                    optionDescription = MenuController.Language == 0 ? "Enable/Disable game music" : "Ativa/Desativa música do jogo";
                    break;
                case 3:
                    optionDescription = MenuController.Language == 0 ? "Execute special commands (developer only)" : "Executa comandos especiais (apenas para o desenvolvedor)";
                    break;
            }

            Header.Line[0][0].text = MenuController.CenterText(optionDescription, bothSides: true);
        }
    }
}
