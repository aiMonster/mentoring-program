namespace MentoringProgram.Common.Models
{
    public class ResponseResult<T>
    {
        public T Data { get; set; }

        public string Erorr { get; set; }
        public bool IsSuccess { get; set; }

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
