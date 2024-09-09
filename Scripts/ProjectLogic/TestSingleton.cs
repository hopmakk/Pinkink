using Godot;
using System;
using System.Runtime.InteropServices;

namespace PinkInk.Scripts.ProjectLogic
{
    internal class TestSingleton
    {
        // TestSingleton.FirstDatePoint = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        public static long FirstDatePoint;

        // TestSingleton.CalculateTime(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        public static void CalculateTime(long ms)
        {
            long measuredMilliseconds = ms - FirstDatePoint;
            GD.Print(measuredMilliseconds);
            GD.Print(measuredMilliseconds / 1000);
            GD.Print("-------------------");
        }


        public static void RunConsole()
        {
            AllocConsole();
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        public static void ConsoleWriteL(string text, ConsoleColor font = ConsoleColor.White, ConsoleColor back = ConsoleColor.Black)
        {
            var lastBack = Console.BackgroundColor;
            var lastFont = Console.ForegroundColor;
            Console.BackgroundColor = back;
            Console.ForegroundColor = font;
            Console.WriteLine(text);
            Console.BackgroundColor = lastBack;
            Console.ForegroundColor = lastFont;
        }
    }
}
