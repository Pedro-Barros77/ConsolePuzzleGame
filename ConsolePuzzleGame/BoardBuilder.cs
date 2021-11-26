using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ConsolePuzzleGame
{
    public class BoardBuilder
    {
        Stopwatch coinSW = new Stopwatch();
        bool coinSWBlocker = false, coinFrameOne = true;

        public ConsoleColor BorderColor = ConsoleColor.DarkGray;
        public void Build(Game GameLvl)
        {
            int renderStartingX = 0, renderStartingY = 0;
            int renderEndingX = 0, renderEndingY = 0;
            int yComplement = 0, xComplement = 0;

            //Y axis render limiter
            if (GameLvl.BoardVerticalField < Game.BoardObjects.Count)
            {

                if (GameLvl.PlayerYPos >= Game.BoardObjects.Count - GameLvl.BoardVerticalField / 2)
                {
                    renderStartingY = Game.BoardObjects.Count + 1 - GameLvl.BoardVerticalField;
                }
                else
                {
                    renderStartingY = GameLvl.PlayerYPos - (GameLvl.PlayerYPos < GameLvl.BoardVerticalField / 2 ? GameLvl.PlayerYPos : (GameLvl.BoardVerticalField / 2) - 1);
                }

                renderEndingY = Game.BoardObjects.Count - (GameLvl.PlayerYPos > Game.BoardObjects.Count - (GameLvl.BoardVerticalField / 2) ? 0 : Game.BoardObjects.Count - GameLvl.BoardVerticalField);

                if (GameLvl.PlayerYPos >= (GameLvl.BoardVerticalField % 2 == 0 ? GameLvl.BoardVerticalField : GameLvl.BoardVerticalField - 2) / 2
                && GameLvl.PlayerYPos < Game.BoardObjects.Count - 1 - GameLvl.BoardVerticalField / 2 &&
                GameLvl.BoardVerticalField < Game.BoardObjects.Count)
                {
                    yComplement = 1;
                    renderEndingY += yComplement;
                }
            }
            else
            {
                renderStartingY = 0;
                renderEndingY = Game.BoardObjects.Count;
            }

            //x axis render limiter
            if (GameLvl.BoardHorizontalField < Game.BoardObjects[0].Count)
            {

                if (GameLvl.PlayerXPos >= Game.BoardObjects[0].Count - GameLvl.BoardHorizontalField / 2)
                {
                    renderStartingX = Game.BoardObjects[0].Count + 1 - GameLvl.BoardHorizontalField;
                }
                else
                {
                    renderStartingX = GameLvl.PlayerXPos - (GameLvl.PlayerXPos < GameLvl.BoardHorizontalField / 2 ? GameLvl.PlayerXPos : (GameLvl.BoardHorizontalField / 2) - 1);
                }

                renderEndingX = (Game.BoardObjects[0].Count - 1) - (GameLvl.PlayerXPos > Game.BoardObjects[0].Count - (GameLvl.BoardHorizontalField / 2) ? 0 : (Game.BoardObjects[0].Count) - GameLvl.BoardHorizontalField);

                if (GameLvl.PlayerXPos + 1 >= (GameLvl.BoardHorizontalField % 2 == 0 ? GameLvl.BoardHorizontalField : GameLvl.BoardHorizontalField - 2) / 2
                && GameLvl.PlayerXPos < Game.BoardObjects[0].Count - 1 - GameLvl.BoardHorizontalField / 2 &&
                GameLvl.BoardHorizontalField < Game.BoardObjects[0].Count)
                {
                    xComplement = 1;
                    renderEndingX += xComplement;
                }
            }
            else
            {
                renderStartingX = 0;
                renderEndingX = Game.BoardObjects[0].Count;
            }




            int firstRenderedRow = 0, firstRenderedItem = 0;
            int boardCursorLeft = 0;
            bool Locked = false;

            for (int y = renderStartingY; y < renderEndingY + (y >= Game.BoardObjects.Count ? 0 : renderStartingY); y++)
            {
                bool visible = true;
                int lastX = 0;
                for (int x = renderStartingX; x < renderEndingX + (x >= Game.BoardObjects[0].Count ? 0 : renderStartingX); x++)
                {
                    if (!Locked)
                    {
                        firstRenderedRow = y;
                        firstRenderedItem = x;
                        Locked = true;
                    }

                    if (y + 1 < GameLvl.BoardVerticalField + yComplement + firstRenderedRow &&
                        (x + 1 < GameLvl.BoardHorizontalField + xComplement + firstRenderedItem) &&
                        //player 1
                        ((GameLvl.P1_YField == -1 || y + 1 <= GameLvl.P1_YField + GameLvl.PlayerYPos) &&
                        (GameLvl.P1_YField == -1 || y > -GameLvl.P1_YField + GameLvl.PlayerYPos)) &&
                        ((GameLvl.P1_XField == -1 || x + 1 <= GameLvl.P1_XField + GameLvl.PlayerXPos) &&
                        (GameLvl.P1_XField == -1 || x > -GameLvl.P1_XField + GameLvl.PlayerXPos)) ||

                        //player 2
                        (GameLvl.Multiplayer && ((
                        (GameLvl.P2_YField == -1 || y + 1 <= GameLvl.P2_YField + GameLvl.Player2YPos) &&
                        (GameLvl.P2_YField == -1 || y > -GameLvl.P2_YField + GameLvl.Player2YPos)) &&
                        ((GameLvl.P2_XField == -1 || x + 1 <= GameLvl.P2_XField + GameLvl.Player2XPos) &&
                        (GameLvl.P2_XField == -1 || x > -GameLvl.P2_XField + GameLvl.Player2XPos)))))
                    {
                        visible = true;
                    }
                    else
                    {
                        visible = false;
                    }

                    if (x == renderStartingX)
                    {
                        //writes at the first row: spaceToCenterBoard, leftBorder, topBorder, rightBorder
                        if (y == 0 && (GameLvl.PlayerYPos < ((GameLvl.BoardVerticalField % 2 == 0 ? GameLvl.BoardVerticalField : GameLvl.BoardVerticalField - 1) - 1) / 2
                            || GameLvl.BoardVerticalField >= Game.BoardObjects.Count))
                        {
                            if (GameLvl.BoardHorizontalField >= Game.BoardObjects[0].Count)
                                Console.CursorLeft = MenuController.CenterText(new string(' ', Game.BoardObjects[0].Count * 3 + 4), spacesOnly: true).Length;
                            //Console.Write(MenuController.CenterText(new string(' ', Game.BoardObjects[0].Count * 3 + 4), spacesOnly: true));

                            Console.BackgroundColor = BorderColor;
                            Console.Write("  ");

                            Console.Write(new string(' ', (GameLvl.BoardHorizontalField <= Game.BoardObjects[0].Count ? (GameLvl.BoardHorizontalField * 3) - 5 : Game.BoardObjects[0].Count * 3)));

                            Console.Write("  ");

                            Console.ResetColor();

                            //Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft));
                            Console.Write("\n");
                        }

                        //writes spaceToCenterBoard
                        if (GameLvl.BoardHorizontalField >= Game.BoardObjects[0].Count)
                            Console.Write(MenuController.CenterText(new string(' ', (Game.BoardObjects[0].Count * 3 + 4)), spacesOnly: true));


                        Console.BackgroundColor = y + 1 < GameLvl.BoardVerticalField + firstRenderedRow + yComplement ? BorderColor : ConsoleColor.Black;
                        boardCursorLeft = Console.CursorLeft;

                        if (GameLvl.PlayerXPos < ((GameLvl.BoardHorizontalField % 2 == 0 ? GameLvl.BoardHorizontalField : GameLvl.BoardHorizontalField - 1) - 1) / 2
                            || GameLvl.BoardHorizontalField >= Game.BoardObjects[0].Count)
                            Console.Write("  ");
                        Console.ResetColor();
                    }

                    if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.Enemy_Seeker.ToString())
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        if (Game.BoardObjects[y][x].Brackets == "0")
                            Console.BackgroundColor = Game.BoardObjects[y][x].bgColor;
                        else
                            Console.BackgroundColor = Game.BoardObjects[y][x].BracketsbgColor;
                    }
                    else
                    {
                        Console.BackgroundColor = Game.BoardObjects[y][x].BracketsbgColor;
                        Console.ForegroundColor = Game.BoardObjects[y][x].BracketsfgColor;
                    }

                    if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
                    {
                        if (int.Parse(Game.BoardObjects[y][x].Value) - GameController.CollectedCoins <= 0)
                        {
                            Game.BoardObjects[y][x].Brackets = ("{}");
                        }
                    }
                    if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString())
                    {
                        if (GameController.CollectedKeys.Contains(Game.BoardObjects[y][x].Value[0]) || GameController.PressedButtons.Contains(Game.BoardObjects[y][x].Value[0]) || Game.BoardObjects[y][x].Content == "P" || Game.BoardObjects[y][x].Content == "#")
                        {
                            Game.BoardObjects[y][x].Brackets = "{}";
                            Game.BoardObjects[y][x].Content = " ";
                        }
                        else
                        {
                            Game.BoardObjects[y][x].Brackets = "[]";
                            if (Game.BoardObjects[y][x].Value.Length > 1)
                                Game.BoardObjects[y][x].Content = "-";

                        }
                    }

                    if (visible)
                    {
                        if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.Enemy_Seeker.ToString())
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            if (Game.BoardObjects[y][x].Brackets == "0")
                            {
                                Console.Write(Game.BoardObjects[y][x].Content);
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                        else
                            Console.Write(Game.BoardObjects[y][x].Brackets[0]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        if (y + 1 < GameLvl.BoardVerticalField + firstRenderedRow &&
                            x + 1 < GameLvl.BoardHorizontalField + firstRenderedItem)
                            Console.Write(" ");
                    }

                    if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.Enemy_Seeker.ToString())
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        if (Game.BoardObjects[y][x].Brackets == "1")
                        {
                            Console.BackgroundColor = Game.BoardObjects[y][x].bgColor;
                        }
                        else
                        {
                            Console.BackgroundColor = Game.BoardObjects[y][x].BracketsbgColor;
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = Game.BoardObjects[y][x].bgColor;
                        Console.ForegroundColor = Game.BoardObjects[y][x].fgColor;
                    }
                    string content = "";
                    if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.Coin.ToString())
                    {
                        string frameText = GameController.CurrentFrame.ToString();
                        int frameDigit = int.Parse(frameText.Substring(frameText.Length - 1));

                        if (coinFrameOne)
                        {
                            if (y % 2 == 0)
                            {
                                content = x % 2 == 0 ? content = "$" : content = "|";
                            }
                            else
                            {
                                content = x % 2 == 0 ? content = "|" : content = "$";
                            }

                        }
                        else
                        {
                            if (y % 2 == 0)
                            {
                                content = x % 2 == 0 ? content = "|" : content = "$";
                            }
                            else
                            {
                                content = x % 2 == 0 ? content = "$" : content = "|";
                            }

                        }

                        if (!coinSWBlocker)
                        {
                            coinSW.Start();
                            coinSWBlocker = true;
                        }
                        if (coinSW.Elapsed >= TimeSpan.FromMilliseconds(150))
                        {
                            coinFrameOne = coinFrameOne ? false : true;
                            coinSWBlocker = false;
                            coinSW.Stop();
                            coinSW.Reset();
                        }
                    }
                    else
                    {
                        if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
                        {
                            if (int.Parse(Game.BoardObjects[y][x].Value) - GameController.CollectedCoins > 0)
                                content = (int.Parse(Game.BoardObjects[y][x].Value) - GameController.CollectedCoins).ToString();
                            else
                                content = " ";
                        }
                        else if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString())
                        {
                            if (GameController.CollectedKeys.Contains(Game.BoardObjects[y][x].Value[0]))
                            {
                                Game.BoardObjects[y][x].Content = " ";
                            }
                            content = (Game.BoardObjects[y][x].Content);
                        }
                        else
                        {
                            content = (Game.BoardObjects[y][x].Content);
                        }
                    }
                    if (visible)
                    {
                        if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.Enemy_Seeker.ToString())
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            if (Game.BoardObjects[y][x].Brackets == "1")
                            {
                                Console.Write(content);
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                        else
                        {
                            Console.Write(content);
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        if (y + 1 < GameLvl.BoardVerticalField + firstRenderedRow &&
                            x + 1 < GameLvl.BoardHorizontalField + firstRenderedItem)
                            Console.Write(" ");
                    }

                    if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.Enemy_Seeker.ToString())
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        if (Game.BoardObjects[y][x].Brackets == "2")
                            Console.BackgroundColor = Game.BoardObjects[y][x].bgColor;
                        else
                            Console.BackgroundColor = Game.BoardObjects[y][x].BracketsbgColor;
                    }
                    else
                    {
                        Console.BackgroundColor = Game.BoardObjects[y][x].BracketsbgColor;
                        Console.ForegroundColor = Game.BoardObjects[y][x].BracketsfgColor;
                    }

                    if (visible)
                    {
                        if (Game.BoardObjects[y][x].ObjectType.ToString() == GameController.Objects.Enemy_Seeker.ToString())
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            if (Game.BoardObjects[y][x].Brackets == "2")
                            {
                                Console.Write(Game.BoardObjects[y][x].Content);
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                        else
                        {
                            Console.Write(Game.BoardObjects[y][x].Brackets[1]);
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        if (y + 1 < GameLvl.BoardVerticalField + firstRenderedRow &&
                            x + 1 < GameLvl.BoardHorizontalField + firstRenderedItem)
                            Console.Write(" ");
                    }

                    if (x == Game.BoardObjects[0].Count - 1 || x == firstRenderedItem + GameLvl.BoardHorizontalField + 1)
                    {
                        Console.BackgroundColor = x + 1 < GameLvl.BoardHorizontalField + xComplement + firstRenderedItem && y + 1 < GameLvl.BoardVerticalField + yComplement + firstRenderedRow ? BorderColor : ConsoleColor.Black;

                        Console.Write("  ");
                        Console.ResetColor();
                        Console.Write(new string(' ', (Console.WindowWidth - Console.CursorLeft) - 1));

                    }

                    if (GameLvl.PlayerXPos < (GameLvl.BoardHorizontalField / 2) - 1 && GameLvl.BoardHorizontalField < Game.BoardObjects[0].Count)
                    {
                        int lastCursorLeft = Console.CursorLeft;
                        Console.CursorLeft = (GameLvl.BoardHorizontalField * 3) - 1;
                        Console.Write(" ");
                        Console.CursorLeft = lastCursorLeft;
                    }

                    Console.ResetColor();
                    lastX = x;
                }
                if (y + 1 < GameLvl.BoardVerticalField + firstRenderedRow + yComplement)
                {
                    if (y == Game.BoardObjects.Count - 1)
                    {
                        Console.Write("\n");
                        if (GameLvl.BoardHorizontalField >= Game.BoardObjects[0].Count)
                            Console.Write(MenuController.CenterText(new string(' ', (Game.BoardObjects[0].Count * 3 + 4)), spacesOnly: true));
                        Console.BackgroundColor = y + 1 < GameLvl.BoardVerticalField + firstRenderedRow ? BorderColor : ConsoleColor.Black;

                        Console.Write("  ");


                        Console.Write(new string(' ', (GameLvl.BoardHorizontalField <= Game.BoardObjects[0].Count ? (GameLvl.BoardHorizontalField * 3) - 5 : (Game.BoardObjects[0].Count * 3))));

                        Console.Write("  ");

                        Console.ResetColor();
                        Console.Write(new string(' ', (Console.WindowWidth - Console.CursorLeft) - 1));
                    }
                }
                Console.ResetColor();
                if (y == GameLvl.BoardVerticalField - 1 + firstRenderedRow + yComplement || y == Game.BoardObjects.Count)
                {
                    int previousCursorLeft = Console.CursorLeft;
                    Console.CursorLeft = boardCursorLeft;
                    Console.Write(new string(' ', (Game.BoardObjects[0].Count * 3 + 4) - 1));
                    Console.CursorLeft = previousCursorLeft;
                }
                Console.Write("\n");
            }
        }

        public enum FillType
        {
            SolidBox,
            HollowBox,
            PointToPoint,
            DrawnGrid
        }

        public void Fill(Enum gameObject, List<List<GameObject>> boardObjects, int fromX, int fromY, int toX, int toY, Enum fillType, string value = null, string grid = "")
        {
            switch (fillType)
            {
                case FillType.SolidBox:
                    for (int currentY = fromY; currentY <= toY; currentY++)
                    {
                        for (int currentX = fromX; currentX <= toX; currentX++)
                        {
                            boardObjects[currentY][currentX].ObjectType = gameObject;
                            if (value != null)
                            {
                                boardObjects[currentY][currentX].Value = value;
                                boardObjects[currentY][currentX].Content = value;
                            }
                            boardObjects[currentY][currentX].Set();
                        }
                    }
                    break;

                case FillType.HollowBox:
                    for (int currentY = fromY; currentY <= toY; currentY++)
                    {
                        for (int currentX = fromX; currentX <= toX; currentX++)
                        {
                            if ((currentX == fromX || currentX == toX) || (currentY == fromY || currentY == toY))
                            {
                                boardObjects[currentY][currentX].ObjectType = gameObject;
                                if (value != null)
                                {
                                    boardObjects[currentY][currentX].Value = value;
                                    boardObjects[currentY][currentX].Content = value;
                                }
                                boardObjects[currentY][currentX].Set();
                            }
                        }
                    }
                    break;

                case FillType.PointToPoint:
                    for (int currentY = fromY; currentY <= toY; currentY++)
                    {
                        int currentX = currentY == fromY ? fromX : 0;
                        for (; currentX < boardObjects[0].Count; currentX++)
                        {
                            if (currentY == toY)
                            {
                                if (currentX <= toX)
                                {
                                    boardObjects[currentY][currentX].ObjectType = gameObject;
                                    if (value != null)
                                    {
                                        boardObjects[currentY][currentX].Value = value;
                                        boardObjects[currentY][currentX].Content = value;
                                    }
                                    boardObjects[currentY][currentX].Set();
                                }
                            }
                            else
                            {
                                boardObjects[currentY][currentX].ObjectType = gameObject;
                                if (value != null)
                                {
                                    boardObjects[currentY][currentX].Value = value;
                                    boardObjects[currentY][currentX].Content = value;
                                }
                                boardObjects[currentY][currentX].Set();
                            }
                        }
                    }
                    break;

                case FillType.DrawnGrid:
                    int startY = fromY;
                    int startX = fromX;
                    for (int currentY = fromY, currentChar = 0; currentY < (startY + toY); currentY++)
                    {
                        for (int currentX = fromX; currentX < (startX + toX); currentX++, currentChar++)
                        {
                            if (grid[currentChar] != '*')
                            {
                                switch (grid[currentChar].ToString().ToLower()[0])
                                {
                                    case 'x':
                                        boardObjects[currentY][currentX].ObjectType = gameObject;
                                        break;

                                    case 'w':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.Wall;
                                        break;

                                    case '$':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.Coin;
                                        break;

                                    case '↑':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.OneWay;
                                        boardObjects[currentY][currentX].Value = "up";
                                        break;
                                    case '>':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.OneWay;
                                        boardObjects[currentY][currentX].Value = "right";
                                        break;
                                    case '↓':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.OneWay;
                                        boardObjects[currentY][currentX].Value = "down";
                                        break;
                                    case '<':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.OneWay;
                                        boardObjects[currentY][currentX].Value = "left";
                                        break;

                                    case '#':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.Box;
                                        break;

                                    case '1':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.PlayerDoor;
                                        boardObjects[currentY][currentX].Value = "1";
                                        break;
                                    case '2':
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.PlayerDoor;
                                        boardObjects[currentY][currentX].Value = "2";
                                        break;

                                    default:
                                        boardObjects[currentY][currentX].ObjectType = GameController.Objects.Blank;
                                        break;
                                }

                                if (value != null && grid[currentChar].ToString().ToLower()[0] == 'x')
                                {
                                    boardObjects[currentY][currentX].Value = value;
                                    boardObjects[currentY][currentX].Content = value;
                                }

                                if(char.IsLetter(grid[currentChar]) && grid[currentChar].ToString().ToLower()[0] != 'x' && grid[currentChar].ToString().ToLower()[0] != 'w')
                                {
                                    boardObjects[currentY][currentX].ObjectType = GameController.Objects.Key;
                                    boardObjects[currentY][currentX].Value = grid[currentChar].ToString();
                                }

                                boardObjects[currentY][currentX].Set();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
