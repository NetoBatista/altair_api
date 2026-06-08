namespace Altair.Domain.Abstraction;

public class UseCaseException : Exception
{
    public UseCaseException()
    {

    }
    public UseCaseException(string message) : base(message)
    {

    }
}
