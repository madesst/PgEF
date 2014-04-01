using System;
using System.Collections.Generic;
using System.Data;
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

    /*
     * Пример грязного хака по исправлению запроса обращающегося к колонке с типом geography (заставляем запрос использовать postgis функции для конвертирования из WKB в lat/lon)
     */
    [DatabaseAttribute(Name = "pgef")]
    [ProviderAttribute(typeof(SpatialPgSqlDataProvider))]
    public class CustomDataContext : DataContext
    {
    }

    public class SpatialPgSqlDataProvider : Devart.Data.PostgreSql.Linq.Provider.PgSqlDataProvider
    {
        protected override IDbCommand CreateCommand(string commandText, IDbConnection connection1, bool forBatch)
        {
            var lastLocationIndex = commandText.IndexOf("last_location", StringComparison.Ordinal);
            if (lastLocationIndex > -1)
            {
                var lastLocationTableIndex = lastLocationIndex;
                while (commandText[lastLocationTableIndex] != ' ')
                    lastLocationTableIndex--;
                lastLocationTableIndex++;
                var tableName = commandText.Substring(lastLocationTableIndex, lastLocationIndex - lastLocationTableIndex - 1);

                commandText = commandText.Replace(tableName + ".last_location", "ST_X(" + tableName + ".last_location::geometry)::text || '_' || ST_Y(" + tableName + ".last_location::geometry)::text");
            }

            return base.CreateCommand(commandText, connection1, forBatch);
        }
    }
}