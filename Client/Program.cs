using DelgateStorePOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Store store = new Store();

            store.StoreAction(()=>MyAction("Print this"));
        }

        public static void MyAction(string input)
        {
            Console.WriteLine(input);
            Console.ReadKey();
        }
    }
}
