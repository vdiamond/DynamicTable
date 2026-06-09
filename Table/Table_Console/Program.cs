using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table;
using System.Collections;

namespace Table_Console
{
    class Program
    {
        public static void printField(int r, IColumn c)
        {
            TypeCode tc = c.TypeCode;
            switch (tc)
            {
                case TypeCode.Boolean: Console.Write(((IColumn<bool>) c)[r].ToString()); break;
                case TypeCode.Byte: Console.Write(((IColumn<Byte>)c)[r].ToString()); break;
                case TypeCode.Char: Console.Write(((IColumn<Char>)c)[r].ToString()); break;
                case TypeCode.DateTime: Console.Write(((IColumn<DateTime>)c)[r].ToString()); break;
                case TypeCode.Decimal: Console.Write(((IColumn<Decimal>)c)[r].ToString()); break;
                case TypeCode.Double: Console.Write(((IColumn<Double>)c)[r].ToString()); break;
                case TypeCode.Int16: Console.Write(((IColumn<Int16>)c)[r].ToString()); break;
                case TypeCode.Int32: Console.Write(((IColumn<Int32>)c)[r].ToString()); break;
                case TypeCode.Int64: Console.Write(((IColumn<Int64>)c)[r].ToString()); break;
                case TypeCode.String: Console.Write(((IColumn<String>)c)[r].ToString()); break;
            }
        }
        public static void print(DynamicTable t)
        {
            var first = true;
            foreach (IColumn c in t.columns)
            {
                if (!first) { Console.Write(", ");  }
                Console.Write(c.Name);
                first = false;
            }
            Console.WriteLine("");
            first = true;
            foreach (IColumn c in t.columns)
            {
                if (!first) { Console.Write(", "); }
                Console.Write("(" + c.TypeCode.ToString() + ")");
                first = false;
            }
            Console.WriteLine("");
            for (int i = 0; i < t.rows.Count; i++)
            {
                first = true;
                foreach (IColumn c in t.columns)
                {
                    if (!first) { Console.Write(", ");  }
                    printField(i, c);
                    first = false;
                }
                Console.WriteLine("");
            }
        }

        static void Main(string[] args)
        {
            var t = new DynamicTable();
            t.columns.NewColumn("Col1", TypeCode.String);
            t.columns.NewColumn("Col2", TypeCode.Int32);
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "111"), new KeyValuePair<string, object>("Col2", 222));
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "333"), new KeyValuePair<string, object>("Col2", 444));
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "555"), new KeyValuePair<string, object>("Col2", 666));
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "777"), new KeyValuePair<string, object>("Col2", 888));

            print(t);

            IEnumerator<string> it = ((IColumn<string>) t.columns["Col1"]).GetEnumerator();
            while (it.MoveNext())
            {
                Console.WriteLine(it.Current);
            }

            Console.WriteLine("-------------------------------");
            foreach (IRow r in t.rows)
            {
                Console.WriteLine(r.GetField<int>("Col2"));
            }

         }
    }
}
