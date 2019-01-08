namespace RelinqTest2
{
    public class DGraphQueryFactory
    {
        public static DGraphQueryable<T> Queryable<T>()
        {
            return new DGraphQueryable<T>();
        }
    }
}