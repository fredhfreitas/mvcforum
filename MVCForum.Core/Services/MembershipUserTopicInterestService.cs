namespace MvcForum.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Constants;
    using Interfaces;
    using Interfaces.Services;
    using Models.Entities;
    using Utilities;


    class MembershipUserTopicInterestService
    {
        private readonly ICacheService _cacheService;
        private IMvcForumContext _context;

        public MembershipUserTopicInterestService(IMvcForumContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context;
        }

        /// <inheritdoc />
        public void RefreshContext(IMvcForumContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public List<MembershipUserTopicInterest> GetAll()
        {
            return _context.MembershipUserTopicInterest.ToList();
        }

        /// <inheritdoc />
        public List<MembershipUserTopicInterest> GetByObject(MembershipUserTopicInterest item)
        {
            return _context.MembershipUserTopicInterest
                .Where(x => x.IdUser == item.IdUser)
                .Where(x => x.IdTopic == item.IdTopic).ToList();
        }

        /// <inheritdoc />
        public List<MembershipUserTopicInterest> GetByUser(Guid id)
        {
            return _context.MembershipUserTopicInterest.Where(x => x.IdUser == id).ToList();
        }

        /// <inheritdoc />
        public List<MembershipUserTopicInterest> GetByTopic(Guid id)
        {
            return _context.MembershipUserTopicInterest.Where(x => x.IdTopic == id).ToList();
        }

        /// <inheritdoc />
        public MembershipUserTopicInterest Add(MembershipUserTopicInterest usuarioTopicInterest)
        {
            return _context.MembershipUserTopicInterest.Add(usuarioTopicInterest);
        }

        /// <inheritdoc />
        public void Delete(MembershipUserTopicInterest item)
        {
            _context.MembershipUserTopicInterest.Remove(item);
        }
    }
}
