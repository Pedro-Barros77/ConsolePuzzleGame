using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ConsolePuzzleGame
{
    public class Game
    {
        public static List<List<GameObject>> BoardObjects = new List<List<GameObject>>();
        public static Dictionary<string, Enemy> BoardEnemies = new Dictionary<string, Enemy>();

        public BoardBuilder Board = new BoardBuilder();

        public int BoardWidth = 4, BoardHeight = 4;
        public int PlayerXPos, PlayerYPos, ExitXPos, ExitYPos;
        public int Player2XPos, Player2YPos;
        public bool LevelCompleted = false, Multiplayer = false;

        public string Title;

        public int boardCursorLeft, boardCursorTop;

        public int maxBoardWidth = Console.BufferWidth - 4;

        public GameObject ObjectUnderPlayer1;
        public GameObject ObjectUnderPlayer2;

        public UI Header, Footer;
        public int HeaderRows, FooterRows, BoardVerticalField, BoardHorizontalField;
        public int P1_XField, P1_YField, P2_XField, P2_YField;

        public Level ActiveLevel;
        bool player1OnPortal = false, player2OnPortal = false;

        public Game(int Width, int Height, Level activeLevel)
        {
            if (Width < 0)
            {
                while (maxBoardWidth % 3 != 0)
                {
                    maxBoardWidth--;
                }
                maxBoardWidth = maxBoardWidth / 3;
                BoardWidth = maxBoardWidth;
            }
            else
            {
                BoardWidth = Width;
            }

            BoardHeight = Height;
            GameController.Playing = true;
            ActiveLevel = activeLevel;
        }

        public void ResetLevelData()
        {
            GameController.CollectedCoins = 0;
            GameController.CollectedKeys.Clear();
            GameController.PressedButtons.Clear();
            GameController.BoardPortals = new List<Tuple<string, int, int>>();
            BoardObjects.Clear();
            BoardEnemies.Clear();
        }

        public void FillBoardWithBlank()
        {
            for (int y = 0; y < BoardHeight; y++)
            {
                BoardObjects.Add(new List<GameObject>());
                for (int x = 0; x < BoardWidth; x++)
                {
                    BoardObjects[y].Add(new GameObject(GameController.Objects.Blank, x, y));
                    BoardObjects[y][x].Set();
                }
            }
        }

        public void AddObject(GameObject obj)
        {
            BoardObjects[obj.yPosition][obj.xPosition] = obj;
            BoardObjects[obj.yPosition][obj.xPosition].Set();

            if (obj.ObjectType.ToString().StartsWith("Enemy"))
            {
                Enemy thisEnemy = null;

                switch (obj.ObjectType)
                {
                    case GameController.Objects.Enemy_Guard:
                        thisEnemy = new Guard(obj.Value, obj.xPosition, obj.yPosition, this);
                        break;
                    case GameController.Objects.Enemy_Cannon:
                        thisEnemy = new Cannon(obj.Value, obj.xPosition, obj.yPosition, this);
                        break;
                    case GameController.Objects.Enemy_Seeker:
                        thisEnemy = new Seeker(obj.Value, obj.xPosition, obj.yPosition, this);
                        break;
                }
                BoardEnemies.Add(obj.Value, thisEnemy);
            }
        }

        public void WriteUI(bool header)
        {
            if (header)
            {
                for (int line = 0; line < Header.Line.Count; line++)
                {
                    bool centerLine = false;
                    for (int item = 0; item < Header.Line[line].Count; item++)
                    {
                        int lineLength = 0;
                        if (Header.Line[line][item].CenterLine)
                        {
                            centerLine = true;
                            foreach (var word in Header.Line[line])
                            {
                                lineLength += word.text.Length;
                            }
                        }
                        Console.BackgroundColor = Header.Line[line][item].bgColor;
                        Console.ForegroundColor = Header.Line[line][item].fgColor;
                        if (centerLine)
                        {
                            Console.Write(MenuController.CenterText(new string(' ', lineLength), spacesOnly: true));
                            centerLine = false;
                        }
                        Console.Write(Header.Line[line][item].text);
                        //if (centerLine && item == Header.Line[line].Count-1)
                        //{
                        //    Console.Write(MenuController.CenterText(new string(' ', lineLength), spacesOnly: true));
                        //    centerLine = false;
                        //}
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                for (int line = 0; line < Footer.Line.Count; line++)
                {
                    bool centerLine = false;
                    for (int item = 0; item < Footer.Line[line].Count; item++)
                    {
                        int lineLength = 0;
                        if (Footer.Line[line][item].CenterLine)
                        {
                            centerLine = true;
                            foreach (var word in Footer.Line[line])
                            {
                                lineLength += word.text.Length;
                            }
                        }
                        Console.BackgroundColor = Footer.Line[line][item].bgColor;
                        Console.ForegroundColor = Footer.Line[line][item].fgColor;
                        if (centerLine && item == 0)
                        {
                            Console.Write(MenuController.CenterText(new string(' ', lineLength), spacesOnly: true));
                        }
                        Console.Write(Footer.Line[line][item].text);
                        if (centerLine && item == Footer.Line[line].Count - 1)
                        {
                            Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft));
                        }
                    }
                    Console.Write("\n");
                }
            }
        }

        public void GetPressedKey(ConsoleKey dirKey)
        {
            switch (dirKey)
            {
                case ConsoleKey.UpArrow:
                    if (PlayerYPos > 0)
                        ValidateDestination(0, -1, 1);
                    break;

                case ConsoleKey.RightArrow:
                    if (PlayerXPos < BoardWidth - 1)
                        ValidateDestination(1, 0, 1);
                    break;

                case ConsoleKey.DownArrow:
                    if (PlayerYPos < BoardHeight - 1)
                        ValidateDestination(0, 1, 1);
                    break;

                case ConsoleKey.LeftArrow:
                    if (PlayerXPos > 0)
                        ValidateDestination(-1, 0, 1);
                    break;




                case ConsoleKey.W:
                    if (Multiplayer && Player2YPos > 0)
                        ValidateDestination(0, -1, 2);

                    else if (!Multiplayer && PlayerYPos > 0)
                        ValidateDestination(0, -1, 1);
                    break;

                case ConsoleKey.D:
                    if (Multiplayer && Player2XPos < BoardWidth - 1)
                        ValidateDestination(1, 0, 2);

                    else if (!Multiplayer && PlayerXPos < BoardWidth - 1)
                        ValidateDestination(1, 0, 1);
                    break;

                case ConsoleKey.S:
                    if (Multiplayer && Player2YPos < BoardHeight - 1)
                        ValidateDestination(0, 1, 2);

                    else if (!Multiplayer && PlayerYPos < BoardHeight - 1)
                        ValidateDestination(0, 1, 1);
                    break;

                case ConsoleKey.A:
                    if (Multiplayer && Player2XPos > 0)
                        ValidateDestination(-1, 0, 2);

                    else if (!Multiplayer && PlayerXPos > 0)
                        ValidateDestination(-1, 0, 1);
                    break;


                case ConsoleKey.R:
                    GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
                    GameController.consoleKey = ConsoleKey.K;
                    GameController.CtrlKeyHeld = false;
                    GameController.RestartLevel(ActiveLevel);
                    break;

                case ConsoleKey.P:
                    GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
                    GameController.consoleKey = ConsoleKey.K;
                    MenuController.PauseGame(ActiveLevel, new Pages.Blank());
                    break;
            }
            GameController.keyInfo = new ConsoleKeyInfo((char)ConsoleKey.K, ConsoleKey.K, false, false, false);
            GameController.consoleKey = ConsoleKey.K;
            GameController.CtrlKeyHeld = false;
        }

        void ValidateDestination(int x, int y, int player)
        {
            GameObject Destination = player == 1 ? BoardObjects[PlayerYPos + y][PlayerXPos + x] : BoardObjects[Player2YPos + y][Player2XPos + x];
            Enum objTypeUnderPlayer = player == 1 ? ObjectUnderPlayer1.ObjectType : ObjectUnderPlayer2.ObjectType;
            int playerX = player == 1 ? PlayerXPos : Player2XPos;
            int playerY = player == 1 ? PlayerYPos : Player2YPos;

            bool canPushBox = true;
            bool canPullBox = true;

            if (Destination.ObjectType.ToString() != GameController.Objects.Portal.ToString())
            {
                if (player == 1)
                    player1OnPortal = false;
                else
                    player2OnPortal = false;
            }

            if (playerX + (x * 2) <= BoardWidth - 1 && playerX + (x * 2) >= 0 && playerY + (y * 2) <= BoardHeight - 1 && playerY + (y * 2) >= 0)
            {
                if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString())
                {
                    if (!GameController.CollectedKeys.Contains(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]) &&
                        !GameController.PressedButtons.Contains(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]))
                        canPushBox = false;
                }
                else if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
                {
                    if (GameController.CollectedCoins < int.Parse(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0].ToString()))
                        canPushBox = false;
                }
            }

            if (objTypeUnderPlayer.ToString() == GameController.Objects.PlayerDoor.ToString())
            {
                canPullBox = false;
            }

            if (Destination.ObjectType.ToString() == GameController.Objects.KeyDoor.ToString())
            {
                if (!GameController.CollectedKeys.Contains(Destination.Value[0]) &&
                    !GameController.PressedButtons.Contains(Destination.Value[0]))
                    canPullBox = false;
            }
            else if (Destination.ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
            {
                if (GameController.CollectedCoins < int.Parse(Destination.Value[0].ToString()))
                    canPullBox = false;
            }

            bool moveBox = false;
            if (playerX + (-x) <= BoardWidth - 1 && playerX + (-x) >= 0 && playerY + (-y) <= BoardHeight - 1 && playerY + (-y) >= 0)
            {
                if (GameController.CtrlKeyHeld && BoardObjects[playerY + (-y)][playerX + (-x)].ObjectType.ToString() == GameController.Objects.Box.ToString())
                {
                    if ((Destination.ObjectType.ToString() == GameController.Objects.Blank.ToString() ||
                       Destination.ObjectType.ToString() == GameController.Objects.StandingButton.ToString() ||
                       Destination.ObjectType.ToString() == GameController.Objects.KeyDoor.ToString() ||
                       Destination.ObjectType.ToString() == GameController.Objects.CoinDoor.ToString() ||
                       (Destination.ObjectType.ToString() == GameController.Objects.PlayerDoor.ToString() && player.ToString() == Destination.Value[0].ToString())) && canPullBox)
                    {
                        if (BoardObjects[playerY + (-y)][playerX + (-x)].ObjectTypeUnderThis == null)
                        {
                            BoardObjects[playerY + (-y)][playerX + (-x)].ObjectType = GameController.Objects.Blank;
                            BoardObjects[playerY + (-y)][playerX + (-x)].Set();
                        }
                        else
                        {
                            BoardObjects[playerY + (-y)][playerX + (-x)] = BoardObjects[playerY + (-y)][playerX + (-x)].ObjectTypeUnderThis;
                            BoardObjects[playerY + (-y)][playerX + (-x)].Set();
                            moveBox = true;
                        }
                        if (BoardObjects[playerY + (-y)][playerX + (-x)].ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
                        {
                            if (GameController.PressedButtons.Contains(BoardObjects[playerY + (-y)][playerX + (-x)].Value[0]))
                            {
                                GameController.PressedButtons.Remove(BoardObjects[playerY + (-y)][playerX + (-x)].Value[0]);
                            }
                        }
                    }
                }
            }

            switch (Destination.ObjectType)
            {
                case GameController.Objects.Blank:
                    if (player == 1)
                        Move(x, y);
                    else
                        Move2(x, y);
                    break;

                case GameController.Objects.Exit:
                    if (player == 1)
                        Move(x, y);
                    else
                        Move2(x, y);
                    LevelCompleted = true;
                    GameController.Playing = false;
                    break;

                case GameController.Objects.Coin:
                    GameController.CollectedCoins++;
                    if (player == 1)
                        Move(x, y);
                    else
                        Move2(x, y);
                    if (MenuController.SoundFX)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Console.Beep(2200, 200);
                        });
                    }
                    break;

                case GameController.Objects.CoinDoor:
                    if (int.Parse(Destination.Value) - GameController.CollectedCoins <= 0)
                    {
                        if (player == 1)
                            Move(x, y);
                        else
                            Move2(x, y);
                    }
                    break;

                case GameController.Objects.Key:
                    if (!GameController.CollectedKeys.Contains(Destination.Value[0]))
                    {
                        GameController.CollectedKeys.Add(Destination.Value[0]);
                    }
                    if (player == 1)
                        Move(x, y);
                    else
                        Move2(x, y);
                    break;

                case GameController.Objects.KeyDoor:
                    if (GameController.CollectedKeys.Contains(Destination.Value[0]) || GameController.PressedButtons.Contains(Destination.Value[0]))
                        if (player == 1)
                            Move(x, y);
                        else
                            Move2(x, y);
                    break;

                case GameController.Objects.OneWay:
                    {
                        if (playerX + (x * 2) <= BoardWidth - 1 && playerX + (x * 2) >= 0 && playerY + (y * 2) <= BoardHeight - 1 && playerY + (y * 2) >= 0)
                        {
                            bool canJump = true;
                            if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Player.ToString()
                                || BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Player2.ToString()
                                || BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Wall.ToString()
                                || BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.OneWay.ToString()
                                || BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Box.ToString()
                                || BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Portal.ToString()) { canJump = false; }

                            if ((BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString() && GameController.CollectedCoins < int.Parse(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value))
                                || (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString() && !GameController.CollectedKeys.Contains(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]) && !GameController.PressedButtons.Contains(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]))
                                || (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.PlayerDoor.ToString() && (player == 1 && BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value == "1" || player != 1 && BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value != "1")))
                            {
                                canJump = false;
                            }

                            if ((Destination.Value == "up" && y == -1) || (Destination.Value == "right" && x == 1) || (Destination.Value == "down" && y == 1) || (Destination.Value == "left" && x == -1))
                            {
                                if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Coin.ToString() && canJump)
                                {
                                    if (MenuController.SoundFX)
                                    {
                                        Task.Factory.StartNew(() =>
                                        {
                                            Console.Beep(2200, 200);
                                        });
                                    }
                                    GameController.CollectedCoins++;
                                }
                                if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Key.ToString() && canJump)
                                {
                                    if (!GameController.CollectedKeys.Contains(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]))
                                    {
                                        GameController.CollectedKeys.Add(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]);
                                    }
                                }


                                if (canJump)
                                {
                                    if (player == 1)
                                        Move(x * 2, y * 2);
                                    else
                                        Move2(x, y);
                                }
                            }
                        }
                    }
                    break;

                case GameController.Objects.Portal:
                    {
                        bool hasDestination = false;
                        Tuple<int, int> destPortalPos = new Tuple<int, int>(0, 0);

                        foreach (Tuple<string, int, int> t in GameController.BoardPortals)
                        {
                            if (char.IsUpper(Destination.Value[0]))
                            {
                                if (t.Item1 == Destination.Value.ToLower())
                                {
                                    hasDestination = true;
                                    destPortalPos = new Tuple<int, int>(t.Item2, t.Item3);
                                }
                            }
                            else if (char.IsLower(Destination.Value[0]))
                            {
                                if (t.Item1 == Destination.Value.ToUpper())
                                {
                                    hasDestination = true;
                                    destPortalPos = new Tuple<int, int>(t.Item2, t.Item3);
                                }
                            }
                        }

                        if (hasDestination & (!player1OnPortal && !player2OnPortal))
                        {
                            if (player == 1)
                            {
                                player1OnPortal = true;
                                Move(destPortalPos.Item1 - playerX, destPortalPos.Item2 - playerY);
                            }
                            else
                            {
                                player2OnPortal = true;
                                Move2(destPortalPos.Item1 - playerX, destPortalPos.Item2 - playerY);
                            }
                        }
                    }
                    break;

                case GameController.Objects.Box:
                    {
                        if (playerX + (x * 2) <= BoardWidth - 1 && playerX + (x * 2) >= 0 && playerY + (y * 2) <= BoardHeight - 1 && playerY + (y * 2) >= 0)
                        {
                            if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.Blank.ToString() ||
                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.StandingButton.ToString() ||
                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString() ||
                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
                            {
                                if (!canPushBox)
                                    break;

                                //creates a template of the object that the box will cover
                                GameObject objUnderBoxTemplate = new GameObject(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType, BoardObjects[playerY + (y * 2)][playerX + (x * 2)].xPosition, BoardObjects[playerY + (y * 2)][playerX + (x * 2)].yPosition, BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value);
                                objUnderBoxTemplate.Set();

                                //sets box to it's destination
                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectType = Destination.ObjectType;
                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Set();

                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectTypeUnderThis = objUnderBoxTemplate;

                                //keeps object under box configuration after setting the box
                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].BracketsbgColor = objUnderBoxTemplate.BracketsbgColor;
                                if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
                                {
                                    BoardObjects[playerY + (y * 2)][playerX + (x * 2)].BracketsfgColor = ConsoleColor.Green;
                                    if (!GameController.PressedButtons.Contains(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]))
                                    {
                                        GameController.PressedButtons.Add(BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Value[0]);
                                    }
                                }
                                else
                                {
                                    BoardObjects[playerY + (y * 2)][playerX + (x * 2)].BracketsfgColor = objUnderBoxTemplate.BracketsfgColor;
                                }

                                if (BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.KeyDoor.ToString() ||
                                    BoardObjects[playerY + (y * 2)][playerX + (x * 2)].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
                                {
                                    BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Brackets = "{}";
                                }
                                else
                                {
                                    BoardObjects[playerY + (y * 2)][playerX + (x * 2)].Brackets = objUnderBoxTemplate.Brackets;
                                }
                                BoardObjects[playerY + (y * 2)][playerX + (x * 2)].bgColor = objUnderBoxTemplate.bgColor;


                                BoardObjects[playerY + y][playerX + x] = Destination.ObjectTypeUnderThis;
                                BoardObjects[playerY + y][playerX + x].Set();

                                if (player == 1)
                                    Move(x, y);
                                else
                                    Move2(x, y);
                            }
                        }
                    }
                    break;

                case GameController.Objects.StandingButton:
                    if (player == 1)
                        Move(x, y);
                    else
                        Move2(x, y);
                    break;

                case GameController.Objects.PlayerDoor:
                    if (player == 1 && Destination.Value == "1")
                        Move(x, y);
                    else if (player == 2 && Destination.Value == "2")
                        Move2(x, y);
                    break;

                case GameController.Objects.Enemy_Guard:
                    {
                        if (player == 1)
                        {
                            BoardObjects[playerY][playerX].Content = " ";

                            Console.SetCursorPosition(boardCursorLeft, boardCursorTop);
                            Board.Build(this);
                            KillPlayer(1);
                        }
                        else
                        {
                            BoardObjects[playerY][playerX].Content = " ";

                            Console.SetCursorPosition(boardCursorLeft, boardCursorTop);
                            Board.Build(this);
                            KillPlayer(2);
                        }
                    }
                    break;

                case GameController.Objects.Enemy_CannonBullet:
                    {
                        if (player == 1)
                        {
                            BoardObjects[playerY][playerX].Content = " ";

                            Console.SetCursorPosition(boardCursorLeft, boardCursorTop);
                            Board.Build(this);
                            KillPlayer(1);
                        }
                        else
                        {
                            BoardObjects[playerY][playerX].Content = " ";

                            Console.SetCursorPosition(boardCursorLeft, boardCursorTop);
                            Board.Build(this);
                            KillPlayer(2);
                        }
                    }
                    break;
            }
            if (moveBox)
            {
                if (BoardObjects[playerY][playerX].ObjectType.ToString() == GameController.Objects.Blank.ToString() ||
                    BoardObjects[playerY][playerX].ObjectType.ToString() == GameController.Objects.StandingButton.ToString() ||
                    BoardObjects[playerY][playerX].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString() ||
                    BoardObjects[playerY][playerX].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
                {
                    GameObject objUnderBoxTemplate = new GameObject(BoardObjects[playerY][playerX].ObjectType, BoardObjects[playerY][playerX].xPosition, BoardObjects[playerY][playerX].yPosition, BoardObjects[playerY][playerX].Value);
                    objUnderBoxTemplate.Set();

                    BoardObjects[playerY][playerX].ObjectType = GameController.Objects.Box;
                    BoardObjects[playerY][playerX].Set();
                    BoardObjects[playerY][playerX].ObjectTypeUnderThis = objUnderBoxTemplate;
                    BoardObjects[playerY][playerX].ObjectTypeUnderThis.Set();

                    BoardObjects[playerY][playerX].BracketsbgColor = objUnderBoxTemplate.BracketsbgColor;
                    if (BoardObjects[playerY][playerX].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
                    {
                        BoardObjects[playerY][playerX].BracketsfgColor = ConsoleColor.Green;
                        if (!GameController.PressedButtons.Contains(BoardObjects[playerY][playerX].Value[0]))
                        {
                            GameController.PressedButtons.Add(BoardObjects[playerY][playerX].Value[0]);
                        }
                    }
                    else
                    {
                        BoardObjects[playerY][playerX].BracketsfgColor = objUnderBoxTemplate.BracketsfgColor;
                        if (playerY + (-y) >= 0 && playerY + (-y) <= BoardHeight - 1 && playerX + (-x) >= 0 && playerX + (-x) <= BoardWidth - 1)
                        {
                            if (BoardObjects[playerY + (-y)][playerX + (-x)].ObjectTypeUnderThis != null && BoardObjects[playerY + (-y)][playerX + (-x)].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
                            {
                                if (GameController.PressedButtons.Contains(BoardObjects[playerY + (-y)][playerX + (-x)].Value[0]))
                                    GameController.PressedButtons.Remove(BoardObjects[playerY + (-y)][playerX + (-x)].Value[0]);
                            }
                        }
                    }
                    if (BoardObjects[playerY][playerX].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.KeyDoor.ToString() ||
                        BoardObjects[playerY][playerX].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.CoinDoor.ToString())
                    {
                        BoardObjects[playerY][playerX].Brackets = "{}";
                    }
                    else
                    {
                        BoardObjects[playerY][playerX].Brackets = objUnderBoxTemplate.Brackets;
                    }
                    BoardObjects[playerY][playerX].bgColor = objUnderBoxTemplate.bgColor;

                    moveBox = false;
                }
            }
        }

        public void Move(int x, int y)
        {
            GameObject destinationObject = BoardObjects[PlayerYPos + y][PlayerXPos + x];
            GameObject previousObject = ObjectUnderPlayer1;

            ObjectUnderPlayer1 = new GameObject(destinationObject.ObjectType, destinationObject.xPosition, destinationObject.yPosition);

            if (GameController.ObjToKeepContent.Contains(previousObject.ObjectType))
            {
                BoardObjects[PlayerYPos][PlayerXPos].ObjectType = previousObject.ObjectType;
                BoardObjects[PlayerYPos][PlayerXPos].Set();
                if (previousObject.ObjectType.ToString() == GameController.Objects.Coin.ToString() ||
                    previousObject.ObjectType.ToString() == GameController.Objects.Key.ToString())
                {
                    BoardObjects[PlayerYPos][PlayerXPos].Content = " ";
                }
            }
            else
            {
                BoardObjects[PlayerYPos][PlayerXPos].ObjectType = GameController.Objects.Blank;
                BoardObjects[PlayerYPos][PlayerXPos].Set();
            }

            GameObject NewDestinationObj = new GameObject(destinationObject.ObjectType, destinationObject.xPosition, destinationObject.yPosition, destinationObject.Value);
            NewDestinationObj.Set();

            BoardObjects[PlayerYPos + y][PlayerXPos + x].ObjectType = GameController.Objects.Player;
            BoardObjects[PlayerYPos + y][PlayerXPos + x].Set();

            if (BoardObjects[PlayerYPos + y][PlayerXPos + x].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString())
            {
                BoardObjects[PlayerYPos + y][PlayerXPos + x].Brackets = "{}";
            }
            else
            {
                BoardObjects[PlayerYPos + y][PlayerXPos + x].Brackets = NewDestinationObj.Brackets;
            }
            if (NewDestinationObj.ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
            {
                BoardObjects[PlayerYPos + y][PlayerXPos + x].BracketsfgColor = ConsoleColor.Green;
                if (!GameController.PressedButtons.Contains(BoardObjects[PlayerYPos + y][PlayerXPos + x].Value[0]))
                    GameController.PressedButtons.Add(BoardObjects[PlayerYPos + y][PlayerXPos + x].Value[0]);
            }
            else
            {
                BoardObjects[PlayerYPos + y][PlayerXPos + x].BracketsfgColor = NewDestinationObj.BracketsfgColor;
            }

            if (BoardObjects[PlayerYPos][PlayerXPos].ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
            {
                if (GameController.PressedButtons.Contains(BoardObjects[PlayerYPos][PlayerXPos].Value[0]))
                    GameController.PressedButtons.Remove(BoardObjects[PlayerYPos][PlayerXPos].Value[0]);
            }

            BoardObjects[PlayerYPos + y][PlayerXPos + x].BracketsbgColor = NewDestinationObj.BracketsbgColor;
            BoardObjects[PlayerYPos + y][PlayerXPos + x].bgColor = NewDestinationObj.bgColor;

            PlayerXPos += x;
            PlayerYPos += y;
        }

        public void Move2(int x, int y)
        {
            GameObject destinationObject = BoardObjects[Player2YPos + y][Player2XPos + x];
            GameObject previousObject = ObjectUnderPlayer2;

            ObjectUnderPlayer2 = new GameObject(destinationObject.ObjectType, destinationObject.xPosition, destinationObject.yPosition);

            if (GameController.ObjToKeepContent.Contains(previousObject.ObjectType))
            {
                BoardObjects[Player2YPos][Player2XPos].ObjectType = previousObject.ObjectType;
                BoardObjects[Player2YPos][Player2XPos].Set();
                if (previousObject.ObjectType.ToString() == GameController.Objects.Coin.ToString() ||
                    previousObject.ObjectType.ToString() == GameController.Objects.Key.ToString())
                {
                    BoardObjects[Player2YPos][Player2XPos].Content = " ";
                }
            }
            else
            {
                BoardObjects[Player2YPos][Player2XPos].ObjectType = GameController.Objects.Blank;
                BoardObjects[Player2YPos][Player2XPos].Set();
            }

            GameObject NewDestinationObj = new GameObject(destinationObject.ObjectType, destinationObject.xPosition, destinationObject.yPosition, destinationObject.Value);
            NewDestinationObj.Set();

            BoardObjects[Player2YPos + y][Player2XPos + x].ObjectType = GameController.Objects.Player2;
            BoardObjects[Player2YPos + y][Player2XPos + x].Set();

            BoardObjects[Player2YPos + y][Player2XPos + x].Brackets = NewDestinationObj.Brackets;
            if (NewDestinationObj.ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
            {
                BoardObjects[Player2YPos + y][Player2XPos + x].BracketsfgColor = ConsoleColor.Green;
                if (!GameController.PressedButtons.Contains(BoardObjects[Player2YPos + y][Player2XPos + x].Value[0]))
                    GameController.PressedButtons.Add(BoardObjects[Player2YPos + y][Player2XPos + x].Value[0]);
            }
            else
            {
                BoardObjects[Player2YPos + y][Player2XPos + x].BracketsfgColor = NewDestinationObj.BracketsfgColor;
            }


            if (BoardObjects[Player2YPos][Player2XPos].ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
            {
                if (GameController.PressedButtons.Contains(BoardObjects[Player2YPos][Player2XPos].Value[0]))
                    GameController.PressedButtons.Remove(BoardObjects[Player2YPos][Player2XPos].Value[0]);
            }

            BoardObjects[Player2YPos + y][Player2XPos + x].BracketsbgColor = NewDestinationObj.BracketsbgColor;
            BoardObjects[Player2YPos + y][Player2XPos + x].bgColor = NewDestinationObj.bgColor;

            Player2XPos += x;
            Player2YPos += y;
        }

        public void RunEnemies()
        {
            for (int i = 0; i < BoardEnemies.Count; i++)
            {
                BoardEnemies.Values.ElementAt(i).Run();
            }
        }

        public void KillPlayer(int player)
        {
            string deathText = MenuController.Language == 0 ? "Eliminado!" : "Eliminated";
            Console.SetCursorPosition(0, HeaderRows + FooterRows + 2);
            Board.Build(this);

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;

            Console.SetCursorPosition(((Console.BufferWidth - deathText.Length) / 2) - 1, (Console.BufferHeight / 2) - 1);
            Console.WriteLine(new string(' ', deathText.Length + 2));

            Console.SetCursorPosition(((Console.BufferWidth - deathText.Length) / 2) - 1, Console.BufferHeight / 2);
            Console.Write(" ");

            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(deathText);

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(" ");

            Console.SetCursorPosition(((Console.BufferWidth - deathText.Length) / 2) - 1, (Console.BufferHeight / 2) + 1);
            Console.WriteLine(new string(' ', deathText.Length + 2));

            Console.ResetColor();
            Thread.Sleep(800);
            GameController.RestartLevel(ActiveLevel);
        }
    }
}
