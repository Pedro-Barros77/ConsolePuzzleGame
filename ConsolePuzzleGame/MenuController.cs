using System;
using System.Collections.Generic;


namespace ConsolePuzzleGame
{
    public static class MenuController
    {
        public static Level nullLevel = null;
        public static int ConsoleBufferWidth = Console.LargestWindowWidth, ConsoleBufferheight = Console.LargestWindowHeight;
        public enum Pages
        {
            MainMenu,
            Tutorials,
            Levels,
            SurvivalMode,
            LevelCreator,
            Options,
            Pause,
            CompletedLevel
        }
        public static Stack<Enum> PagesHistory = new Stack<Enum>();

        public static int CurrentOptionIndex = 0, Language = 0;
        public static bool SoundFX = true, Music = false, ReadInput = true;

        public static void StartMenu()
        {
            OpenPage(Pages.MainMenu, new ConsolePuzzleGame.Pages.Blank(), nullLevel);
        }

        public static void OpenPage(Enum pageToOpen, ConsolePuzzleGame.Pages.Page pageToClose, Level gameLvl)
        {
            ConsolePuzzleGame.Pages.Page NewPage;

            CurrentOptionIndex = 0;

            switch (pageToOpen)
            {
                case Pages.MainMenu:
                    NewPage = new ConsolePuzzleGame.Pages.MainMenu(pageToClose);
                    break;

                case Pages.Tutorials:
                    NewPage = new ConsolePuzzleGame.Pages.Tutorials(pageToClose);
                    break;

                case Pages.Options:
                    NewPage = new ConsolePuzzleGame.Pages.Options(pageToClose, gameLvl);
                    break;

                case Pages.Pause:
                    PauseGame(gameLvl, pageToClose);
                    break;

                case Pages.CompletedLevel:
                    NewPage = new ConsolePuzzleGame.Pages.LevelComplete(pageToClose, gameLvl);
                    break;
            }
        }
        public static void PauseGame(Level gameLvl, ConsolePuzzleGame.Pages.Page pageToClose)
        {
            ConsolePuzzleGame.Pages.Page NewPage;
            NewPage = new ConsolePuzzleGame.Pages.Pause(pageToClose, gameLvl);
        }

        public static void PushPageInHistory(Enum page)
        {
            if (PagesHistory.Count > 0)
            {
                if (PagesHistory.Peek().ToString() != page.ToString())
                    PagesHistory.Push(page);
            }
            else
            {
                PagesHistory.Push(page);
            }
        }

        public static string CenterText(string textToCenter, bool bothSides = false, bool spacesOnly = false)
        {
            string spaces = new string(' ', ((Console.WindowWidth - textToCenter.Length) / 2) - 1);

            if (spacesOnly)
                return bothSides ? spaces + " " + spaces + " " : spaces;
            else
            {
                return bothSides ? spaces + textToCenter + spaces + " " : spaces + textToCenter;
            }
        }
    }

    public class UI
    {
        public List<List<Text>> Line = new List<List<Text>>();
    }

    public class Text
    {
        public string text = "";
        public ConsoleColor bgColor, fgColor;
        public bool CenterLine, AlignRows, Active;
        public Text(string text, ConsoleColor bgColor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.Gray, bool centerLine = false, bool alignRows = false, bool active = true)
        {
            this.text = text;
            this.bgColor = bgColor;
            this.fgColor = fgColor;
            CenterLine = centerLine;
            AlignRows = alignRows;
            Active = active;
        }
    }

    public class Panel
    {
        public string Text = "";
        public float Delay = 0f;
        DateTime dt1, dt2;
        public int startingColumn = 0, Width = 0, PanelLeft = -1, textAlignSpaces = 0;
        public string Display = "", TextAlign = "center", displayAlignSpaces = "";
        public bool Slide = true, Loop;

        public ConsoleColor TextColor = ConsoleColor.White, BgColor = ConsoleColor.Black;
        public int lastCursorTop = 5;

        public Panel(string text, int delay = 0, int width = -1, int panelLeft = -1, string textAlign = "center", bool slide = true, bool loop = true)
        {
            Text = text.ToUpper();
            Delay = delay;
            PanelLeft = panelLeft;
            TextAlign = textAlign;
            Slide = slide;
            Loop = loop;
            dt1 = DateTime.Now;
            dt2 = dt1.AddMilliseconds(Delay);

            //Console.WriteLine(dt1 + " - " + dt2);
            //Console.ReadKey(true);

            if (width == -1)
            {
                Width = ((Console.BufferWidth / 3) - Text.Length) - 1;
            }
            else if (width == 0)
            {
                Width = Text.Length * 3 + (text.Length - 1);
            }
            else
            {
                Width = width;
            }

            for (int row = 0; row < 5; row++)
            {
                for (int letter = 0; letter < Text.Length; letter++)
                {
                    if (PanelLetters.ContainsKey(Text[letter]))
                    {
                        Display += PanelLetters[Text[letter]][0 + (row * 3)];
                        Display += PanelLetters[Text[letter]][1 + (row * 3)];
                        Display += PanelLetters[Text[letter]][2 + (row * 3)];
                    }

                    if (letter != Text.Length - 1)
                    {
                        Display += " ";
                    }
                }
            }
            int contentLength = Text.Length * 3 + (text.Length - 1);

            if (panelLeft == -1)
            {
                displayAlignSpaces = width == -1 ? "" : MenuController.CenterText(new string(' ', Width), spacesOnly: true);
            }
            else if (panelLeft == 0)
            {
                displayAlignSpaces = "";
            }
            else
            {
                if (panelLeft < Console.BufferWidth - Width)
                    displayAlignSpaces = new string(' ', panelLeft);
                else
                    displayAlignSpaces = new string(' ', (Console.BufferWidth - Width)-1);
            }

            if (width == -1)
            {
                displayAlignSpaces = "";
                if (TextAlign == "center")
                    textAlignSpaces = ((Console.BufferWidth - contentLength) / 2)-1;
                else if (TextAlign == "left")
                    textAlignSpaces = 0;
                else if (TextAlign == "right")
                    textAlignSpaces = (Console.BufferWidth - contentLength) - 1;
                else if (TextAlign == "hidden")
                    textAlignSpaces = Console.BufferWidth - 1;
            }
            else
            {
                if (TextAlign == "center")
                    textAlignSpaces = Width < contentLength ? 0 : ((Width - contentLength) / 2)-1;
                else if (TextAlign == "left")
                    textAlignSpaces = 0;
                else if (TextAlign == "right")
                    textAlignSpaces = Width < contentLength ? 0 : (Width - contentLength)-1;
                else if (TextAlign == "hidden")
                    textAlignSpaces = width;
            }
            Display = Display.Insert(contentLength * 4, new string(';', textAlignSpaces));
            Display = Display.Insert(contentLength * 3, new string(';', textAlignSpaces));
            Display = Display.Insert(contentLength * 2, new string(';', textAlignSpaces));
            Display = Display.Insert(contentLength * 1, new string(';', textAlignSpaces));
            Display = Display.Insert(contentLength * 0, new string(';', textAlignSpaces));
        }

        public void SlidePanel()
        {
            if (DateTime.Compare(DateTime.Now, dt2) >= 0)
            {
                dt1 = DateTime.Now;
                dt2 = dt1.AddMilliseconds(Delay);
                WriteOnPanel(Text);
                if (Slide)
                    startingColumn++;
                if (Loop)
                {
                    string content = Display.Substring(textAlignSpaces, Text.Length * 3 + (Text.Length - 1));
                    if (Width == (Console.BufferWidth / 3) - Text.Length - 1)
                    {
                        if (startingColumn >= Console.BufferWidth - 1 + (Text.Length * 3 + (Text.Length - 1)))
                        {
                            startingColumn = 0;
                        }
                    }
                    else if (Width != (Console.BufferWidth / 3) - Text.Length - 1)
                    {
                        if (startingColumn >= Width + (Text.Length * 3 + (Text.Length - 1)))
                        {
                            startingColumn = 0;
                        }
                    }
                }
            }
        }

        public Dictionary<char, string> PanelLetters = new Dictionary<char, string>
        {
            {'A',"010101111101101" },
            {'B',"110101110101110" },
            {'C',"111100100100111" },
            {'D',"110101101101110" },
            {'E',"111100110100111" },
            {'F',"111100110100100" },
            {'G',"011100100101011" },
            {'H',"101101111101101" },
            {'I',"111010010010111" },
            {'J',"111010010010110" },
            {'K',"101101110101101" },
            {'L',"100100100100111" },
            {'M',"101111111101101" },
            {'N',"010101101101101" },
            {'O',"010101101101010" },
            {'P',"111101111100100" },
            {'Q',"010101101111011" },
            {'R',"111101110101101" },
            {'S',"011100010001110" },
            {'T',"111010010010010" },
            {'U',"101101101101111" },
            {'V',"101101101101010" },
            {'W',"101101111111101" },
            {'X',"101101010101101" },
            {'Y',"101101010010010" },
            {'Z',"111001010100111" },
            {' ',"000000000000000" },
            {'1',"010110010010010" },
            {'2',"111001111100111" },
            {'3',"111001011001111" },
            {'4',"101101111001001" },
            {'5',"111100111001111" },
            {'6',"111100111101111" },
            {'7',"111001001001001" },
            {'8',"111101111101111" },
            {'9',"111101111001111" },
            {'0',"111101101101111" },
            {'?',"111001010000010" },
            {'!',"010010010000010" },
            {':',"000010000010000" },
            {',',"000010000010000" },
            {'-',"000000111000000" }
        };

        public void WriteOnPanel(string text)
        {
            int currentRow = 0;
            int contentLength = text.Length * 3 + (text.Length - 1);

            Console.CursorLeft += displayAlignSpaces.Length;

            for (int currentPixel = 0; currentPixel < Display.Length; currentPixel++)
            {
                bool printPixel = true;

                if (currentPixel < startingColumn + (currentRow * (text.Length * 4 - 1 + textAlignSpaces)))
                    printPixel = false;

                if (TextAlign == "hidden")
                {
                    if (currentPixel - ((contentLength + textAlignSpaces) * currentRow) >= textAlignSpaces + startingColumn)
                        printPixel = false;
                }

                //if (currentPixel - ((text.Length * 3 + (text.Length-1)) * currentRow) + textAlignSpaces >= Width + startingColumn)
                //    printPixel = false;

                if (printPixel)
                {
                    if (Display[currentPixel] == '1')
                    {
                        Console.BackgroundColor = TextColor;
                        Console.Write(" ");
                    }
                    else if (Display[currentPixel] == '0')
                    {
                        Console.BackgroundColor = BgColor;
                        Console.Write(" ");
                    }
                    else if (Display[currentPixel] == ';')
                    {
                        Console.BackgroundColor = BgColor;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.BackgroundColor = BgColor;
                        Console.Write(" ");
                    }
                }

                if (currentPixel == (contentLength + textAlignSpaces) * 1 - 1 ||
                    currentPixel == (contentLength + textAlignSpaces) * 2 - 1 ||
                    currentPixel == (contentLength + textAlignSpaces) * 3 - 1 ||
                    currentPixel == (contentLength + textAlignSpaces) * 4 - 1 ||
                    currentPixel == (contentLength + textAlignSpaces) * 5 - 1)
                {
                    currentRow++;
                    int secondSpace;

                    Console.BackgroundColor = BgColor;
                    Console.Write(new string(' ', (Console.WindowWidth - Console.CursorLeft)-1) + "\n");
                    Console.CursorLeft += displayAlignSpaces.Length;

                    if (currentRow >= 5)
                    {
                        lastCursorTop = Console.CursorTop;
                    }
                }
            }
        }
    }

}
