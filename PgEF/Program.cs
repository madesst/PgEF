using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace PgEF
{
    class Program
    {
        public static void Main(String[] args)
        {
            DB db = new DB();

            var clients = db.Clients;
            var payments = db.Payments;

            Console.WriteLine("Clients listing:");
            foreach (var client in clients)
            {
                Console.WriteLine(client);
            }

            Console.WriteLine("Payments listing:");
            foreach (var payment in payments)
            {
                Console.WriteLine(payment);
            }

            //Каноничный пример для MSSQL
            /*Итоговый шизофренический скуль
SELECT 
	"Project2"."id" AS "id",
	CAST ("Project2"."C1" AS float4) AS "C1" 
FROM (
	SELECT 
		"Extent1"."id" AS "id",(
			SELECT 
				CAST (avg("Project1"."amount") AS float8) AS "A1",
				count("Project1"."created_at")
			FROM (
				SELECT 
					"Extent2"."created_at" AS "created_at",
					"Extent2"."amount" AS "amount"
				FROM 
					"public"."payment" AS "Extent2"
				WHERE "Extent2"."client_id"="Extent1"."id"
			) AS "Project1"
			ORDER BY 
				"Project1"."created_at" DESC
			LIMIT 3
		) AS "C1"
	FROM 
		"public"."client" AS "Extent1"
) AS "Project2"
             */
            var sortedNames =
                from c in db.Clients
                select new
                {
                    c.ID,
                    x = (from p in db.Payments
                        where p.ClientID == c.ID
                        select p).OrderByDescending(q => q.CreatedAt).Take(3).Average(q => q.Amount)
                };

            foreach (var something in sortedNames)
            {
                Console.WriteLine(something.ID + " | " + something.x);
            }

            Console.Read();
        }
    }




    //ENTITIES MAPPINGS
    [Table("client", Schema = "public")]
    public class Client
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("email")]
        public string Email { get; set; }

        public override string ToString()
        {
            return ID + " | " + Email;
        }
    }

    [Table("payment", Schema = "public")]
    public class Payment
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("client_id")]
        public int ClientID { set; get; }
        public virtual Client Client { set; get; }
        [Column("amount")]
        public Single Amount { set; get; }

        public override string ToString()
        {
            return ID + " | " + ClientID + " | " + CreatedAt + " | " + Amount;
        }
    }

    //DBS MAPPINGS
    public class DB : DbContext
    {
        public DB() : base(nameOrConnectionString: "PgEF") { }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}


        