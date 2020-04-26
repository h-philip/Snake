using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Simple Snake Game by Philip Hönnecke\n\n" +
                "The game will run in the current terminal window.\n" +
                "If you want to change the game's size, simply change the terminal's size. (Before pressing <Enter>!)\n" +
                "To start the game, press <Enter> and then any arrow key.\n\n" +
                "If you only got this exe-file and want to see the code, go to \"https://github.com/h-philip/Snake\".\n\n" +
                "Have fun! :)");

            Console.ReadLine();
            Console.Clear();
            new Snake(Console.WindowWidth / 2 - 1, Console.WindowHeight - 1);
            //new Snake(10, 10);
        }
    }

    enum Direction
    {
        Up = 0, Right = 1, Down = 2, Left = 3, None
    }

    class Part
    {
        public bool isHead = false;
        public int x, y;
        public Part next, prev;
        public Part(Part next, Part prev, int x, int y, bool isHead)
        {
            this.next = next;
            this.prev = prev;
            this.x = x;
            this.y = y;
            this.isHead = isHead;
        }
    }

    class Snake
    {
        Random rand;
        Direction dir = Direction.None;
        int headX, headY, cookieX, cookieY, tailX, tailY;
        char headChar = 'o', tailChar = '#', cookieChar = 'x';
        int width, height;
        Part head, end;
        bool bot = false;

        public Snake(int width, int height)
        {
            this.width = width;
            this.height = height;
            Console.CursorVisible = false;

            rand = new Random();
            headX = rand.Next(1, width - 1);
            headY = rand.Next(1, height - 1);
            head = new Part(null, null, headX, headY, true);

            cookieX = rand.Next(width);
            cookieY = rand.Next(height);
            if (cookieX == headX)
                if (cookieX < width / 2) cookieX++;
                else cookieX--;
            if (cookieY == headY)
                if (cookieY < height / 2) cookieY++;
                else cookieY--;

            while (dir == Direction.None)
                dir = KeyToDir(Console.ReadKey(true));
            tailX = headX;
            tailY = headY;
            if (dir == Direction.Up)
                tailY++;
            else if (dir == Direction.Right)
                tailX--;
            else if (dir == Direction.Down)
                tailY--;
            else if (dir == Direction.Left)
                tailX++;
            end = new Part(null, head, tailX, tailY, false);
            head.next = end;

            WriteAt(headX, headY, headChar);
            WriteAt(cookieX, cookieY, cookieChar);
            WriteAt(tailX, tailY, tailChar);
            Thread.Sleep(1000);
            Console.CursorVisible = false;
            Run();
        }

        void WriteAt(int column, int row, char c)
        {
            Console.CursorLeft = column * 2;
            Console.CursorTop = row;
            Console.Write(c);
        }

        void Run()
        {
            while (true)
            {
                if (bot)
                {
                    if (cookieX > head.x)
                        dir = Direction.Right;
                    else if (cookieX < head.x)
                        dir = Direction.Left;
                    else if (cookieY > head.y)
                        dir = Direction.Down;
                    else if (cookieY < head.y)
                        dir = Direction.Up;
                }
                if (Console.KeyAvailable)
                {
                    Direction prevDir = dir;
                    dir = KeyToDir(Console.ReadKey(true));
                    if (((int)dir % 2 == 1 && (int)prevDir % 2 == 1) || ((int)dir % 2 == 0 && (int)prevDir % 2 == 0))
                        dir = prevDir;
                }
                if (!Move(dir))
                    break;

                Thread.Sleep(100);
            }
            WriteAt(width / 2 - 3, height / 2, ' ');
            Console.WriteLine("Game Over!");
            Console.ReadKey();
        }

        Direction KeyToDir(ConsoleKeyInfo key)
        {
            if (key.Key.Equals(ConsoleKey.UpArrow))
                return Direction.Up;
            else if (key.Key.Equals(ConsoleKey.RightArrow))
                return Direction.Right;
            else if (key.Key.Equals(ConsoleKey.DownArrow))
                return Direction.Down;
            else if (key.Key.Equals(ConsoleKey.LeftArrow))
                return Direction.Left;
            else
                return dir;
        }

        void GenerateCookie()
        {
            List<int[]> locked = new List<int[]>();
            Part p = head;
            while (p != null)
            {
                locked.Add(new int[] { p.x, p.y });
                p = p.next;
            }

            while (true)
            {
                cookieX = rand.Next(width);
                cookieY = rand.Next(height);
                bool stop = true;
                foreach (int[] l in locked)
                    if (l[0] == cookieX && l[1] == cookieY)
                        stop = false;
                if (stop)
                    break;
            }
            WriteAt(cookieX, cookieY, cookieChar);
        }

        bool Move(Direction dir)
        {
            Part p = new Part(head.next, head, headX, headY, false);
            WriteAt(headX, headY, tailChar);

            if (dir == Direction.Up)
                headY--;
            else if (dir == Direction.Right)
                headX++;
            else if (dir == Direction.Down)
                headY++;
            else if (dir == Direction.Left)
                headX--;
            else return false;

            if (headX < 0 || headY < 0 || headX > width || headY > height)
                return false;

            WriteAt(headX, headY, headChar);
            head.x = headX;
            head.y = headY;

            p.next.prev = p;
            p.prev.next = p;

            Part t = head.next;
            while (t != null)
            {
                if (head.x == t.x && head.y == t.y)
                    return false;
                t = t.next;
            }

            if (headX == cookieX && headY == cookieY)
                GenerateCookie();
            else
            {
                WriteAt(end.x, end.y, ' ');
                end = end.prev;
                end.next = null;
            }

            return true;
        }
    }
}
