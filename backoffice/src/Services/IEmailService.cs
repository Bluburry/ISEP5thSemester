using System.Threading.Tasks;
using DDDSample1.Domain.Tokens;

public interface IEmailService
{
    static abstract Task SendActivationEmail(string emailAddress, TokenDto pathAuthToken);
}