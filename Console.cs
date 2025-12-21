using System.Collections.Specialized;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.IO;

namespace Console2
{
    class str_cl {
        public string name;
        public string value;

        public str_cl(string name, string val) {
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


    internal class Program
    {
        static public List<str_cl> Var_str = new List<str_cl>();
        static public List<int_cl> Var_int = new List<int_cl>();
        static public char[] chars = {'{', '}', ' ', '(', ')'};
        static public char[] chars2 = { '{', '}' };

        static string string_(List <string>? h, string a) {
            string? b = "";
            for (int i = 0; i < h.Count(); i++) { 
                b += h[i] + a;
            }
            return b;
        }

        static string ColorRep(string x)
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

        static string Rep_not_space(string x)
        {
            string X;



            X = x.Replace("\\n", "\n") // \n - аналогично enter
                .Replace("\\[", "{").Replace("\\]", "}"); // для фигурных 

            X = X.Replace("\\`", "\\") // это обязательно делать в конец

            ; // по идеи этого достаточно для полноценной системы

            return X;
        }

        static string Rep(string x)
        {
            string X;



            X = x.Replace("\\n", "\n") // \n - аналогично enter
                .Replace("\\s", " ") // это важно, т. к. в моменте идет разделение пробелом
                .Replace("\\[", "{").Replace("\\]", "}"); // для фигурных 

            X = X.Replace("\\`", "\\") // это обязательно делать в конец

            ; // по идеи этого достаточно для полноценной системы

            return X;
        }

        static List<string> Parse(string a) { 
            List<string> A2 = new List<string>();
            bool flag = false; string new_add = "";
            foreach (char s2 in a) {

                if (chars.Contains(s2))
                {
                    flag = !flag; A2.Add(new_add); new_add = ""; A2.Add(s2.ToString());
                }
                else {
                    new_add += s2;
                }
            }

            A2.Add(new_add);

            return A2;
        }

        static string find(string a) {
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

            return A;
        }
        static bool inA(List<string> A, string n) { 
            bool flag = false;

            foreach (var i in Var_str) {
                if (i.name == n) {
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

            return num;
        }

        static string sob2(List<string> A, int j)
        {
            string a = "";
            bool flag = false;
            for (int i = j; i < A.Count(); i++)
            {

                if (i != A.Count() - 1)
                    a += A[i] + " ";
                else
                    a += A[i];

            }
            return a;
        }

        static string sob(List<string> A, int j)
        {
            string a = "";
            bool flag = false;
            for (int i = j; i < A.Count(); i++)
            {
                if (A[i] == "{") {
                    flag = true;
                }
                if (!flag)
                {
                    a += A[i];
                }
                else {
                    a += find(A[i]);
                }
                if (A[i] == "}")
                {
                    flag = false;
                }
            }
            return a;
        }

        static string sob_not_space(List<string> A, int j)
        {
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
                    a += find(A[i]).Replace(" ", "\\s");
                }
                if (A[i] == "}")
                {
                    flag = false;
                }
            }
            return a;
        }

        static string sob(List<string> A, int j, int k)
        {
            string a = "";
            bool flag = false;
            for (int i = j; i <= k; i++)
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

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string reset = "\u001b[0m";
            string white = $"\u001b[38;2;255;255;255m";
            string black = $"\u001b[38;2;0;0;0m";
            string blue = $"\u001b[38;2;0;0;255m";
            string green = $"\u001b[38;2;0;255;0m";
            string red = $"\u001b[38;2;255;100;50m";

            string select_color = reset;


            Console.WriteLine("Console\r\n(c) Корпорация Consolesoft (Consolesoft Corporation). Не все права защищены.");
            string a, t; List <string>? A = new List <string> (), History = new List<string>();
            int n = 0;
            bool is_command=false;

            while (true)
            {
                Console.Write("");
                is_command = false;
                a = Console.ReadLine();
                A = new List<string>(Parse(a));

                if (a == "exit")
                {
                    is_command = true;
                    Console.WriteLine(">Выход из программы");
                    break;
                }

                if (a == "history")
                {
                    is_command = true;
                    Console.WriteLine(">Ваша история команд:\n" + string_(History, "\n"));
                }

                // япшное

                if (A[0] == "print")
                {
                    is_command = true;
                    Console.WriteLine(Rep(ColorRep(sob(A, 2))));
                }

                if (A[0] == "set") // set(0) (1) int(2) (3) name(4)  (5) val(6)
                {
                    is_command = true;

                    if (A[2] == "str")
                    {
                        string val = Rep(sob(A, 6));
                        if (!inA(A, A[4]))
                            Var_str.Add(new str_cl(A[4], val));
                        else
                        {
                            n = findA(A, A[4]);
                            Var_str[n] = new str_cl(A[4], val);
                        }
                    }
                    if (A[2] == "int")
                    {
                        int val = int.Parse(Rep(sob(A, 6)));
                        if (!inA(A, A[4]))
                            Var_int.Add(new int_cl(A[4], val));
                        else
                        {
                            n = findA(A, A[4]);
                            Var_int[n] = new int_cl(A[4], val);
                        }
                    }
                }

                if (true)
                {  // нежелательно, но можно без 
                    if (A[0] == "str")
                    {
                        is_command = true;

                        string val = Rep(sob(A, 4));
                        if (!inA(A, A[2]))
                            Var_str.Add(new str_cl(A[2], val));
                        else
                        {
                            n = findA(A, A[2]);
                            Var_str[n] = new str_cl(A[2], val);
                        }
                    }
                    if (A[0] == "int")
                    {
                        is_command = true;

                        int val = int.Parse(Rep(sob(A, 4)));
                        if (!inA(A, A[2]))
                            Var_int.Add(new int_cl(A[2], val));
                        else
                        {
                            n = findA(A, A[2]);
                            Var_int[n] = new int_cl(A[2], val);
                        }
                    }
                }

                if (A[0] == "get")
                {
                    is_command = true;

                    Console.Write(green + ">\r"); // чтоб было визуальное сопровождение
                    string val = Console.ReadLine();
                    if (!inA(A, A[2]))
                        Var_str.Add(new str_cl(A[2], val));
                    else
                    {
                        n = findA(A, A[2]);
                        Var_str[n] = new str_cl(A[2], val);
                    }
                    Console.Write(select_color);
                }

                if (!inA(A, A[0]))
                {    // конвертатор типов данных
                    n = findA(A, A[0]);
                    t = inO(A, A[0]);
                    if (t == "int")
                    {
                        int value = Var_int[n].value;
                        if (A[2] == "to")
                        {
                            is_command = true;

                            if (A[4] == "str")
                            {
                                Var_int.RemoveAt(n);
                                Var_str.Add(new str_cl(A[0], value.ToString()));
                            }
                        }
                    }

                    if (t == "str")
                    {
                        string value = Var_str[n].value;
                        if (A[2] == "to")
                        {
                            is_command = true;

                            if (A[4] == "int")
                            {
                                Var_int.RemoveAt(n);
                                Var_int.Add(new int_cl(A[0], Convert.ToInt32(value)));
                            }
                        }
                    }


                }

                if (A[0] == "add") {
                    is_command = true;
                    string reg = Rep(sob(A, 4));

                    n = findA(A, A[2]);
                    t = inO(A, A[2]);

                    if (t == "int") {
                        Var_int[n].value += int.Parse(reg);
                    }
                    if (t == "str"){
                        Var_str[n].value += sob(A, 4);
                    }
                }

                if (A[0] == "sub")
                {
                    is_command = true;
                    string reg = Rep(sob(A, 4));


                    n = findA(A, A[2]);
                    t = inO(A, A[2]);

                    if (t == "int")
                    { // это только для чисел
                        Var_int[n].value -= int.Parse(reg);
                    }
                }

                if (A[0] == "multy")
                {
                    is_command = true;
                    string reg = Rep(sob(A, 4));


                    n = findA(A, A[2]);
                    t = inO(A, A[2]);

                    if (t == "int")
                    { 
                        Var_int[n].value *= int.Parse(reg);
                    }

                    if (t == "str")
                    { 
                        string val = Var_str[n].value;
                        for (int i = 0; i < int.Parse(reg); i++) Var_str[n].value += val;
                    }
                }

                if (A[0] == "div")
                {
                    is_command = true;
                    string reg = Rep(sob(A, 4));


                    n = findA(A, A[2]);
                    t = inO(A, A[2]);

                    if (t == "int")
                    {
                        Var_int[n].value /= int.Parse(reg); // внимание оно округлит
                    }
                }


                    if (A[0] == "color")
                {
                    is_command = true;


                    if (A[2] == "reset")
                    {
                        Console.Write(reset);
                        select_color = reset;
                    } // еще цветов пописать
                    else if (A[2] == "white")
                    {
                        Console.Write(white);
                        select_color = white;
                    }
                    else if (A[2] == "black")
                    {
                        Console.Write(black);
                        select_color = black;
                    }
                    else if (A[2] == "blue")
                    {
                        Console.Write(blue);
                        select_color = blue;
                    }
                    else if (A[2] == "green")
                    {
                        Console.Write(green);
                        select_color = green;
                    }
                    else if (A[2] == "red")
                    {
                        Console.Write(red);
                        select_color = red;
                    }
                    else
                    {

                        if (A[2] == "rgb")
                        {
                            List<string> parametrs = new List<string>(Rep(sob(A, 4)).Split(" "));

                            string rgb = $"\u001b[38;2;{parametrs[0]};{parametrs[1]};{parametrs[2]}m";
                            Console.Write(rgb);
                            select_color = rgb;
                        }
                        else if (A[2] == "hex")
                        { // сделаю позже
                            string hex = sob(A, 4);
                            string rgb = $"\u001b[38;2;{sob(A, 4, 4)};{sob(A, 6, 6)};{sob(A, 8, 8)}m";
                            Console.Write(rgb);
                            select_color = rgb;
                        }

                    }
                }

                // файлы

                if (A[0] == "write")
                {
                    List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                    var reg = new List<string>();
                    foreach (string i in reg2)
                    {
                        reg.Add(Rep(i));
                    }

                    string filePath = reg[0];
                    string text = sob2(reg, 1);

                    File.WriteAllTextAsync(filePath, text, Encoding.UTF8);


                    is_command = true;


                }

                if (A[0] == "del")
                {
                    List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                    var reg = new List<string>();
                    foreach (string i in reg2)
                    {
                        reg.Add(Rep(i));
                    }

                    string filePath = reg[0];

                    if (File.Exists(filePath))
                    { File.Delete(filePath); }


                    is_command = true;
                }

                if (A[0] == "del_rf") // удалять рекурсивно
                {
                    List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                    var reg = new List<string>();
                    foreach (string i in reg2)
                    {
                        reg.Add(Rep(i));
                    }

                    string folderPath = reg[0];

                    Directory.Delete(folderPath, recursive: true);


                    is_command = true;
                }

                if (A[0] == "read")
                {
                    List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                    var reg = new List<string>();
                    foreach (string i in reg2)
                    {
                        reg.Add(Rep(i));
                    }

                    string filePath = reg[0];


                    is_command = true;

                    string allContent = File.ReadAllText(filePath, Encoding.UTF8);


                    Console.WriteLine(allContent);


                }

                if (A[0] == "create_nwv")// nwv - not with var, типо не с переменными и без интерполяцией (что невсегда удобно)
                {
                    string filePath = A[2];

                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) { writer.WriteLine(""); }; // по умолчанию файл пустой

                    is_command = true;


                }

                if (A[0] == "create")
                {
                    List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                    var reg = new List<string>(); 
                    foreach (string i in reg2) { 
                        reg.Add(Rep(i));
                    }

                    string filePath = reg[0];

                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) { writer.WriteLine(""); }
                    ; // по умолчанию файл пустой

                    is_command = true;

                }

                if (A[0] == "create_rep") // создать папку 
                {
                    List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                    var reg = new List<string>();
                    foreach (string i in reg2)
                    {
                        reg.Add(Rep(i));
                    }

                    string folderPath = reg[0];

                    DirectoryInfo directoryInfo = Directory.CreateDirectory(folderPath);

                    is_command = true;

                }


                if (a == "clear")
                {
                    is_command = true;
                    Console.Clear();
                }

                if (is_command)
                {
                    History.Add(a);
                }

            }


        }
    }
}

