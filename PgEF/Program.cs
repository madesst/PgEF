using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Devart.Data.Linq;
using PgEFModel;

namespace PgEF
{
    class Program
    {
        public static void Main(String[] args)
        {
            var db = new PgEFModel.CustomDataContext();
            Table<Client> clients = db.GetTable<Client>();
            Table<Payment> payments = db.GetTable<Payment>();

            Console.WriteLine("Clients listing:");

            foreach (var client in clients)
            {
                Console.WriteLine(client);
            }

            foreach (var payment in payments)
            {
                Console.WriteLine(payment.ToString());
            }

            var complexQuery = from c in clients
                select new
                {
                    c.Id,
                    x = (from p in payments
                        where p.ClientId == c.Id
                        select p).OrderByDescending(q => q.CreatedAt).Take(3).Average(q => q.Amount)
                };

            Console.WriteLine(complexQuery.ToString());

            foreach (var something in complexQuery)
            {
                Console.WriteLine(something.Id + " | " + something.x);
            }

            Console.Read();
        }
    }
}


        