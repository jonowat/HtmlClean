using System.Collections.Generic;
using System.Linq;

namespace ORM
{
    public partial class Where
    {
        public Where() { }

        public Where(string column, object value)
            : this(column, Comparison.Equals, value)
        {
        }

        public Where(string column, Comparison compare, object value)
        {
            this.Column = column;
            this.Compare = compare;
            this.Value = value;
        }

        public Where(string column, IList<object> values)
            :this(column, Comparison.In, values)
        {
        }

        public static Where Like(string column, string value)
        {
            return new Where(column, Comparison.Like, value);
        }

        public static Where IsNull(string column)
        {
            return new Where(column, Comparison.Is, null);
        }
        public static Where IsNotNull(string column)
        {
            return new Where(column, Comparison.IsNot, null);
        }

        public Where(string column, Comparison compare, params object[] values)
        {
            this.Column = column;
            this.Compare = compare;
            if(values != null)
            {
                this.Values.AddRange(values);
            }
        }

        public string Column;

        public Comparison Compare = Comparison.Equals;

        public object Value 
        { 
            get { return this.Values.First(); } 
            set { this.Values.Clear(); this.Values.Add(value); }
        }

        public List<object> Values = new List<object>();

        public virtual Parameterize ToSql(int paramId = 0)
        {
            var values = this.Values.Select(v => v.ToString()).ToList();
            var sql = "{0} = {1}";
            switch (this.Compare)
            {
                case Comparison.NotEquals:
                    sql = "{0} != {1}";
                    break;
                case Comparison.Like:
                    sql = "{0} LIKE {1}";
                    values = values.Select(v => v.Replace("*", "%")).ToList();
                    break;
                case Comparison.In:
                    sql = "{0} IN (" + values.Select((v, index) => "{" + (index + 1) + "}").Implode() + ")" ;
                    break;
                case Comparison.Is:
                    sql = "{0} IS NULL";
                    values.Clear();
                    break;
                case Comparison.IsNot:
                    sql = "{0} IS NOT NULL";
                    values.Clear();
                    break;
                case Comparison.Equals:
                default:
                    sql = "{0} = {1}";
                    break;
            }

            var sqlParameters = new List<object>();
            sqlParameters.Add(this.Column);
            sqlParameters.AddRange(this.Values.Select((v, index) => "@param" + (index + paramId)));

            var parameters = values.Select((value, index) => new KeyValuePair<string, string>("param" + (index + paramId), value.ToString()))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return new Parameterize
            {
                Sql = string.Format(sql, sqlParameters.ToArray()),
                Parameters = parameters
            };
        }
    }
}
