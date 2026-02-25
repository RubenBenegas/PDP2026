namespace Books.Api.CustomExceptions
{
    public class TransactionException : Exception
    {

        public TransactionException(string mensaje) : base(mensaje)
        {

        }
    }
}