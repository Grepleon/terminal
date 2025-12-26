using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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

    class list_cl
    {
        public string name;
        public string value; // плюсы stringово хранить список: любой тип данных хранить будет; минус: больше весить будет
                             // формат хранения:
                             // [type val, type val, type val, type val], например: [int 10, int 5, str слово]
                             // списки в списках:
                             // [list [int 5, str букашка, list [str картошка, str еще\_что-то]], list [str автобус, int 5], str камаз]
        static public char[] chars = { ' ', '[', ']', ','};
        static public char[] chars2 = {' ', '\0', ','};
        List<string> list;



        public list_cl(string name, string val)
        {
            this.name = name;
            this.value = Rep_in(val);
            list = Parser();

        }

        private List<string> Parser() {
            List<string> A2 = new List<string>();
            bool flag = false; string new_add = "";
            int level = 0;

            foreach (char s2 in value)
            {
                if (s2 == '[') level++;

                if (chars.Contains(s2) && level == 1) // 1 т. к. мы уже в []
                {
                    flag = !flag; if (new_add != "") A2.Add(new_add); new_add = ""; if (!chars2.Contains(s2)) { A2.Add(s2.ToString()); };
                }
                else
                {
                    new_add += s2;
                }

                if (s2 == ']') level--;

            }

            if(new_add != "") A2.Add(new_add);

            return A2;

        }
        private string Rep_in(string x)
        {
            return x.Replace("{", "\\[").Replace("}", "\\]")
                .Replace("\\[", "\\\\(").Replace("\\]", "\\\\)")
                ;

        }
        private string Rep_out(string x) { 
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

        public string find_i(int i) {
            string type = "", val = "";
            //Console.WriteLine($"'{this.value}'");
            //foreach (string s in list) { Console.WriteLine("'" + s + "'" + "\n"); } // тесты закончены, парсер работает четко
            int n = -1;
            foreach (string s in list) {
                n++; // самый первый символ - [, нет смысла рассматривать

                if ((n + 1) / 2 == i + 1) { //значится нашли
                    if ((n - 1) % 2 == 0)
                    {
                        // это тип данных
                        type = s;
                    }
                    else if ((n - 1) % 2 == 1) {
                        // это значение
                        val = Rep_out(s);
                    }
                }
            }

            return type + " " + val;
        }
    }


    internal class Program
    {
        static public List<str_cl> Var_str = new List<str_cl>();
        static public List<int_cl> Var_int = new List<int_cl>();
        static public List<list_cl> Var_list= new List<list_cl>();

        static public char[] chars = {'{', '}', ' ', '[', ']'};
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



            X = x.Replace("\\*", "");
            

            return X;
        }

        static string Rep(string x)
        {
            string X;



            X = x.Replace("\\n", "\n") // \n - аналогично enter
                .Replace("\\_", " ") // это важно, т. к. в моменте идет разделение пробелом
                .Replace("\\0", "") // иногда это важно и можно сказать спасает
                .Replace("\\*", "")
                .Replace("\\;", ":") // иногда это важно т. к. : нужно будет
                .Replace("\\.", ",") // иногда это важно т. к. , нужно будет
                .Replace("\\t", "\t") // табы
                .Replace("\\(", "[").Replace("\\)", "]") // для квадратных 
                .Replace("\\[", "{").Replace("\\]", "}")  // для фигурных 
                ;
            X = X.Replace("\\`", "\\") // это обязательно делать в конец

            ; // по идеи этого достаточно для полноценной системы

            return X;
        }

        static string Rep_in(string x)
        {
            string X;
            if (x != "")
                X = x.Replace("{", "\\[").Replace("}", "\\]")
               .Replace("\\[", "\\\\(").Replace("\\]", "\\\\)")
               .Replace(" ", "\\_")
               .Replace(",", "\\.")
               .Replace(":", ";") // это надо для того чтоб в список нормально загружалось
               ;
            else {
                X = "\\0";
            }

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

            for (int i = 0; i < Var_list.Count; i++)
            {
                if (Var_list[i].name == a)
                {
                    A = Var_list[i].value; break;
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
        static string _sob(List<string> A, int j)
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
                        a += Rep_in(find(A[i]));
                    
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
                    a += find(A[i]).Replace(" ", "\\_");
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
                is_command = false; bool is_many = false;
                a = Console.ReadLine();
                if (a.Contains("\\*")) {
                    is_many = true;
                }
                while (is_many) {
                    Console.Write(">\r");
                    string _a = Console.ReadLine();
                    a += "\n" +_a;
                    if (_a.Contains("\\*")) {
                        is_many = false;
                    }
                }

                A = new List<string>(Parse(a));

                try
                {
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

                    if (A[0] == "printf") // что и обычный print но без экранизации, только многострочный режим
                    {
                        is_command = true;
                        Console.WriteLine(Rep_not_space(ColorRep(sob(A, 2))));
                    }

                    if (A[0] == "echo")
                    {
                        is_command = true;
                        Console.Write(Rep(ColorRep(sob(A, 2)))); // без WriteLine
                    }

                    if (a == "vars" || a == "variables")
                    {
                        is_command = true;
                        Console.WriteLine("Строковые переменные:");
                        foreach (var v in Var_str)
                        {
                            Console.WriteLine($"  {v.name} = \"{v.value}\"");
                        }
                        Console.WriteLine("Числовые переменные:");
                        foreach (var v in Var_int)
                        {
                            Console.WriteLine($"  {v.name} = {v.value}");
                        }
                        Console.WriteLine("Списковые переменные:");
                        foreach (var v in Var_list)
                        {
                            Console.WriteLine($"  {v.name} = {v.value}");
                        }
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

                        if (A[2] == "list")
                        {
                            var val = Rep_not_space(sob(A, 6)); // в листе Rep не нужен, он сам там все будет делать у себя
                            if (!inA(A, A[4]))
                                Var_list.Add(new list_cl(A[4], val));
                            else
                            {
                                n = findA(A, A[4]);
                                Var_list[n] = new list_cl(A[4], val);
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
                        if (A[0] == "list")
                        {
                            is_command = true;

                            string val = Rep_not_space(sob(A, 4)); // в листе Rep не нужен, он сам там все будет делать у себя
                            if (!inA(A, A[2]))
                                Var_list.Add(new list_cl(A[2], val));
                            else
                            {
                                n = findA(A, A[2]);
                                Var_list[n] = new list_cl(A[2], val);
                            }
                        }
                    }

                    if (A[0] == "element") // element name index list - элемент с каким-то индексом
                                           // element list index is val - задать элемент списка каким-то
                        //тип данных писать не надо, т. к. он опредлится автоматически
                    {
                        is_command = true; int k = 2;

                        List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob(A, 0))).Split(" "));
                        var reg = new List<string>();
                        foreach (string i in reg2)
                        {
                            reg.Add(Rep_not_space(i)); reg.Add(" ");
                        }

                        var A2 = reg;
                        if (A2[3 * k] != "is")
                        {
                            list_cl list = new list_cl(A2[1 * k], Rep_not_space(sob(A2, 3 * k)));
                            var val2 = new List<string>(list.find_i(int.Parse(A2[2 * k])).Split(" "));
                            string type = val2[0];
                            string val = sob2(val2, 1);
                            if (type == "str")
                            {
                                if (!inA(A2, A2[1 * k]))
                                    Var_str.Add(new str_cl(A2[1 * k], val));
                                else
                                {
                                    n = findA(A, A[1]);
                                    Var_str[n] = new str_cl(A2[1 * k], val);
                                }
                            }
                            if (type == "int")
                            {
                                if (!inA(A2, A2[1 * k]))
                                    Var_int.Add(new int_cl(A2[1 * k], int.Parse(val)));
                                else
                                {
                                    n = findA(A, A[1]);
                                    Var_int[n] = new int_cl(A2[1 * k], int.Parse(val));
                                }
                            }
                            if (type == "list")
                            {
                                if (!inA(A2, A2[1 * k]))
                                    Var_list.Add(new list_cl(A2[1 * k], val));
                                else
                                {
                                    n = findA(A, A[1]);
                                    Var_list[n] = new list_cl(A2[1 * k], val);
                                }
                            }
                        }
                    }

                    if (A[0] == "del_var" && A.Count > 1)
                    {
                        is_command = true;
                        string varName = A[2];

                        // Удалить из Var_str
                        for (int i = Var_str.Count - 1; i >= 0; i--)
                        {
                            if (Var_str[i].name == varName)
                            {
                                Var_str.RemoveAt(i);
                            }
                        }

                        // Удалить из Var_int
                        for (int i = Var_int.Count - 1; i >= 0; i--)
                        {
                            if (Var_int[i].name == varName)
                            {
                                Var_int.RemoveAt(i);
                            }
                        }

                        // Удалить из Var_list
                        for (int i = Var_list.Count - 1; i >= 0; i--)
                        {
                            if (Var_list[i].name == varName)
                            {
                                Var_list.RemoveAt(i);
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

                    if (A[0] == "add")
                    {
                        is_command = true;
                        string reg = Rep(sob(A, 4));

                        n = findA(A, A[2]);
                        t = inO(A, A[2]);

                        if (t == "int")
                        {
                            Var_int[n].value += int.Parse(reg);
                        }
                        if (t == "str")
                        {
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

                        string filePath = reg[0].Replace("\n", "\\n");
                        string text = sob2(reg, 1);

                        File.WriteAllTextAsync(filePath, text, Encoding.UTF8);


                        is_command = true;


                    }

                    if (A[0] == "append")
                    {
                        List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                        var reg = new List<string>();
                        foreach (string i in reg2)
                        {
                            reg.Add(Rep(i));
                        }

                        string filePath = reg[0].Replace("\n", "\\n");
                        string text = sob2(reg, 1);

                        File.AppendAllText(filePath, text, Encoding.UTF8);
                    }

                    if (A[0] == "del")
                    {
                        List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                        var reg = new List<string>();
                        foreach (string i in reg2)
                        {
                            reg.Add(Rep(i));
                        }

                        string filePath = reg[0].Replace("\n", "\\n");

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

                        string folderPath = reg[0].Replace("\n", "\\n");

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

                        string filePath = reg[0].Replace("\n", "\\n");


                        is_command = true;

                        string allContent = File.ReadAllText(filePath, Encoding.UTF8);


                        Console.WriteLine(allContent);


                    }

                    if (A[0] == "read_var")
                    {
                        List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                        var reg = new List<string>();
                        foreach (string i in reg2)
                        {
                            reg.Add(Rep(i));
                        }

                        string name_var = reg[0];
                        string filePath = reg[1].Replace("\n", "\\n");


                        is_command = true;

                        string allContent = File.ReadAllText(filePath, Encoding.UTF8);


                        string val = allContent;
                        if (!inA(A, A[2]))
                            Var_str.Add(new str_cl(A[2], val));
                        else
                        {
                            n = findA(A, A[2]);
                            Var_str[n] = new str_cl(A[2], val);
                        }


                    }

                    if (A[0] == "create_nwv")// nwv - not with var, типо не с переменными и без интерполяцией (что невсегда удобно)
                    {
                        string filePath = A[2];

                        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) { writer.WriteLine(""); }
                        ; // по умолчанию файл пустой

                        is_command = true;


                    }

                    if (A[0] == "create")
                    {
                        List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                        var reg = new List<string>();
                        foreach (string i in reg2)
                        {
                            reg.Add(Rep(i));
                        }

                        string filePath = reg[0].Replace("\n", "\\n");

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

                        string folderPath = reg[0].Replace("\n", "\\n");

                        DirectoryInfo directoryInfo = Directory.CreateDirectory(folderPath);

                        is_command = true;

                    }

                    if (A[0] == "open")
                    {
                        List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                        var reg = new List<string>();
                        foreach (string i in reg2)
                        {
                            reg.Add(Rep(i));
                        }

                        string filePath = reg[0].Replace("\n", "\\n");

                        try
                        {
                            // Вариант 1: С ProcessStartInfo
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = filePath,
                                UseShellExecute = true  // Важно для Windows!
                            });

                            // Или вариант 2: Проверить существование
                            if (File.Exists(filePath))
                            {
                                Process.Start(filePath);
                            }
                            else
                            {
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        is_command = true;
                    }

                    if (A[0] == "exists")
                    {
                        List<string> reg2 = new List<string>(Rep_not_space(ColorRep(sob_not_space(A, 2))).Split(" "));
                        var reg = new List<string>();
                        foreach (string i in reg2)
                        {
                            reg.Add(Rep(i));
                        }

                        string path = reg[0].Replace("\n", "\\n");
                        if (File.Exists(path))
                            Console.WriteLine($"Файл '{path}' существует");
                        else if (Directory.Exists(path))
                            Console.WriteLine($"Папка '{path}' существует");
                        else
                            Console.WriteLine($"'{path}' не найден");
                    }

                    if (A[0] == "dir" || A[0] == "ls")
                    {
                        is_command = true;
                        string path = ".";
                        if (A.Count > 1) path = Rep(sob(A, 2, 2));

                        if (Directory.Exists(path))
                        {
                            Console.WriteLine($"Содержимое {path}:");
                            foreach (var file in Directory.GetFiles(path))
                            {
                                Console.WriteLine($">[ФАЙЛ]   {Path.GetFileName(file)}");
                            }
                            foreach (var dir in Directory.GetDirectories(path))
                            {
                                Console.WriteLine($">[ПАПКА]  {Path.GetFileName(dir)}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Папка '{path}' не найдена");
                        }
                    }

                    if (A[0] == "dir_rf") // рекурсивно обрабатывать запрос
                    {
                        string mul(char a, int b) {
                            string A="";
                            for (int i = 0; i < b; i++)
                            {
                                A += a;
                            }

                            return A;

                        }
                        void dir_rf(string? path, int level)
                        {
                            int a = 3;
                            foreach (var file in Directory.GetFiles(path))
                            {
                                Console.WriteLine($"{mul('>', level)}{mul(' ', level * a)}[ФАЙЛ]  {Path.GetFileName(file)}");
                            }
                            foreach (var dir in Directory.GetDirectories(path))
                            {
                                Console.WriteLine($"{mul('>', level)}{mul(' ', level * a)}[ПАПКА] {Path.GetFileName(dir)}");
                                dir_rf(path + "\\" + Path.GetFileName(dir), level + 1);
                            }
                        }

                        is_command = true;
                        string path = ".";
                        if (A.Count > 1) path = Rep(sob(A, 2, 2));

                        if (Directory.Exists(path))
                        {
                            Console.WriteLine($"Содержимое {path}:");
                            dir_rf(path, 1);
                        }
                        else
                        {
                            Console.WriteLine($"Папка '{path}' не найдена");
                        }
                    }


                    if (a == "clear")
                    {
                        is_command = true;
                        Console.Clear();
                    }

                    if (a == "help" || a == "?" || a == "info")
                    {
                        is_command = true;
                        Console.WriteLine("=== СПРАВКА ===");
                        Console.WriteLine("Основные команды:");
                        Console.WriteLine("  print <текст>            - вывод с переносом строки");
                        Console.WriteLine("  echo <текст>             - вывод без переноса");
                        Console.WriteLine("  str <имя> <значение>     - строковая переменная");
                        Console.WriteLine("  int <имя> <значение>     - числовая переменная");
                        Console.WriteLine("  add <имя> <значение>     - прибавить к переменной");
                        Console.WriteLine("  vars                     - список всех переменных");
                        Console.WriteLine("  del_var <имя>            - удалить переменную");

                        Console.WriteLine("\nЦвета:");
                        Console.WriteLine("  color reset              - сброс цвета");
                        Console.WriteLine("  color <цвет>             - white, black, blue, green, red");
                        Console.WriteLine("  color rgb <r> <g> <b>    - RGB цвет");

                        Console.WriteLine("\nФайлы и папки:");
                        Console.WriteLine("  dir [путь]               - содержимое папки");
                        Console.WriteLine("  dir_rf [путь]            - рекурсивный обход");
                        Console.WriteLine("  exists <путь>            - проверить существование");
                        Console.WriteLine("  read <файл>              - прочитать файл");
                        Console.WriteLine("  write <файл> <текст>     - записать в файл");
                        Console.WriteLine("  create <файл>            - создать файл");
                        Console.WriteLine("  del <файл>               - удалить файл");
                        Console.WriteLine("  open <файл>              - открыть файл");

                        Console.WriteLine("\nЭкранизация:");
                        Console.WriteLine("  \\0                       - пустое значение");
                        Console.WriteLine("  \\n                       - следующая строка");
                        Console.WriteLine("  \\_                       - пробел");
                        Console.WriteLine("  \\`                       - слеш (\\)");
                        Console.WriteLine("  \\*                       - начать/закончить многострочный ввод");
                        Console.WriteLine("  \\[                       - {");
                        Console.WriteLine("  \\]                       - }");
                        Console.WriteLine("  \\u0m                     - вернуть цвет к нормальному");
                        Console.WriteLine("  \\uR.G.Bm                 - задать цвет формата RGB");

                        Console.WriteLine("\nПрочее:");
                        Console.WriteLine("  clear                    - очистить экран");
                        Console.WriteLine("  history                  - история команд");
                        Console.WriteLine("  exit                     - выход");
                        Console.WriteLine("=== КОНЕЦ СПРАВКИ ===");
                    }


                    if (is_command)
                    {
                        History.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    // чтоб при ошибке программа не вылетала а продолжала работать
                    Console.WriteLine(red+"Вызвана ошибка" + select_color);
                }
            }


        }
    }
}

