using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;

namespace ConsolePuzzleGame
{
    class Program
    {
        //Configuration to maximize console window
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int MAXIMIZE = 3;

        //Configuration to prevent user from minimizing and resizing console window
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_SIZE = 0xF000;
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        //Configuration to prevent user from stopping program by clicking on the console (edit mode)
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int ioMode);
        const int QuickEditMode = 64;
        const int ExtendedFlags = 128;
        public const int STD_INPUT_HANDLE = -10;
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        public Level LevelsHolder;

        static void Main(string[] args)
        {
            try
            {
                Console.TreatControlCAsInput = true;
                SetConsoleWindowConfig();
                StartReadingInput();

                MenuController.StartMenu();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ResetColor();
                Console.BufferHeight = 500;
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                Console.ReadKey();
            }


        }

        public static void MaximizeWindow()
        {
            //Maximize console window
            ShowWindow(ThisConsole, MAXIMIZE);

            //Deletes options to minimize and resize console window
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            //Disable console edit mode
            DisableQuickEdit();
        }

        static void DisableQuickEdit()
        {
            IntPtr conHandle = GetStdHandle(STD_INPUT_HANDLE);
            int mode;

            if (!GetConsoleMode(conHandle, out mode))
            {
                // error getting the console mode. Exit.
                return;
            }

            mode = mode & ~(QuickEditMode | ExtendedFlags);

            if (!SetConsoleMode(conHandle, mode))
            {
                // error setting console mode.
            }
        }

        public static void SetConsoleWindowConfig()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            MaximizeWindow();
        }

        public static void StartReadingInput()
        {
            Task.Factory.StartNew(() =>
            {
                while (MenuController.ReadInput)
                {
                    Console.CursorVisible = false;

                    GameController.keyInfo = Console.ReadKey(true);
                    GameController.consoleKey = GameController.keyInfo.Key;
                    if ((GameController.keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                    {
                        GameController.CtrlKeyHeld = true;
                    }

                    Console.CursorVisible = true;
                }
            });
        }
    }
}
