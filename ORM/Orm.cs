using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    public class Orm
    {
        
        private Query query = new Query();

        public Orm()
        {

        }

        protected Orm(string table)
        {
            this.query.Table = table;
        }

        public Orm From(string table)
        {
            this.query.Table = table;
            return this;
        }

        public Orm Take(int count)
        {
            this.query.Take = count;
            return this;
        }

        public Orm Skip(int count)
        {
            this.query.Skip = count;
            return this;
        }

        public Orm OrderBy(string column)
        {
            this.query.Orders.Add(new Order
            {
                Column = column
            });

            return this;
        }

        public Orm Where(Where where)
        {
            this.query.Wheres.Add(where);
            return this;
        }

        public Orm Where(string column, object value)
        {
            this.query.Wheres.Add(new ORM.Where(column, value));
            return this;
        }

        public Orm SelectAll()
        {
            this.query.Selects.Clear();
            return this;
        }

        public Orm Select(Select select)
        {
            this.query.Selects.Add(select);
            return this;
        }

        public Orm Select(params Select[] selects)
        {
            foreach (var select in selects)
            {
                this.query.Selects.Add(select);
            }

            return this;
        }

        public Orm Select(string column)
        {
            return this.Select(new[] { column });
        }

        public Orm Select(params string[] columns)
        {
            foreach (var column in columns)
            {
                this.query.Selects.Add(new Select(column));
            }

            return this;
        }

        public Orm OrderByDesc(string column)
        {
            this.query.Orders.Add(new Order
            {
                Column = column,
                Direction = Order.Dir.Descending
            });

            return this;
        }
        
        public IList<Dictionary<string, object>> Fetch(string connectionString)
        {
            var data = new List<Dictionary<string, object>>();
            var sql = this.BuildQuery();
            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(sql.Sql, connection);
                using(var reader = cmd.ExecuteReader()){
                    var colCount = reader.FieldCount;
                    while (reader.Read())
                    {
                        var datum = new Dictionary<string, object>();
                        for (int i = 0; i < colCount; i++)
                        {
                            datum.Add(reader.GetName(i), reader.GetValue(i));
                        }

                        data.Add(datum);
                    }
                }
            }

            return data;
        }

        public Parameterize BuildQuery()
        {
            return new QueryBuilder(this.query).Build();
        }
    }
}
