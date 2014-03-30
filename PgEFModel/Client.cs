using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.Linq.Mapping;

namespace PgEFModel
{
    public partial class Client
    {
        public override string ToString()
        {
            return Id + " | " + Email;
        }
    }
}
