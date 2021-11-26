using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsolePuzzleGame.Pages
{
    public class Page
    {
        protected int HeaderRows = 5, FooterRows = 3, cursorTop, longestHLine = 0, longestFLine = 0, firstOptionIndex = 0;
        protected UI Header = new UI();
        protected UI Footer = new UI();
        public bool RunningThisPage = true;

        protected void WriteHeader()
        {
            //count the rows length and set the longest one
            for (int line = firstOptionIndex; line < Header.Line.Count; line++)
            {
                int lineLength = 0;
                for (int item = 0; item < Header.Line[line].Count; item++)
                {
                    lineLength += Header.Line[line][item].text.Length;
                }
                if (lineLength > longestHLine)
                    longestHLine = lineLength;
            }

            Console.CursorTop = cursorTop;
            Console.CursorLeft = 0;
            for (int line = 0; line < Header.Line.Count; line++)
            {
                bool centerLine = false;
                bool alignRows = false;
                int lineLength = 0;

                for (int item = 0; item < Header.Line[line].Count; item++)
                {
                    if (Header.Line[line][item].CenterLine)
                    {
                        centerLine = true;
                        foreach (var word in Header.Line[line])
                        {
                            lineLength += word.text.Length;
                        }
                        if (Header.Line[line][item].AlignRows)
                        {
                            alignRows = true;
                        }
                    }
                    if (MenuController.CurrentOptionIndex == line - firstOptionIndex)
                    {
                        if (Header.Line[line][0].Active)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = Header.Line[line][item].bgColor;
                        Console.ForegroundColor = Header.Line[line][0].Active ? Header.Line[line][item].fgColor : ConsoleColor.DarkGray;
                    }
                    if (centerLine && Header.Line[line][item] == Header.Line[line][0])
                    {
                        if (alignRows)
                        {
                            Console.Write(MenuController.CenterText(new string(' ', longestHLine+2), spacesOnly: true));
                        }
                        else
                        {
                            Console.Write(MenuController.CenterText(new string(' ', lineLength+2), spacesOnly: true));
                        }
                    }
                    Console.Write(Header.Line[line][item].text);
                }

                if (line >= firstOptionIndex)
                {
                    if (centerLine)
                    {
                        if (alignRows)
                        {
                            int previousLength = lineLength + MenuController.CenterText(new string(' ', longestHLine), spacesOnly: true).Length;
                            Console.Write(new string(' ', (Console.WindowWidth - previousLength)));
                        }
                        else
                        {
                            Console.Write(MenuController.CenterText(new string(' ', lineLength+1), spacesOnly: true));
                        }
                    }
                    else
                    {
                        Console.Write(new string(' ', (Console.WindowWidth - lineLength)-1));
                    }
                }
                centerLine = false;
                alignRows = false;
                Console.Write("\n");
            }
        }

        protected void WriteFooter()
        {
            //count the rows length and set the longest one
            for (int line = 0; line < Footer.Line.Count; line++)
            {
                int lineLength = 0;
                for (int item = 0; item < Footer.Line[line].Count; item++)
                {
                    lineLength += Footer.Line[line][item].text.Length;
                }
                if (lineLength > longestFLine)
                    longestFLine = lineLength;
            }

            for (int line = 0; line < Footer.Line.Count; line++)
            {
                bool centerLine = false;
                bool alignRows = false;
                int lineLength = 0;

                for (int item = 0; item < Footer.Line[line].Count; item++)
                {
                    if (Footer.Line[line][item].CenterLine)
                    {
                        centerLine = true;
                        foreach (var word in Footer.Line[line])
                        {
                            lineLength += word.text.Length;
                        }
                        if (Footer.Line[line][item].AlignRows)
                        {
                            alignRows = true;
                        }
                    }

                    //Console.BackgroundColor = ConsoleColor.Red;
                    //Console.Title = "wait...";
                    //    Console.ReadKey(true);
                    if (centerLine && Footer.Line[line][item] == Footer.Line[line][0])
                    {
                        if (alignRows)
                        {
                            Console.Write(MenuController.CenterText(new string(' ', longestFLine+2), spacesOnly: true));
                        }
                        else
                        {
                            Console.Write(MenuController.CenterText(new string(' ', lineLength+2), spacesOnly: true));
                        }
                    }
                    Console.BackgroundColor = Footer.Line[line][item].bgColor;
                    Console.ForegroundColor = Footer.Line[line][item].fgColor;
                    Console.Write(Footer.Line[line][item].text);
                }
                centerLine = false;
                alignRows = false;
                Console.Write("\n");
            }
        }
    }

    public class Blank : Page
    {

    }
}
