using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ORM
{
    public class Select
    {
        private static readonly Regex nonWords = new Regex(@"[^\w]");
        private string columnName { get; set; }
        private string selectName { get; set; }

        public Select() { }
        public Select(string columnName)
            : this(columnName, nonWords.Replace(columnName, string.Empty))
        {
        }

        public Select(string columnName, string selectName)
        {
            if (!new Regex(@"^(?:(\[\w+\])|(\w+))$").IsMatch(columnName))
            {
                throw new Exception("Potentially unsafe select as names");
            }

            this.columnName = columnName;

            if (nonWords.IsMatch(selectName))
            {
                throw new Exception("Potentially unsafe select as names");
            }
            this.selectName = selectName;
        }

        public static Select Raw(string sqlExpression)
        {
            return new Select()
            {
                columnName = sqlExpression,
                selectName = nonWords.Replace(sqlExpression, string.Empty)
            };
        }

        public static Select Raw(string sqlExpression, string name)
        {
            return new Select()
            {
                columnName = sqlExpression,
                selectName = name
            };
        }

        public string ToSql()
        {
            if(string.IsNullOrEmpty(this.selectName) || this.selectName == this.columnName)
            {
                return this.selectName;
            }

            return $"{this.columnName} as {this.selectName}";
        }
    }
}
