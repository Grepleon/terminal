using Console2;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
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

    class real_cl
    {
        public string name;
        public double value;

        public real_cl(string name, double val)
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
        public List<string> list;



        public list_cl(string name, string val)
        {
            this.name = name;
            this.value = val;
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
                    flag = !flag; if (new_add != "") A2.Add(new_add); new_add = ""; if (!chars2.Contains(s2)) { A2.Add(s2.ToString(CultureInfo.InvariantCulture)); }
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
                        val = s;
                    }
                }
            }

            return type + " " + val;
        }

        public void write_element(int i, string _type, string _value)
        {
            string type = "", val = "";
            int n = -1;
            for (int j = 0; j < list.Count; j++)
            {
                var s = list[j];
                n++; // самый первый символ - [, нет смысла рассматривать

                if ((n + 1) / 2 == i + 1)
                { //значится нашли
                    if ((n - 1) % 2 == 0)
                    {
                        // это тип данных
                        type = s;
                        if (_type != "auto") // auto - не менять тип данных
                            list[n] = _type;
                    }
                    else if ((n - 1) % 2 == 1)
                    {
                        // это значение
                        val = s;
                        list[n] = _value;
                    }
                }
            }
            new_string();

        }

        private void new_string()
        {
            string new_val = ""; int n = 0;
            foreach (string s in list) { new_val += s + (n % 2 != 0 || n == 0 || n == list.Count - 2 ? " " : ", "); n++; }
            //                                                       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            //                                                    обрезаем запятые в конце и в начале
            this.value = new_val; // обновляем значение stringа, чтоб отображалось правильно
        }

        public void add(string _type, string _value)
        {
            string type = "", val = "";
            int n = -1;
            list.RemoveAt(list.Count - 1);
            list.Add(_type);
            list.Add(_value);
            list.Add("]");
            new_string();

        }

        public void pop(int i)
        {
            string type = "", val = "";
            int n = -1;
            var list2 = new List<string>();

            int N = list.Count;
            for (int j = 0; j < N; j++)
            {
                list2.Add(list[j]);
            }

            for (int j = 0; j < N; j++)
            {

                var s = list[j];
                n++; // самый первый символ - [, нет смысла рассматривать

                if ((n + 1) / 2 == i + 1)
                { //значится нашли
                    if ((n - 1) % 2 == 0)
                    {
                        list2.RemoveAt(n);
                    }
                    else if ((n - 1) % 2 == 1)
                    {
                        list2.RemoveAt(n - 1); // т.к. один уже удален
                    }
                }

            }

            this.list = new List<string>();
            for (int j = 0; j < list2.Count; j++)
            {
                list.Add(list2[j]);
            }
            new_string();

        }
    }

    class bool_cl
    {
        public string name;
        public bool value;

        public bool_cl(string name, bool val)
        {
            this.name = name;
            this.value = val;
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

        static public List<str_cl>  Var_str      = new List<str_cl>  ();
        static public List<int_cl>  Var_int      = new List<int_cl>  ();
        static public List<real_cl> Var_real     = new List<real_cl> ();
        static public List<list_cl> Var_list     = new List<list_cl> ();
        static public List<bool_cl> Var_bool     = new List<bool_cl>();

        static public char[]     chars           = { '{', '}', ' ', '[', ']' };
        static public char[]     chars2          = { '{', '}' };

        static public bool       flag_exit       = false;

        static public int        level_condition = 0;
        static public List<bool> conditions      = new List<bool>();
        static public bool       condition_if    = true;

        static List<string> Parse(string a)
        {
            List<string> A2 = new List<string>();
            bool flag = false; string new_add = "";
            foreach (char s2 in a)
            {

                if (chars.Contains(s2))
                {
                    flag = !flag; A2.Add(new_add); new_add = ""; A2.Add(s2.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    new_add += s2;
                }
            }

            A2.Add(new_add);

            return A2;
        }

        static List<string> Parse_var(List<string> A) {
            List<string> reg2 = new List<string>(Collect_ns(A, 2).Split(" "));
            var reg = new List<string>();
            foreach (string i in reg2)
            {
                reg.Add(Rep_out(i));
            }
            return reg;
        }

        static List<string> listParse_var(List<string> A)
        {
            List<string> reg2 = new List<string>(Collect_ns(A, 2).Split(" "));
            var reg = new List<string>();
            foreach (string i in reg2)
            {
                reg.Add(i);
            }
            return reg;
        }

        static List<string> Input() {
            Console.Write(""); 
            bool   is_many = false;
            string a = Console.ReadLine().Split("\\\\")[0]; // \\ - комментарий
            
            if (a.Contains("\\*"))
            {
                is_many = true;
            }

            while (is_many)
            {
                Console.Write(">\r");
                string _a = Console.ReadLine().Split("\\\\")[0];
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
                    A = Var_int[i].value.ToString(CultureInfo.InvariantCulture); break;
                }
            }

            for (int i = 0; i < Var_real.Count; i++)
            {
                if (Var_real[i].name == a)
                {
                    A = Var_real[i].value.ToString(CultureInfo.InvariantCulture); break;
                }
            }

            for (int i = 0; i < Var_list.Count; i++)
            {
                if (Var_list[i].name == a)
                {
                    A = Var_list[i].value; break;
                }
            }

            for (int i = 0; i < Var_bool.Count; i++)
            {
                if (Var_bool[i].name == a)
                {
                    A = Var_bool[i].value.ToString(CultureInfo.InvariantCulture); break;
                }
            }

            return A;
        }

        
        static bool exists(string n)
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
            foreach (var i in Var_real)
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
            foreach (var i in Var_bool)
            {
                if (i.name == n)
                {
                    flag = true;
                }
            }

            return flag;
        }

        static string type(string n)
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
            foreach (var i in Var_real)
            {
                if (i.name == n)
                {
                    flag = "real";
                }
            }
            foreach (var i in Var_list)
            {
                if (i.name == n)
                {
                    flag = "list";
                }
            }
            foreach (var i in Var_bool)
            {
                if (i.name == n)
                {
                    flag = "bool";
                }
            }

            return flag;
        }

        static int number(string n)
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
            foreach (var i in Var_real)
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
            n2 = 0;
            foreach (var i in Var_bool)
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

        static string Collect(List<string> A, int j, string sep)
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
                    a += A[i] + ((i == A.Count - 1) ? "" : sep);
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

        static string Collect_ns(List<string> A, int j)
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
                .Replace("\\_", " ")
                .Replace("\\;", ":")
                .Replace("\\.", ",")
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

        static string Rep_in(string x)
        {
            if (string.IsNullOrEmpty(x)) return "\\0";

            return x
                .Replace("{", "\\[")
                .Replace("}", "\\]")
                .Replace("\\[", "\\\\(")
                .Replace("\\]", "\\\\)")
                .Replace(" ", "\\_")
                .Replace("\n", "\\n")
                .Replace(",", "\\.")
                .Replace(":", "\\;")
                ;
        }

        static string listRep_in(string x)
        {
            if (string.IsNullOrEmpty(x)) return "\\0";

            return x
                .Replace("{", "\\[")
                .Replace("}", "\\]")
                .Replace("\\[", "\\\\(")
                .Replace("\\]", "\\\\)")
                .Replace("\n", "\\n")
                .Replace(":", "\\;")
                ;
        }

        static string Rep_into(string x)
        {
            if (string.IsNullOrEmpty(x)) return "\\0";

            return x
                .Replace("\\", "\\`")
                .Replace("{", "\\[")
                .Replace("}", "\\]")
                .Replace("\\[", "\\\\(")
                .Replace("\\]", "\\\\)")
                .Replace(" ", "\\_")
                .Replace("\n", "\\n")
                .Replace(",", "\\.")
                .Replace(":", "\\;")
                ;
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

        static double to_real(string n) 
        { 
            double a = double.Parse(n.Replace(",", "."), CultureInfo.InvariantCulture);
            return a;
        }
        static double to_real(int n)
        {
            double a = n;
            return a;
        }

        static void ShowError(string error) {
            Console.WriteLine(red + error + select_color);
        }

        static void exit_fun() {
            flag_exit = true;
        }

        static void clear_fun()
        {
            Console.Clear();
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

        static void CreateVar(string name, string val) {
            if (!exists(name))
                Var_str.Add(new str_cl(name, val));
            else
            {
                int num = number(name);
                Var_str[num] = new str_cl(name, val);
            }
        }

        static void CreateVar(string name, bool val)
        {
            if (!exists(name))
                Var_bool.Add(new bool_cl(name, val));
            else
            {
                int num = number(name);
                Var_bool[num] = new bool_cl(name, val);
            }
        }

        static void CreateVar(string name, int val)
        {
            if (!exists(name))
                Var_int.Add(new int_cl(name, val));
            else
            {
                int num = number(name);
                Var_int[num] = new int_cl(name, val);
            }
        }

        static void CreateVar(string name, double val)
        {
            if (!exists(name))
                Var_real.Add(new real_cl(name, val));
            else
            {
                int num = number(name);
                Var_real[num] = new real_cl(name, val);
            }
        }

        static void CreateList(string name, string val)
        {
            if (!exists(name))
                Var_list.Add(new list_cl(name, val));
            else
            {
                int num = number(name);
                Var_list[num] = new list_cl(name, val);
            }
        }

        static void str_var_create(List<string> list, int k)
        { 
            string name = list[k];
            string val  = Rep_in(Collect(list, k + 2));

            CreateVar(name, val);
        }

        static void int_var_create(List<string> list, int k)
        {
            string name = list[k];
            int val = int.Parse(Collect(list, k + 2));
            int num = -1;

            CreateVar(name, val);
        }

        static void bool_var_create(List<string> list, int k)
        {
            string name = list[k];
            string _val = Collect(list, k + 2);
            bool val=false;
            if (_val.ToLower() == "false") val = false;
            else if (_val.ToLower() == "true") val = true;
            else { // а далее выражения
                var list_val = new List<string>(_val.Split());
                // ! a - отрицание
                // not a - отрицание
                // a == b - равно?
                // a != b - не равно?
                // a > b - больше
                // a >= b - больше равно
                // a < b - меньше
                // a <= b - меньше b
                // a and b - лог. И
                // a or b - лог. ИЛИ
                if (list_val.Count() == 2)
                {
                    val = !(bool.Parse(list_val[1]));
                }
                else {
                    switch (list_val[1]) {
                        case "==":
                            val = list_val[0] == list_val[2];
                            break;
                        case "!=":
                            val = list_val[0] != list_val[2];
                            break;
                        case ">=":
                            val = to_real(list_val[0]) >= to_real(list_val[2]);
                            break;
                        case "<=":
                            val = to_real(list_val[0]) <= to_real(list_val[2]);
                            break;
                        case ">":
                            val = to_real(list_val[0]) >  to_real(list_val[2]);
                            break;
                        case "<":
                            val = to_real(list_val[0]) < to_real(list_val[2]);
                            break;
                        case "and":
                            val = bool.Parse(list_val[0]) && bool.Parse(list_val[2]);
                            break;
                        case "or":
                            val = bool.Parse(list_val[0]) || bool.Parse(list_val[2]);
                            break;
                    }
                }
            }
                int num = -1;

            CreateVar(name, val);
        }

        static void real_var_create(List<string> list, int k)
        {
            string name = list[k];
            double val = to_real(Collect(list, k + 2));
            int num = -1;

            CreateVar(name, val);
        }

        static void list_var_create(List<string> list, int k)
        {
            string name = list[k];
            string val = listRep_in(Collect(list, k + 2));

            CreateList(name, val);
        }

        static void set_var(List<string> list)
        {
            if (list[2] == "str")
                str_var_create(list, 4);
            else if (list[2] == "int")
                int_var_create(list, 4);
            else if (list[2] == "real")
                real_var_create(list, 4);
            else if (list[2] == "list")
                list_var_create(list, 4);
            else if (list[2] == "bool")
                bool_var_create(list, 4);
            else // иначе: (для него уже нужна созданная переменная)
                 // set var val (не нужно указывать тип данных)
            {
                string name = list[2];
                string t = type(name);
                if (t == "str")   str_var_create(list, 2);
                if (t == "int")   int_var_create(list, 2);
                if (t == "real") real_var_create(list, 2);
                if (t == "list") list_var_create(list, 2);
                if (t == "bool") bool_var_create(list, 2);
            }
        }

        static void decide_var(List<string> list)
        {
            // сделаю позже
        }

        static void get_var(List<string> list)
        {
            string name = list[2];
            Console.Write(green + ">\r");
            string val = Console.ReadLine();
            Console.Write(reset);

            CreateVar(name, val);
        }

        static void del_var(List<string> list)
        {
            string name = list[2];
            string t    = type(name);
            int    index = number(name);
            if (exists(name))
            {
                if (t == "int")
                    Var_int.RemoveAt(index);
                if (t == "real")
                    Var_real.RemoveAt(index);
                if (t == "list")
                    Var_list.RemoveAt(index);
                if (t == "str")
                    Var_str.RemoveAt(index);
            }

        }

        static void add_vars(List<string> list)
        {
            string reg  = Rep_outf(Collect(list, 4));

            string name = list[2];
            string t    = type(name);
            int    num  = number(name);

            if (t == "int") {
                Var_int[num].value += int.Parse(reg);
            }
            if (t == "real")
            {
                Var_real[num].value += to_real(reg);
            }
            if (t == "str"){
                Var_str[num].value += reg;
            }
            if (t == "list")
            {
                // добавить элемент
                // add list type value
                reg = Rep_outf(Collect(list, 6));
                string _t = list[4];
                Var_list[num].add(_t, reg);
            }
        }

        static void pop_list(List<string> list)
        {
            string reg = Rep_outf(Collect(list, 4));

            string name = list[2];
            string t = type(name);
            int num = number(name);

            if (t == "list")
            {
                // удалить элемент по индексу
                // pop list index
                Var_list[num].pop(int.Parse(reg));
            }
        }

        static void sub_vars(List<string> list)
        {
            string reg = Rep_outf(Collect(list, 4));

            string name = list[2];
            string t    = type(name);
            int    num  = number(name);

            if (t == "int")
            {
                Var_int[num].value -= int.Parse(reg);
            }
            if (t == "real")
            {
                Var_real[num].value -= to_real(reg);
            }
        }

        static void multy_vars(List<string> list)
        {
            string reg = Rep_outf(Collect(list, 4));

            string name = list[2];
            string t    = type(name);
            int    num  = number(name);

            if (t == "int"){
                Var_int[num].value *= int.Parse(reg);
            }
            if (t == "real")
            {
                Var_real[num].value *= to_real(reg);
            }
            if (t == "str"){
                string val = Var_str[num].value;
                Var_str[num].value = "";
                for (int i = 0; i < int.Parse(reg); i++) Var_str[num].value += val;

                if (Var_str[num].value == "") { Var_str[num].value = "\\0"; } // просто ничего хранить нельзя
            }
        }

        static int div_vars(List<string> list)
        {
            string reg = Rep_outf(Collect(list, 4));

            string name = list[2];
            string t    = type(name);
            int    num  = number(name);

            if (t == "int")
            {
                if (int.Parse(reg) == 0) { ShowError("Деление на 0 невозможно"); return 0; }

                Var_int[num].value /= int.Parse(reg);
            }
            if (t == "real")
            {
                if (to_real(reg) == 0) { ShowError("Деление на 0 невозможно"); return 0; }

                Var_real[num].value /= to_real(reg);
            }

            return 1;
        }

        static void pow_vars(List<string> list)
        {
            string reg = Rep_outf(Collect(list, 4));

            string name = list[2];
            string t    = type(name);
            int    num  = number(name);

            if (t == "int")
            {
                Var_int[num].value = (int)Math.Pow(Var_int[num].value, int.Parse(reg));
            }
            if (t == "real")
            {
                Var_real[num].value = Math.Pow(Var_real[num].value, to_real(reg));
            }

        }

        static int mod_vars(List<string> list)
        {
            string reg = Rep_outf(Collect(list, 4));

            string name = list[2];
            string t   = type(name);
            int   num  = number(name);

            if (t == "int")
            {
                int divisor = int.Parse(reg);

                if (divisor == 0) { ShowError("Деление на 0 невозможно"); return 0; }

                int result = Var_int[num].value % divisor;
                // Делаем результат неотрицательным
                if (result < 0) result += divisor;
                Var_int[num].value = result;
            }

            if (t == "real")
            {
                double divisor = to_real(reg);
                if (divisor == 0) { ShowError("Деление на 0 невозможно"); return 0; }

                // Для дробных: fmod (остаток от деления)
                Var_real[num].value = Var_real[num].value % divisor;
            }

            return 1;
        }

        static void len_list(List<string> list)
        {
            // len var list \\ var - всегда intовая

            var reg = Parse_var(list);

            string name = reg[0];
            string val = Collect(reg, 1, " ");
            list_cl _list = new list_cl("new_list", val);

            CreateVar(name, _list.list.Count() / 2 - 1);

        }
        static void to_type(List<string> list)
        {
            string name = list[0];
            string t = type(name);
            string type_to = list[4];
            int n = number(name);

            if (t == "str")
            {
                string value = Var_str[n].value;
                if (type_to == "int")
                {
                    Var_str.RemoveAt(n);
                    Var_int.Add(new int_cl(list[0], Convert.ToInt32(value)));
                }
                if (type_to == "real")
                {
                    Var_str.RemoveAt(n);
                    Var_real.Add(new real_cl(list[0], to_real(value)));
                }
                if (type_to == "bool")
                {
                    Var_str.RemoveAt(n);
                    Var_bool.Add(new bool_cl(list[0], bool.Parse(value)));
                }
            }
            if (t == "int")
            {
                int value = Var_int[n].value;
                if (type_to == "str")
                {
                    Var_int.RemoveAt(n);
                    Var_str.Add(new str_cl(list[0], value.ToString()));
                }
                if (type_to == "real")
                {
                    Var_int.RemoveAt(n);
                    Var_real.Add(new real_cl(list[0], to_real(value)));
                }
            }
            if (t == "real")
            {
                double value = Var_real[n].value;
                if (type_to == "str")
                {
                    Var_real.RemoveAt(n);
                    Var_str.Add(new str_cl(list[0], value.ToString()));
                }
                if (type_to == "int")
                {
                    Var_real.RemoveAt(n);
                    Var_int.Add(new int_cl(list[0], (int)value));
                }
            }
            if (t == "bool")
            {
                bool value = Var_bool[n].value;

                if (type_to == "str")
                {
                    Var_bool.RemoveAt(n);
                    Var_str.Add(new str_cl(list[0], value.ToString()));
                }
            }
        }

        static void create_file(List<string> list) {
            List<string> list_file = Parse_var(list);
            string filePath = list_file[0];
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) { writer.WriteLine(""); }
                        ; // по умолчанию файл пустой
        }

        static void write_file(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string       filePath  = list_file[0];
            string       text      = Collect(list_file, 1, " ");
            if (File.Exists(filePath))
                File.WriteAllTextAsync(filePath, text, Encoding.UTF8);
            else { ShowError("Файла не существует"); }

        }
        static void append_file(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string filePath = list_file[0];
            string text = Collect(list_file, 1, " ");
            if (File.Exists(filePath))
                File.AppendAllText(filePath, text, Encoding.UTF8);
            else { ShowError("Файла не существует"); }

        }

        static void read_file(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string filePath = list_file[0];
            if (File.Exists(filePath))
            {
                string allContent = File.ReadAllText(filePath, Encoding.UTF8);
                Console.WriteLine(allContent);
            }
            else { ShowError("Файла не существует"); }

        }
        static void read_var_file(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string nameVar = list_file[0];
            string filePath = list_file[1];
            if (File.Exists(filePath))
            {
                string allContent = Rep_into(File.ReadAllText(filePath, Encoding.UTF8));
                CreateVar(nameVar, allContent);
            }
            else { ShowError("Файла не существует"); }
        }

        static void del_file(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string filePath = list_file[0];
            if (File.Exists(filePath))
            { File.Delete(filePath); }
            else { ShowError("Файла не существует"); }

        }

        static void create_folder(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string folderPath = list_file[0];
            DirectoryInfo directoryInfo = Directory.CreateDirectory(folderPath);
        }

        static void del_rf_folder(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string folderPath = list_file[0];
            if (Directory.Exists(folderPath))
                Directory.Delete(folderPath, recursive: true);
            else { ShowError("Папки не существует"); }
        }

        static void exists(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string path = list_file[0];
            if (File.Exists(path))
                Console.WriteLine($"Файл '{path}' существует");
            else if (Directory.Exists(path))
                Console.WriteLine($"Папка '{path}' существует");
            else
                Console.WriteLine($"'{path}' не найден");
        }

        static void open_file(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string filePath = list_file[0];
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
                ShowError($"Возникла ошибка при открытии файла '{filePath}'");
            }

        }

        static void dir_folder(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string path = list_file[0];

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
                ShowError($"Папка '{path}' не найдена");
            }
        }

        static void dir_rf_folder(List<string> list)
        {
            List<string> list_file = Parse_var(list);
            string path = list_file[0];

            string mul(char a, int b)
            {
                string A = "";
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

        static void auto_create_var(string type, string value, string name)
        {
            if (type == "str")
            {
                CreateVar(name, value);
            }
            if (type == "int")
            {
                CreateVar(name, int.Parse(value));
            }
            if (type == "real")
            {
                CreateVar(name, double.Parse(value));
            }
            if (type == "list")
            {
                CreateList(name, value);
            }
            if (type == "bool")
            {
                CreateVar(name, bool.Parse(value));
            }
        }

        static void return_element(List<string> A) {
            var list = listParse_var(A);
            string name = list[0];
            int index = int.Parse(list[1]);
            string list_val = Collect(list, 2, " ");
            list_cl list_var = new list_cl("new_list", list_val);

       
            List<string> type_and_value = new List<string>(list_var.find_i(index).Split(" "));

            string e_type = type_and_value[0];
            string e_value = type_and_value[1];

            auto_create_var(e_type, e_value, name);
        }

        static void write_element(List<string> A)
        {
            var list = listParse_var(A);
            string name = list[0];
            list_cl _list;
            int index = int.Parse(list[1]);
            string _type = list[3];
            string value = Rep_in(Collect(list, 4, " "));
            for (int i = 0; i < Var_list.Count; i++)
            {
                if (Var_list[i].name == name)
                {
                    _list = Var_list[i]; _list.write_element(index, _type, value); break;
                }
            }
            
        }

        static void element_select(List<string> list)
        {
            // Записать i-тый элемент в переменную
            // element var i list
            // Записать переменную в i-тый элемент
            // element list i is type var
            

            if (list[6] == "is")
            {
                // Записать переменную в i-тый элемент
                write_element(list);
            }
            else 
            {
                // Записать i-тый элемент в переменную
                return_element(list);
            }
        }

        static void color_fun(List<string> list) {
            var reg = Parse_var(list);
            switch (reg[0]) {
                case "reset":
                    select_color = reset;
                    Console.Write(reset);
                    break;
                case "red":
                    select_color = red;
                    Console.Write(red);
                    break;
                case "blue":
                    select_color = blue;
                    Console.Write(blue);
                    break;
                case "green":
                    select_color = green;
                    Console.Write(green);
                    break;
                case "white":
                    select_color = white;
                    Console.Write(white);
                    break;
                case "black":
                    select_color = black;
                    Console.Write(black);
                    break;
                case "rgb":
                    string R = reg[1];
                    string G = reg[2];
                    string B = reg[3];
                    select_color = $"\u001b[38;2;{R};{G};{B}m";
                    Console.Write(select_color);
                    break;
            }
        }

        static void if_fun(List<string> list)
        {
            string collected_value = Collect(list, 2);
            level_condition++;
            conditions.Add(bool.Parse(collected_value));
            //condition_if = condition;
        }

        static void if_fun_false()
        {
            level_condition++;
            conditions.Add(false);
            //condition_if = condition;
        }

        static void end_fun()
        {
            level_condition--; conditions.RemoveAt(level_condition);
        }


        static void Out(List<string> A) {
            /*Вывод по командам*/
            bool flag = true;
            switch (A[0].Replace("\t", "")) // чтобы можно было делать табы для отступа визуального
            {
                // общие команды
                case "exit":
                    exit_fun();
                    break;

                case "clear":
                    clear_fun();
                    break;

                case "history":
                    history_fun();
                    break;

                case "color":
                    color_fun(A);
                    break;

                // ЯПшные команды
                case "print": // вывод
                    print(A);
                    break;

                case "printf": // вывод без экранизации (только без многострочного режима)
                    printf(A);
                    break;

                case "echo": // вывод без экранизации (только без многострочного режима)
                    echo(A);
                    break;

                case "set":
                    set_var(A);
                    break;

                case "get":
                    get_var(A);
                    break;

                case "del_var":
                    del_var(A);
                    break;

                case "str":
                    str_var_create(A, 2);
                    break;

                case "int":
                    int_var_create(A, 2);
                    break;

                case "real":
                    real_var_create(A, 2);
                    break;

                case "list":
                    list_var_create(A, 2);
                    break;

                case "bool":
                    bool_var_create(A, 2);
                    break;

                case "decide":
                    decide_var(A); // решить любое выражение (даже сложное)
                    break;

                case "add": // сложить/добавить в список
                    add_vars(A);
                    break;

                case "pop": // удалить по индексу
                    pop_list(A);
                    break;

                case "sub": // вычесть
                    sub_vars(A);
                    break;

                case "multy": // умножить
                    multy_vars(A);
                    break;

                case "div": // разделить
                    div_vars(A);
                    break;

                case "pow": // возвести в степень
                    pow_vars(A);
                    break;

                case "mod": // остаток от деления
                    mod_vars(A);
                    break;

                case "element":
                    element_select(A);
                    break;

                case "len":
                    len_list(A);
                    break;

                // файлы и работа с ними
                case "create": // создать файл
                    create_file(A);
                    break;

                case "write": // перезаписать
                    write_file(A);
                    break;

                case "append": // дописать
                    append_file(A);
                    break;

                case "read": // прочитать
                    read_file(A);
                    break;

                case "read_var": // записать файл в переменную
                    read_var_file(A);
                    break;

                case "del": // удалить файл
                    del_file(A);
                    break;

                case "create_rep": // создать папку
                    create_folder(A);
                    break;

                case "del_rf": // удалять папку рекурсивно
                    del_rf_folder(A);
                    break;

                case "exists": // проверить существование
                    exists(A);
                    break;

                case "open": // запустить файл
                    open_file(A);
                    break;

                case "dir": // файлы и папки в папке
                    dir_folder(A);
                    break;

                case "dir_rf": // файлы и папки в папке рекурсивно
                    dir_rf_folder(A);
                    break;

                // условия, циклы и т.д.
                case "if":
                    if_fun(A);
                    break;

                case "end":
                    end_fun();
                    break;

                case "--if":
                    string c = ""; foreach (bool s in conditions) c += s + " ";
                    Console.WriteLine(c + " " + level_condition.ToString());
                    break;

                default:
                    try
                    {
                        // ЯПшные команды
                        switch (A[2])
                        {
                            case "to": // приведение типа данных
                                to_type(A);
                                break;

                            default:
                                flag = false;
                                break;
                        }
                    }
                    catch (Exception e) { }

                    break;
            }
            if (flag) {
                history.Add(A[0]); // сохраняем только именование комманды            
            }
            
        }

        static void if_false(List<string> A){
            /*При ложном условии в условии*/
            bool flag = true;
            switch (A[0].Replace("\t", "")) // чтобы можно было делать табы для отступа визуального
            {
                case "end":
                    end_fun();
                    break;
                case "if":
                    if_fun_false();
                    break;
                case "elif":
                    level_condition++;
                    break;
                case "else":
                    level_condition++;
                    break;
                case "--if":
                    string c = ""; foreach (bool s in conditions) c += s + " ";
                    Console.WriteLine(c + " " + level_condition.ToString());
                    break;
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            Console.WriteLine("Console\r\n(c) Корпорация Consolesoft (Consolesoft Corporation). Не все права защищены.");
            List<string>? A          = new List<string>();
            int           n          = 0;
            bool          is_command = false;

            while (!flag_exit)
            {
                A = Input();
                try{
                    bool condition=false;
                    try
                    {
                        if (conditions.Count == 0) condition = true;
                        else condition = conditions[level_condition - 1];
                    }
                    catch {
                        ShowError("Вызвана ошибка при работе с условиями");
                    }

                    if (condition)
                        Out(A);
                    else
                        if_false(A);
                }
                catch (Exception e) {
                    // чтоб при ошибке программа не вылетала а продолжала работать
                    ShowError("Вызвана ошибка");  
                }
            }
        }
    }
}
