using DDDSample1.Domain.Tokens;
using DDDSample1.Infrastructure.Shared;


namespace DDDSample1.Infrastructure.Tokens
{
    public class TokenRepository : BaseRepository<Token, TokenId>, ITokenRepository
    {
        public TokenRepository(HospitalDbContext context) : base(context.Tokens)
        {
        }
    }
}