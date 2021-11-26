using System;
using System.Collections.Generic;
using System.Text;

namespace ConsolePuzzleGame
{
    public class GameObject
    {
        public Enum ObjectType;
        public int xPosition, yPosition;
        public ConsoleColor BracketsbgColor, BracketsfgColor, bgColor, fgColor;
        public string Brackets = "", Content = "", Value = "";
        public GameObject ObjectTypeUnderThis = null;

        public GameObject(Enum Type, int Xpos, int Ypos, string value = " ")
        {
            ObjectTypeUnderThis = Type.ToString() == GameController.Objects.Blank.ToString() ? null : new GameObject(GameController.Objects.Blank, 0, 0);
            if (ObjectTypeUnderThis != null)
                ObjectTypeUnderThis.Set();
            ObjectType = Type;
            xPosition = Xpos;
            yPosition = Ypos;
            Value = value;
        }

        public void Set()
        {
            Console.ResetColor();
            switch (ObjectType)
            {
                case GameController.Objects.Blank:
                    Brackets = "()";
                    Content = " ";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.DarkGray;
                    break;

                case GameController.Objects.Player:
                    Brackets = "()";
                    Content = "P";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Magenta;
                    break;

                case GameController.Objects.Player2:
                    Brackets = "()";
                    Content = "P";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.DarkBlue;
                    break;

                case GameController.Objects.Exit:
                    Brackets = "()";
                    Content = "@";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Green;
                    break;

                case GameController.Objects.Wall:
                    Brackets = "[]";
                    Content = Value;
                    BracketsbgColor = ConsoleColor.DarkGray;
                    BracketsfgColor = ConsoleColor.Black;
                    bgColor = ConsoleColor.DarkGray;
                    fgColor = ConsoleColor.Black;
                    break;

                case GameController.Objects.Coin:
                    Brackets = "()";
                    Content = "O";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Yellow;
                    break;

                case GameController.Objects.CoinDoor:
                    if (Value == " " || Value.Length < 1)
                        Value = "0";
                    Brackets = int.Parse(Value) - GameController.CollectedCoins <= 0 ? "{}" : "[]";
                    Content = Value;
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkYellow;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Yellow;
                    break;

                case GameController.Objects.Key:
                    Brackets = "()";
                    Content = Value;
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Cyan;
                    break;

                case GameController.Objects.KeyDoor:
                    Brackets = GameController.CollectedKeys.Contains(Value[0]) || GameController.PressedButtons.Contains(Value[0]) ? "{}" : "[]";
                    Content = Value.Length == 1 ? Value : "-";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkCyan;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Cyan;
                    break;

                case GameController.Objects.OneWay:
                    Brackets = "{}";
                    switch (Value)
                    {
                        case "up":
                            Content = "↑";
                            break;
                        case "right":
                            Content = ">";
                            break;
                        case "down":
                            Content = "↓";
                            break;
                        case "left":
                            Content = "<";
                            break;
                    }
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.Gray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Gray;
                    break;

                case GameController.Objects.Portal:
                    Brackets = "||";
                    Content = Value;
                    GameController.BoardPortals.Add(new Tuple<string, int, int>(Value, xPosition, yPosition));
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkBlue;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Blue;
                    break;

                case GameController.Objects.Box:
                    Brackets = "()";
                    Content = "#";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.DarkYellow;
                    break;

                case GameController.Objects.StandingButton:
                    Brackets = "()";
                    Content = "-";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.White;
                    bgColor = ConsoleColor.DarkGray;
                    fgColor = ConsoleColor.Gray;
                    break;

                case GameController.Objects.PlayerDoor:
                    Brackets = "[]";
                    Content = "p";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = Value == "1" ? ConsoleColor.Magenta : ConsoleColor.Blue;
                    bgColor = ConsoleColor.Black;
                    fgColor = Value == "1" ? ConsoleColor.DarkMagenta : ConsoleColor.DarkBlue;
                    break;

                case GameController.Objects.Enemy_Guard:
                    Brackets = "()";
                    Content = "X";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Red;
                    break;

                case GameController.Objects.Enemy_Cannon:
                    if (Game.BoardEnemies.ContainsKey(Value))
                    {
                        switch (Game.BoardEnemies[Value].Direction)
                        {
                            case "up":
                                Brackets = @"/\";
                                Content = "'";
                                break;
                            case "right":
                                Brackets = "[=";
                                Content = ")";
                                break;
                            case "down":
                                Brackets = @"\/";
                                Content = ".";
                                break;
                            case "left":
                                Brackets = "=]";
                                Content = "(";
                                break;
                        }
                    }
                    else
                    {
                        Brackets = "()";
                        Content = "?";
                    }
                    
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.Red;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Red;
                    break;

                case GameController.Objects.Enemy_CannonBullet:
                    Brackets = "()";
                    Content = "o";
                    BracketsbgColor = ConsoleColor.Black;
                    BracketsfgColor = ConsoleColor.DarkGray;
                    bgColor = ConsoleColor.Black;
                    fgColor = ConsoleColor.Red;
                    break;

                case GameController.Objects.Enemy_Seeker:
                    Brackets = "1";
                    Content = ".";
                    BracketsbgColor = ConsoleColor.DarkRed;
                    BracketsfgColor = ConsoleColor.Black;
                    bgColor = ConsoleColor.Red;
                    fgColor = ConsoleColor.Black;
                    break;
            }
        }
    }
}
