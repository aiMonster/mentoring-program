namespace MentoringProgram.Common.Models
{
    public class ResponseResult<T>
    {
        public T Data { get; }

        public string Erorr { get;}
        public bool IsSuccess { get; }

        public ResponseResult(T data)
        {
            Data = data;
            IsSuccess = true;
        }

        public ResponseResult(string error)
        {
            Erorr = error;
        }
    }
}
