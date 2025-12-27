using Console2;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Console2
{
    class str_cl
    {
        public string name;
        public string value;

        public str_cl(string name, string val)
        {
            this.name = name;
            this.value = val;
        }
    }

    class int_cl
    {
        public string name;
        public int value;

        public int_cl(string name, int val)
        {
            this.name = name;
            this.value = val;
        }
    }

    class list_cl
    {
        public string name;
        public string value; // плюсы stringово хранить список: любой тип данных хранить будет; минус: больше весить будет
                             // формат хранения:
                             // [type val, type val, type val, type val], например: [int 10, int 5, str слово]
                             // списки в списках:
                             // [list [int 5, str букашка, list [str картошка, str еще\_что-то]], list [str автобус, int 5], str камаз]
        static public char[] chars = { ' ', '[', ']', ',' };
        static public char[] chars2 = { ' ', '\0', ',' };
        List<string> list;



        public list_cl(string name, string val)
        {
            this.name = name;
            this.value = Rep_in(val);
            list = Parser();

        }

        private List<string> Parser()
        {
            List<string> A2 = new List<string>();
            bool flag = false; string new_add = "";
            int level = 0;

            foreach (char s2 in value)
            {
                if (s2 == '[') level++;

                if (chars.Contains(s2) && level == 1) // 1 т. к. мы уже в []
                {
                    flag = !flag; if (new_add != "") A2.Add(new_add); new_add = ""; if (!chars2.Contains(s2)) { A2.Add(s2.ToString()); }
                    ;
                }
                else
                {
                    new_add += s2;
                }

                if (s2 == ']') level--;

            }

            if (new_add != "") A2.Add(new_add);

            return A2;

        }
        private string Rep_in(string x)
        {
            return x.Replace("{", "\\[").Replace("}", "\\]")
                .Replace("\\[", "\\\\(").Replace("\\]", "\\\\)")
                ;

        }
        private string Rep_out(string x)
        {
            return x
                .Replace("\\(", "[").Replace("\\)", "]")
                .Replace("\\[", "{").Replace("\\]", "}")
                .Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .Replace("\\_", " ")
                .Replace("\\;", ":")
                .Replace("\\.", ",")
                .Replace("\\0", "")
                .Replace("\\`", "\\")
                ;

        }

        public string find_i(int i)
        {
            string type = "", val = "";
            //Console.WriteLine($"'{this.value}'");
            //foreach (string s in list) { Console.WriteLine("'" + s + "'" + "\n"); } // тесты закончены, парсер работает четко
            int n = -1;
            foreach (string s in list)
            {
                n++; // самый первый символ - [, нет смысла рассматривать

                if ((n + 1) / 2 == i + 1)
                { //значится нашли
                    if ((n - 1) % 2 == 0)
                    {
                        // это тип данных
                        type = s;
                    }
                    else if ((n - 1) % 2 == 1)
                    {
                        // это значение
                        val = Rep_out(s);
                    }
                }
            }

            return type + " " + val;
        }
    }
}

namespace Consolesoft
{
    internal class Program
    {
        static public string reset               = "\u001b[0m";
        static public string white               = $"\u001b[38;2;255;255;255m";
        static public string black               = $"\u001b[38;2;0;0;0m";
        static public string blue                = $"\u001b[38;2;0;0;255m";
        static public string green               = $"\u001b[38;2;0;255;0m";
        static public string red                 = $"\u001b[38;2;255;100;50m";

        static public string        select_color = "\u001b[0m";

        static public List <string> history      = new List <string> ();

        static public List<str_cl>  Var_str      = new List<str_cl>();
        static public List<int_cl>  Var_int      = new List<int_cl>();
        static public List<list_cl> Var_list     = new List<list_cl>();

        static public char[] chars               = { '{', '}', ' ', '[', ']' };
        static public char[] chars2              = { '{', '}' };

        static bool          flag_exit           = false;

        static List<string> Parse(string a)
        {
            List<string> A2 = new List<string>();
            bool flag = false; string new_add = "";
            foreach (char s2 in a)
            {

                if (chars.Contains(s2))
                {
                    flag = !flag; A2.Add(new_add); new_add = ""; A2.Add(s2.ToString());
                }
                else
                {
                    new_add += s2;
                }
            }

            A2.Add(new_add);

            return A2;
        }

        static List<string> Input() {
            Console.Write(""); 
            bool   is_many = false;
            string a       = Console.ReadLine();
            
            if (a.Contains("\\*"))
            {
                is_many = true;
            }

            while (is_many)
            {
                Console.Write(">\r");
                string _a = Console.ReadLine();
                a += "\\n" + _a;
                if (_a.Contains("\\*"))
                {
                    is_many = false;
                }
            }

            var A = new List<string>(Parse(a));
            return A;
        }

        static string find(string a)
        {
            string A = "";

            for (int i = 0; i < Var_str.Count; i++)
            {
                if (Var_str[i].name == a)
                {
                    A = Var_str[i].value; break;
                }
            }

            for (int i = 0; i < Var_int.Count; i++)
            {
                if (Var_int[i].name == a)
                {
                    A = Var_int[i].value.ToString(); break;
                }
            }

            for (int i = 0; i < Var_list.Count; i++)
            {
                if (Var_list[i].name == a)
                {
                    A = Var_list[i].value; break;
                }
            }

            return A;
        }
        static bool inA(List<string> A, string n)
        {
            bool flag = false;

            foreach (var i in Var_str)
            {
                if (i.name == n)
                {
                    flag = true;
                }
            }
            foreach (var i in Var_int)
            {
                if (i.name == n)
                {
                    flag = true;
                }
            }
            foreach (var i in Var_list)
            {
                if (i.name == n)
                {
                    flag = true;
                }
            }

            return flag;
        }

        static string inO(List<string> A, string n)
        {
            string flag = "none";

            foreach (var i in Var_str)
            {
                if (i.name == n)
                {
                    flag = "str";
                }
            }
            foreach (var i in Var_int)
            {
                if (i.name == n)
                {
                    flag = "int";
                }
            }
            foreach (var i in Var_list)
            {
                if (i.name == n)
                {
                    flag = "list";
                }
            }

            return flag;
        }

        static int findA(List<string> A, string n)
        {
            int num = 0;

            int n2 = 0;
            foreach (var i in Var_str)
            {
                if (i.name == n)
                {
                    num = n2;
                }

                n2++;
            }
            n2 = 0;
            foreach (var i in Var_int)
            {
                if (i.name == n)
                {
                    num = n2;
                }

                n2++;
            }
            n2 = 0;
            foreach (var i in Var_list)
            {
                if (i.name == n)
                {
                    num = n2;
                }

                n2++;
            }

            return num;
        }


        static string Collect(List<string> A, int j) {
            string a = "";
            bool flag = false;
            for (int i = j; i < A.Count(); i++)
            {
                if (A[i] == "{")
                {
                    flag = true;
                }
                if (!flag)
                {
                    a += A[i];
                }
                else
                {
                   a += find(A[i]);
                }
                if (A[i] == "}")
                {
                    flag = false;
                }
            }
            return a;
        }

        static string Rep_out(string val) {
            string new_val = val
                .Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .Replace("\\(", "[")
                .Replace("\\)", "]")
                .Replace("\\[", "{")
                .Replace("\\]", "}")
                .Replace("\\0", "")
                .Replace("\\*", "")
                .Replace("\\`", "\\")
                ;
            return new_val;
        }

        static string Rep_outf(string val)
        {
            string new_val = val
                .Replace("\\*", "")
                ;
            return new_val;
        }

        static string Rep_color(string x)
        {
            if (string.IsNullOrEmpty(x)) return x;

            // Сначала reset
            string result = x.Replace("\\u0m", "\u001b[0m");

            // Потом RGB цвета
            result = Regex.Replace(result, @"\\u(\d{1,3})\.(\d{1,3})\.(\d{1,3})m",
                match => {
                    int r = int.Parse(match.Groups[1].Value);
                    int g = int.Parse(match.Groups[2].Value);
                    int b = int.Parse(match.Groups[3].Value);

                    // Проверяем диапазон
                    if (r > 255 || g > 255 || b > 255)
                        return match.Value; // Оставляем как есть если невалидный

                    return $"\u001b[38;2;{r};{g};{b}m";
                });

            return result;
        }

        static void exit_fun() {
            flag_exit = true;
        }

        static void history_fun() {
            Console.WriteLine("Ваша история команд:");
            foreach (string command in history) {
                Console.WriteLine("\t" + command);
            }
        }

        static void print(List<string> list) {
            string collected_value = Collect(list, 2);
            Console.WriteLine(Rep_color(Rep_out(collected_value)));
        }

        static void printf(List<string> list){
            string collected_value = Collect(list, 2);
            Console.WriteLine(Rep_outf(collected_value));
        }

        static void echo(List<string> list){
            string collected_value = Collect(list, 2);
            Console.Write(Rep_color(Rep_out(collected_value)));
        }

        static void Out(List<string> A) {
            /*Вывод по командам*/
            bool flag = true;
            switch (A[0])
            {
                case "exit":
                    exit_fun();
                    break;

                case "history":
                    history_fun();
                    break;

                case "print": // вывод
                    print(A);
                    break;

                case "printf": // вывод без экранизации (только без многострочного режима)
                    printf(A);
                    break;

                case "echo": // вывод без экранизации (только без многострочного режима)
                    echo(A);
                    break;

                default:
                    flag = false;
                    break;
            }
            if (flag) {
                history.Add(A[0]); // сохраняем только именование комманды            
            }
            
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Console\r\n(c) Корпорация Consolesoft (Consolesoft Corporation). Не все права защищены.");
            List<string>? A          = new List<string>();
            int           n          = 0;
            bool          is_command = false;

            while (!flag_exit)
            {
                A = Input();
                try{
                    Out(A);
                }
                catch (Exception e) {
                    // чтоб при ошибке программа не вылетала а продолжала работать
                    Console.WriteLine(red + "Вызвана ошибка" + select_color);    
                }
            }
        }
    }
}
