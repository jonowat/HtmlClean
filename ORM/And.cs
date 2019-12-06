using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM
{
    public class And : Where
    {
        public And() { }
        protected And(string join)
        {
            this.join = join;
        }

        public static And Where(params Where[] wheres)
        {
            var and = new And();
            and.Wheres.AddRange(wheres);
            return and;
        }

        private string join = " AND ";

        public List<Where> Wheres = new List<Where>();

        public override Parameterize ToSql(int paramId = 0)
        {
            var format = "({0})";
            if(this.Wheres.Count == 1)
            {
                format = "{0}";
            }

            var sql = new StringBuilder();

            var index = paramId;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (var where in this.Wheres)
            {
                var part = where.ToSql(index);
                
                if(index > paramId)
                {
                    sql.Append(this.join);
                }

                sql.Append(part.Sql);
                foreach (var kvp in part.Parameters)
                {
                    parameters.Add(kvp.Key, kvp.Value);
                    index++;
                }

            }

            return new Parameterize
            {
                Sql = string.Format(format, sql),
                Parameters = parameters// this.Wheres.SelectMany((where, i) => where.ToSql(i + paramId).Parameters).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
    }
}
