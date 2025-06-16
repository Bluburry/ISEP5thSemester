using System.Threading.Tasks;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Tokens
{
    public interface ITokenRepository : IRepository<Token, TokenId>
    {
        Task<Token> GetByIdAsync(TokenId id);
    }
}
