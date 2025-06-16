using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Specializations;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.ContactInformations
{
    public class ContactInformationRepository : BaseRepository<ContactInformation, ContactInformationId>, IContactInformationRepository
    {
    
        public ContactInformationRepository(HospitalDbContext context):base(context.ContactInformations)
        {
           
        }


    }
}