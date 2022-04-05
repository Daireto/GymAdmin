namespace GymAdmin.Common
{
    //Manages the connection operations responses
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
}
