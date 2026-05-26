namespace TravelPlanner.Common
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }

        public static ServiceResponse<T> Ok(T data) => new() { Success = true, Data = data };
        public static ServiceResponse<T> Fail(string error) => new() { Success = false, Error = error };
    }
}