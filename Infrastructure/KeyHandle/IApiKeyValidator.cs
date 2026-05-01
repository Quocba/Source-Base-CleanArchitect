namespace Domain.KeyHandle
{
    public interface IApiKeyValidator
    {
        bool IsValid(string key);

    }
}
