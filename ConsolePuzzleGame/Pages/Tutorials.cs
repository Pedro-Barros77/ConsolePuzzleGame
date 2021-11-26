using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsolePuzzleGame.Pages
{
    public class Tutorials : Page
    {
        public Tutorials(Page pgToClose)
        {
            pgToClose.RunningThisPage = false;
            MenuController.PushPageInHistory(MenuController.Pages.Tutorials);
            Console.Clear();
            addContent();
            Run();
        }
        string optionDescription = MenuController.Language == 0 ? "Start Tutorial 01" : "Começar Tutorial 01";
        Panel TutorialsPanel;
        void addContent()
        {
            HeaderRows = 14;
            for (int lines = 0; lines < HeaderRows; lines++)
                Header.Line.Add(new List<Text>());

            Header.Line[0].Add(new Text(MenuController.CenterText(optionDescription, bothSides: true), ConsoleColor.DarkMagenta, ConsoleColor.White));
            Header.Line[1].Add(new Text(MenuController.CenterText("", true, true)));
            Header.Line[2].Add(new Text(MenuController.Language == 0 ? "01- Movement" : "01- Movimentação", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 1));
            Header.Line[3].Add(new Text(MenuController.Language == 0 ? "02- Walls" : "02- Paredes", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 2));
            Header.Line[4].Add(new Text(MenuController.Language == 0 ? "03- Coins" : "03- Moedas", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 3));
            Header.Line[5].Add(new Text(MenuController.Language == 0 ? "04- Keys" : "04- Chaves", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 4));
            Header.Line[6].Add(new Text(MenuController.Language == 0 ? "05- One-Ways" : "05- One-Ways", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 5));
            Header.Line[7].Add(new Text(MenuController.Language == 0 ? "06- Portals" : "06- Portais", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 6));
            Header.Line[8].Add(new Text(MenuController.Language == 0 ? "07- Boxes" : "07- Caixas", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 7));
            Header.Line[9].Add(new Text(MenuController.Language == 0 ? "08- The Guards" : "08- Os Guardas", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 8));
            Header.Line[10].Add(new Text(MenuController.Language == 0 ? "09- The Cannons" : "09- Os Canhões", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 9));
            Header.Line[11].Add(new Text(MenuController.Language == 0 ? "10- The Seekers" : "10- Os Perseguidores", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 10));
            Header.Line[12].Add(new Text(MenuController.Language == 0 ? "11- Multiplayer" : "11- Multijogador", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 11));
            Header.Line[13].Add(new Text(MenuController.Language == 0 ? "12- Field of View" : "12- Campo de Visão", centerLine: true, alignRows: true, active: GameController.CompletedTutorials.Count >= 11));

            firstOptionIndex = 2;


            FooterRows = 5;
            for (int lines = 0; lines < FooterRows; lines++)
                Footer.Line.Add(new List<Text>());

            Footer.Line[0].Add(new Text(new string('_', Console.WindowWidth -1), fgColor: ConsoleColor.DarkGray));

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

            TutorialsPanel = new Panel(MenuController.Language == 0 ? "tutorials" : "tutoriais", width: -1, panelLeft: -1, textAlign: "center", slide: false, loop: false);
            TutorialsPanel.TextColor = ConsoleColor.Gray;
            TutorialsPanel.BgColor = ConsoleColor.Black;
        }
        void Run()
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write(new string(' ', Console.WindowWidth -1));
            while (RunningThisPage)
            {
                Console.CursorTop = 1;
                
                TutorialsPanel.SlidePanel();

                cursorTop = TutorialsPanel.lastCursorTop;
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
                    if (GameController.TutorialsList.Count-1 >= MenuController.CurrentOptionIndex && Header.Line[MenuController.CurrentOptionIndex + firstOptionIndex][0].Active)
                        GameController.Play(GameController.TutorialsList[MenuController.CurrentOptionIndex], this);
                    break;

                case ConsoleKey.Backspace:
                case ConsoleKey.LeftArrow:
                    MenuController.PagesHistory.Pop();
                    MenuController.OpenPage(MenuController.PagesHistory.Peek(), this, MenuController.nullLevel);
                    break;
            }
            GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
            GameController.consoleKey = ConsoleKey.K;

            optionDescription = MenuController.Language == 0 ? "Start Tutorial " : "Começar Tutorial ";
            optionDescription += MenuController.CurrentOptionIndex.ToString().Length == 1 ? "0" + (MenuController.CurrentOptionIndex + 1) : (MenuController.CurrentOptionIndex + 1).ToString();
            Header.Line[0][0].text = MenuController.CenterText(optionDescription, bothSides: true);
        }
    }
}
