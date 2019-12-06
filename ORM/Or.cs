namespace ORM
{
    public class Or : And
    {
        public Or() : base(" OR ")
        {
        }

        public new static And Where(params Where[] wheres)
        {
            var or = new Or();
            or.Wheres.AddRange(wheres);
            return or;
        }

    }
}
