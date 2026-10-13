namespace NeonClub.Shared.Constracts.Interfaces
{
    public interface IDTO
    {
        (bool IsValid, string Message) Validate();
    }
}
