namespace ORM
{
    public class Order
    {
        public string Column;

        public enum Dir
        {
            Ascending,
            Descending
        };

        public Dir Direction = Dir.Ascending;

        public string ToSql()
        {
            return $"{this.Column} {(this.Direction == Dir.Ascending ? "ASC" : "DESC")}";
        }
    }
}
