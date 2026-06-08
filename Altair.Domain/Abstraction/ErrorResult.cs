namespace Altair.Domain.Abstraction;

public class ErrorResult
{
    public ErrorResult(string error)
    {
        Error = error;
    }

    public ErrorResult(string error, int status)
    {
        Status = status;
        Error = error;
    }

    public int Status { get; private set; } = 400;
    public string Title { get; private set; } = "One or more validation errors occurred.";
    public string Error { get; set; }
    public Guid TraceId { get; private set; } = Guid.NewGuid();
}
