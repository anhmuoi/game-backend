namespace ERPSystem.DataModel.Response;

public class ResponseStatus
{
    public string Message { get; set; }
    public bool StatusCode { get; set; }
    public object Data { get; set; }
}