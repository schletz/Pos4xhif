using StoreManager.Application.Model;

namespace StoreManager.Application.Infrastructure.Repositories
{
    public class UserRepository : Repository<User, int>
    {
        private readonly ICryptService _cryptService;
        public UserRepository(StoreContext db, ICryptService cryptService) : base(db)
        {
            _cryptService = cryptService;
        }
    }

}
