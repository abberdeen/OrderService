namespace OrderService.Exceptions
{
    public class OrderUpdateException : Exception
    {
        public OrderUpdateException()
        {
        }

        public OrderUpdateException(string message)
            : base(message)
        {
        }

        public OrderUpdateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
