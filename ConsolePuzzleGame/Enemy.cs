using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ConsolePuzzleGame
{
    public class Enemy
    {
        public int xPos, yPos;
        public string Name = "", Direction = "down";
        public int Health = -1, MovementRadius = 3, SightRadius = 2;
        public double Speed = 0.1, Delay = 0, StartDelay = 0, BulletSpeed = 1, rateOfFire = 0.5;
        protected Game GameLvl;

        public TimeSpan? delayToMove = null;

        public static List<Enum> EnemiesAllowedDestinations = new List<Enum>
                {GameController.Objects.Blank, GameController.Objects.Player, GameController.Objects.Player2, GameController.Objects.StandingButton,
                 GameController.Objects.CoinDoor, GameController.Objects.KeyDoor, GameController.Objects.OneWay};

        public Enemy(string name, int x, int y, Game gameLvl)
        {
            Name = name;
            xPos = x;
            yPos = y;
            GameLvl = gameLvl;
        }

        public virtual void Run() { }

        public void Set(int health = -1, double speed = 0.1, double delay = 0.03, double bulletSpeed = 1, int movementRadius = 3, int sightRadius = 4, string direction = "down", double rateOfFire = 0.5, double startDelay = 0)
        {
            Health = health;
            Speed = speed;
            Delay = delay;
            BulletSpeed = bulletSpeed;
            MovementRadius = movementRadius;
            SightRadius = sightRadius;
            Direction = direction;
            this.rateOfFire = rateOfFire;
            StartDelay = startDelay;

            Game.BoardObjects[yPos][xPos].Set();
        }

        public bool Move(Enemy enemy, int xIncrement, int yIncrement)
        {
            int x = xIncrement, y = yIncrement;
            bool canMove = true, inBounds = true;

            if ((enemy.xPos == 0 && x < 0) || (enemy.xPos == Game.BoardObjects[0].Count - 1 && x > 0)) { canMove = false; inBounds = false; }
            if ((enemy.yPos == 0 && y < 0) || (enemy.yPos == Game.BoardObjects.Count - 1 && y > 0)) { canMove = false; inBounds = false; }

            if (inBounds)
            {
                if (!EnemiesAllowedDestinations.Contains(Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].ObjectType)) { canMove = false; }

                if (Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString()
                    && GameController.CollectedCoins < int.Parse(Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Value)) { canMove = false; }

                if (Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].ObjectType.ToString() == GameController.Objects.KeyDoor.ToString()
                    && (!GameController.CollectedKeys.Contains(Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Value[0])
                    && !GameController.PressedButtons.Contains(Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Value[0]))) { canMove = false; }

                if (Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].ObjectType.ToString() == GameController.Objects.OneWay.ToString())
                {
                    if ((Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Value == "up" && y < 0) || (Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Value == "down" && y > 0)
                    || (Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Value == "left" && x < 0) || (Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Value == "right" && x > 0))
                    { x *= 2; y *= 2; }
                    else { canMove = false; }

                    if (y * 2 < 0 || y * 2 > Game.BoardObjects.Count - 1 || x * 2 < 0 || x * 2 > Game.BoardObjects[0].Count)
                    {
                        canMove = false;
                    }
                }
            }


            if (canMove)
            {

                GameObject Destination = Game.BoardObjects[enemy.yPos + y][enemy.xPos + x];
                GameObject NewDestinationObj = new GameObject(Destination.ObjectType, Destination.xPosition, Destination.yPosition, Destination.Value);
                NewDestinationObj.Set();

                //moves destination to background
                Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].ObjectTypeUnderThis = NewDestinationObj;

                //sets enemy to destination
                Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].ObjectType = Game.BoardObjects[enemy.yPos][enemy.xPos].ObjectType;
                Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Set();
                if (Game.BoardObjects[enemy.yPos][enemy.xPos].ObjectType.ToString() != GameController.Objects.Enemy_Seeker.ToString())
                {
                    Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].Brackets = NewDestinationObj.Brackets;
                    Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].BracketsbgColor = NewDestinationObj.BracketsbgColor;

                    if (Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].ObjectType.ToString() != GameController.Objects.StandingButton.ToString())
                        Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].BracketsfgColor = NewDestinationObj.BracketsfgColor;
                    Game.BoardObjects[enemy.yPos + y][enemy.xPos + x].bgColor = NewDestinationObj.bgColor;
                }

                //sets enemy old position to old object
                Game.BoardObjects[enemy.yPos][enemy.xPos].ObjectType = Game.BoardObjects[enemy.yPos][enemy.xPos].ObjectTypeUnderThis.ObjectType;
                Game.BoardObjects[enemy.yPos][enemy.xPos].Set();


                enemy.xPos += x;
                enemy.yPos += y;

                if (Game.BoardObjects[enemy.yPos][enemy.xPos].ObjectTypeUnderThis.ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
                {
                    if (!GameController.PressedButtons.Contains(Game.BoardObjects[enemy.yPos][enemy.xPos].Value[0]))
                        GameController.PressedButtons.Add(Game.BoardObjects[enemy.yPos][enemy.xPos].Value[0]);

                    Game.BoardObjects[enemy.yPos][enemy.xPos].BracketsfgColor = ConsoleColor.Green;
                }

                if (enemy.yPos + (-y) >= 0 && enemy.yPos + (-y) <= Game.BoardObjects.Count - 1 && enemy.xPos + (-x) >= 0 && enemy.xPos + (-x) <= Game.BoardObjects[0].Count - 1)
                {
                    if (Game.BoardObjects[enemy.yPos + (-y)][enemy.xPos + (-x)].ObjectType.ToString() == GameController.Objects.StandingButton.ToString())
                    {
                        Game.BoardObjects[enemy.yPos + (-y)][enemy.xPos + (-x)].Set();

                        if (GameController.PressedButtons.Contains(Game.BoardObjects[enemy.yPos + (-y)][enemy.xPos + (-x)].Value[0]))
                            GameController.PressedButtons.Remove(Game.BoardObjects[enemy.yPos + (-y)][enemy.xPos + (-x)].Value[0]);
                    }
                }
            }
            return (canMove);
        }
    }

    public class Guard : Enemy
    {
        int x = 0, y = 0;
        public Guard(string name, int x, int y, Game gameLvl) : base(name, x, y, gameLvl)
        {
            //Name: The name of this Enemy in Game.BoardEnemies Dictionary
            //Health: The health of this Enemy (Needed damage to die)
            //Speed: The movement speed of this Enemy
            //Delay: The time waited to change movement direction after hitting a wall
            //Direction: The direction this enemy will start moving to
            //StartDelay: The time waited before start moving
        }

        bool waitingDelay = false;
        bool firstMove = true;
        Stopwatch sw = new Stopwatch();

        public override void Run()
        {
            if (delayToMove == null)
            {
                sw.Start();
                if (firstMove)
                {
                    switch (Direction)
                    {
                        case "up":
                            y = -1;
                            break;
                        case "right":
                            x = 1;
                            break;
                        case "down":
                            y = 1;
                            break;
                        case "left":
                            x = -1;
                            break;
                    }
                    delayToMove = DateTime.Now.AddMilliseconds(StartDelay) - DateTime.Now;
                    firstMove = false;
                }
                else
                {
                    delayToMove = DateTime.Now.AddMilliseconds(100 / Speed) - DateTime.Now;
                }
            }

            if (sw.Elapsed >= TimeSpan.FromMilliseconds(Delay))
                waitingDelay = false;

            if (sw.Elapsed >= delayToMove && !waitingDelay)
            {
                bool canMove;

                canMove = Move(this, x, y);

                if (!canMove)
                {
                    waitingDelay = true;

                    x = x < 0 ? x * x : x * -x;
                    y = y < 0 ? y * y : y * -y;
                }

                sw.Stop();
                sw.Reset();
                delayToMove = null;
            }
            if (xPos == GameLvl.PlayerXPos && yPos == GameLvl.PlayerYPos)
            {
                GameLvl.KillPlayer(1);
            }
            if (xPos == GameLvl.Player2XPos && yPos == GameLvl.Player2YPos)
            {
                if (GameLvl.Multiplayer == true)
                {
                    GameLvl.KillPlayer(2);
                }
            }
        }
    }

    public class Cannon : Enemy
    {
        int bulletsCounter = 0;
        bool firstShot = true;
        Enemy bullet;
        public Cannon(string name, int x, int y, Game gameLvl) : base(name, x, y, gameLvl)
        {
            //Name: The name of this Enemy in Game.BoardEnemies Dictionary
            //Health: The health of this Enemy (Needed damage to die)
            //Direction: The direction this enemy will start moving to
            //bulletSpeed: The movement speed of the bullets shot by this Cannon
            //rateOfFire: The fire rate of this cannon (the greater the number, the shortest the time waited before firing again)
            //StartDelay: The time waited before start shooting
        }

        Stopwatch sw = new Stopwatch();

        public override void Run()
        {
            if (delayToMove == null)
            {
                sw.Start();
                if (firstShot)
                {
                    delayToMove = DateTime.Now.AddMilliseconds(StartDelay) - DateTime.Now;
                }
                else
                {
                    delayToMove = DateTime.Now.AddMilliseconds(1000 / rateOfFire) - DateTime.Now;
                }
            }

            if (sw.Elapsed >= delayToMove || (firstShot && StartDelay == 0))
            {
                bullet = new CannonBullet(bulletsCounter, xPos, yPos, Direction, GameLvl, Name, BulletSpeed);
                bulletsCounter++;

                sw.Stop();
                sw.Reset();
                delayToMove = null;
                firstShot = false;
            }
        }
    }

    public class CannonBullet : Enemy
    {
        string CannonName;
        public CannonBullet(int bulletID, int x, int y, string dir, Game gameLvl, string cannonName, double speed) : base(cannonName + "_Bullet_" + bulletID, x, y, gameLvl)
        {
            //cannonName: The name of the cannon that shot this bullet
            //Name: cannonName + bulletID
            //bulletID: The ID of this bullet according to the bulletsCounter of the cannon
            //dir: The direction the cannon is pointing to, wich is the direction the bullet will move to
            //bulletSpeed: The movement speed of this bullet, set by the cannon

            switch (dir)
            {
                case "up":
                    yPos--;
                    break;
                case "right":
                    xPos++;
                    break;
                case "down":
                    yPos++;
                    break;
                case "left":
                    xPos--;
                    break;
                default:
                    yPos++;
                    break;
            }
            if (EnemiesAllowedDestinations.Contains(Game.BoardObjects[yPos][xPos].ObjectType))
            {
                Game.BoardObjects[yPos][xPos] = new GameObject(GameController.Objects.Enemy_CannonBullet, yPos, xPos);
                Game.BoardObjects[yPos][xPos].Set();
                Game.BoardEnemies.Add(Name, this);
            }
            BulletSpeed = speed;
            CannonName = cannonName;
            Direction = dir;
        }

        Stopwatch sw = new Stopwatch();

        public override void Run()
        {
            if (delayToMove == null)
            {
                sw.Start();
                delayToMove = DateTime.Now.AddMilliseconds(100 / BulletSpeed) - DateTime.Now;
            }

            if (sw.Elapsed >= delayToMove)
            {
                bool canMove;
                switch (Direction)
                {
                    case "up":
                        canMove = Move(this, 0, -1);
                        break;
                    case "right":
                        canMove = Move(this, 1, 0);
                        break;
                    case "down":
                        canMove = Move(this, 0, 1);
                        break;
                    case "left":
                        canMove = Move(this, -1, 0);
                        break;
                    default:
                        canMove = Move(this, 0, 1);
                        break;
                }

                if (!canMove)
                {
                    Game.BoardEnemies.Remove(Name);
                    Game.BoardObjects[yPos][xPos] = Game.BoardObjects[yPos][xPos].ObjectTypeUnderThis;
                    Game.BoardObjects[yPos][xPos].Set();
                }

                sw.Stop();
                sw.Reset();
                delayToMove = null;
            }
            if (xPos == GameLvl.PlayerXPos && yPos == GameLvl.PlayerYPos)
            {
                GameLvl.KillPlayer(1);
            }
            if (xPos == GameLvl.Player2XPos && yPos == GameLvl.Player2YPos)
            {
                if (GameLvl.Multiplayer == true)
                {
                    GameLvl.KillPlayer(2);
                }
            }

        }

    }

    public class Seeker : Enemy
    {
        int x = 0, y = 0;
        public Seeker(string name, int x, int y, Game gameLvl) : base(name, x, y, gameLvl)
        {

        }

        int playerX = 0, playerY = 0;
        int spotted = 0;

        Stopwatch sw = new Stopwatch();

        public override void Run()
        {
            //eyes
            {
                if (spotted == 0)
                {
                    if ((GameLvl.PlayerXPos >= xPos - SightRadius && GameLvl.PlayerXPos <= xPos + SightRadius)
                && (GameLvl.PlayerYPos >= yPos - SightRadius && GameLvl.PlayerYPos <= yPos + SightRadius))
                    {
                        spotted = 1;
                    }
                    if (GameLvl.Player2XPos >= xPos - SightRadius && GameLvl.Player2XPos <= xPos + SightRadius
                    && GameLvl.Player2YPos >= yPos - SightRadius && GameLvl.Player2YPos <= yPos + SightRadius)
                    {
                        spotted = 2;
                    }
                }

                if (spotted == 1)
                {
                    playerX = GameLvl.PlayerXPos;
                    playerY = GameLvl.PlayerYPos;
                }
                else if (spotted == 2)
                {
                    playerX = GameLvl.Player2XPos;
                    playerY = GameLvl.Player2YPos;
                }

                //sets horizontal pupil
                if (playerX < xPos && spotted != 0)
                {
                    Game.BoardObjects[yPos][xPos].Brackets = "0";
                }
                else if (playerX > xPos && spotted != 0)
                {
                    Game.BoardObjects[yPos][xPos].Brackets = "2";
                }
                else if (playerX == xPos || spotted == 0)
                {
                    Game.BoardObjects[yPos][xPos].Brackets = "1";
                }

                //sets vertical pupil
                if (playerY < yPos && spotted != 0)
                {
                    Game.BoardObjects[yPos][xPos].Content = "\'";
                }
                else if (playerY > yPos && spotted != 0)
                {
                    Game.BoardObjects[yPos][xPos].Content = ".";
                }
                else if (playerY == yPos && spotted != 0)
                {
                    Game.BoardObjects[yPos][xPos].Content = "-";
                }
                else if (spotted == 0)
                {
                    Game.BoardObjects[yPos][xPos].Content = "?";
                }
            }

            if (spotted != 0)
            {
                if (delayToMove == null)
                {
                    sw.Start();
                }

                delayToMove = DateTime.Now.AddMilliseconds(100 / Speed) - DateTime.Now;

                if (sw.Elapsed >= delayToMove)
                {
                    bool canMove;
                    calcDir();
                    canMove = Move(this, x, y);

                    if (!canMove)
                    {

                    }

                    sw.Stop();
                    sw.Reset();
                    delayToMove = null;
                }
                if (xPos == GameLvl.PlayerXPos && yPos == GameLvl.PlayerYPos)
                {
                    GameLvl.KillPlayer(1);
                }
                if (xPos == GameLvl.Player2XPos && yPos == GameLvl.Player2YPos)
                {
                    if (GameLvl.Multiplayer == true)
                    {
                        GameLvl.KillPlayer(2);
                    }
                }
            }
        }

        void calcDir()
        {
            int distanceX;
            int distanceY;

            if (xPos > playerX)
                distanceX = xPos - playerX;
            else
                distanceX = playerX - xPos;

            if (yPos > playerY)
                distanceY = yPos - playerY;
            else
                distanceY = playerY - yPos;

            bool movehorizontal = false;

            if (distanceX < distanceY)
            {
                movehorizontal = true;

                int xInc = playerX > xPos ? 1 : -1;
                if (distanceX == 0 || !EnemiesAllowedDestinations.Contains(Game.BoardObjects[yPos][xPos + xInc].ObjectType)
                    || (Game.BoardObjects[yPos][xPos + xInc].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString()
                    && GameController.CollectedCoins < int.Parse(Game.BoardObjects[yPos][xPos + xInc].Value)))
                {
                    movehorizontal = false;
                }
            }
            else if (distanceX > distanceY)
            {
                movehorizontal = false;

                int yInc = playerY > yPos ? 1 : -1;
                if (distanceY == 0 || !EnemiesAllowedDestinations.Contains(Game.BoardObjects[yPos + yInc][xPos].ObjectType)
                    || (Game.BoardObjects[yPos + yInc][xPos].ObjectType.ToString() == GameController.Objects.CoinDoor.ToString()
                    && GameController.CollectedCoins < int.Parse(Game.BoardObjects[yPos + yInc][xPos].Value)))
                {
                    movehorizontal = true;
                }
            }
            else
            {
                x = 0;
                y = 0;
            }


            if (movehorizontal)
            {
                if (playerX > xPos)
                {
                    x = 1;
                    y = 0;
                }
                else if (playerX < xPos)
                {
                    x = -1;
                    y = 0;
                }
                else
                {
                    x = 0;
                    y = 0;
                }
            }
            else
            {
                if (playerY > yPos)
                {
                    x = 0;
                    y = 1;
                }
                else if (playerY < yPos)
                {
                    x = 0;
                    y = -1;
                }
                else
                {
                    x = 0;
                    y = 0;
                }
            }
        }
    }
}
