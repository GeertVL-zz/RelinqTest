using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelinqTest2
{
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class Helper
    {
        public static double Median(this IEnumerable<int> source)
        {
            return 666;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var q = DGraphQueryFactory.Queryable<Person>();
            var r = from p in q where p.Name == "GEERT" select new { First = p.Id, Last = p.Name };

            Console.WriteLine($"{r.ToList()}");
        }
    }
}
