using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    class QueryBuilder
    {
        private Query query;

        public QueryBuilder(Query query)
        {
            this.query = query;
        }

        public Parameterize Build()
        {
            var sql = new StringBuilder();
            sql.Append("SELECT ");
            if (this.query.Take >= 0 && this.query.Skip == 0)
            {
                sql.AppendFormat("TOP {0} ", this.query.Take);
            }

            if (this.query.Selects.Count == 0)
            {
                sql.Append("*");
            }
            else
            {
                sql.Append(this.query.Selects.Select(s => s.ToSql()).Implode(", "));
            }

            if (!string.IsNullOrEmpty(this.query.Table))
            {
                sql.Append(" FROM ");
                sql.Append(this.query.Table);
            }

            var parameters = new Dictionary<string, string>();
            if (this.query.Wheres.Any())
            {
                var where = new And { Wheres = this.query.Wheres.ToList() }.ToSql();
                sql.Append(" WHERE ");
                sql.Append(where.Sql);
                parameters = where.Parameters;
            }

            if (this.query.Orders.Any())
            {
                sql.Append(" ORDER BY ");
                sql.Append(this.query.Orders.Select(order => order.ToSql()).Implode(", "));
            }

            if (this.query.Skip > 0)
            {
                if (!this.query.Orders.Any())
                {
                    sql.Append(" ORDER BY(SELECT NULL) ");
                }
                sql.AppendFormat("OFFSET {0} ROWS", this.query.Skip);
                if(this.query.Take > 0)
                {
                    sql.AppendFormat(" FETCH NEXT {0} ROWS ONLY", this.query.Take);
                }
            }

            return new Parameterize
            {
                Sql = sql.ToString(),
                Parameters = parameters
            };
        }
    }
}
