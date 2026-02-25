namespace Books.Api.CustomExceptions
{
    public class BusinessException : Exception
    {

        public BusinessException(string mensaje) : base(mensaje)
        {

        }
    }
}