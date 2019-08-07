using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;

namespace ConsoleApp21
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var p = new TimedTypeConsole();
            p.ComtentsImpl += ResourceHelper.GetRandomText;
            p.Begin();

            Console.ForegroundColor = ConsoleColor.Green;

            while (true)
            {
                var c = Console.ReadKey(true);
                if (c.Key == ConsoleKey.Enter)
                {
                    p.Reset();
                }
                else
                {
                    p.Next();
                }
            }
        }
    }

    // タイピングに応じてコンソールに文字を順次表示するクラス
    public class TimedTypeConsole
    {
        private int i = 0;
        private string text = "";

        private Timer timer;
        private TimeSpan span;

        DateTime last;
        Random rand = new Random();

        public void SetDisplayText(string msg)
        {
            this.i = 0;
        }

        public void Begin()
        {
            this.text = this.ComtentsImpl();
            this.i = 0;

            using (this.timer) { }

            this.timer = new Timer(50);
            this.timer.Elapsed += this.timer_Elapsed;
            this.timer.Start();
        }

        public void Reset()
        {
            Console.Clear();
        }

        public void Next()
        {
            this.last = DateTime.Now;
            this.span = TimeSpan.FromMilliseconds(this.rand.Next(80, 150));
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now > this.last + this.span)
            {
                //Console.WriteLine("SKIP");
                return;
            }

            if (this.i >= this.text.Length)
            {
                this.Reset();
                this.Begin();
                return; // 末尾を超えていないか？
            }

            //if (this.text[this.i] == '\n')
            //{
            //    this.i++;
            //    Console.WriteLine();
            //    return;
            //}

            char c = this.text[this.i++];
            if (c == '\t')
            {
                Console.Write("    ");
            }
            else
            {
                Console.Write(c);
            }
                

            Console.Write(this.text[this.i++]);
        }

        public event Func<string> ComtentsImpl;
    }

    // 埋め込リソースを読み取るクラス
    public class ResourceHelper
    {
        private static Random r = new Random();

        public static string GetRandomText()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string[] res = assembly.GetManifestResourceNames();
            string name = res[r.Next(0, res.Length)];

            Trace.WriteLine($"key = {name}");

            using (var sr = new StreamReader(assembly.GetManifestResourceStream(name)))
            {
                string msg = sr.ReadToEnd();
                for (int i = 0; i < 5; i++)
                {
                    msg = msg.Substring(msg.IndexOf('\n'));
                    msg = msg.TrimStart('\n');
                }

                return msg;
            }
        }
    }
}
