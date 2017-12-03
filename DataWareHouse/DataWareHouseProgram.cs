using MyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataWareHouse
{
    class DataWareHouseProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter 1 or 2 to start warehouses");

            switch (Console.ReadLine())
            {
                case "1":
                    WareHouseHttpServer server1 = new WareHouseHttpServer(8001);
                    break;
                case "2":
                    WareHouseHttpServer server2 = new WareHouseHttpServer(8002);
                    break;
            }


            Console.ReadKey();
        }
    }
}
