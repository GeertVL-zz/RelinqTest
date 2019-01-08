namespace RelinqTest2
{
    public class CommandData
    {
        public CommandData(QueryPartsAggregator queryParts)
        {
            QueryParts = queryParts;
        }

        public QueryPartsAggregator QueryParts { get; set; }

        public DGraphQuery CreateQuery()
        {
            return new DGraphQuery(QueryParts);
        }
    }
}