using System.Collections.Generic;

namespace ORM
{
    public class Query
    {
        public string Table;
        public int Take = -1;
        public int Skip;
        public IList<Order> Orders = new List<Order>();
        public IList<Where> Wheres = new List<Where>();
        public IList<Select> Selects = new List<Select>();
    }
}
