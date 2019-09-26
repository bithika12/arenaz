namespace ArenaZ.Api
{
    public class ApiResponseFormat<T>
    {
        public int Status;
        public string Message;
        public T Data;
    }
}