namespace OrderService.Exceptions
{
    public class OrderCreateException : Exception
    {
        public OrderCreateException()
        {
        }

        public OrderCreateException(string message)
            : base(message)
        {
        }

        public OrderCreateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
