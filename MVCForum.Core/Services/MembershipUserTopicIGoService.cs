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


    class MembershipUserTopicIGoService : IMembershipUserTopicIGoService
    {
        private readonly ICacheService _cacheService;
        private IMvcForumContext _context;

        public MembershipUserTopicIGoService(IMvcForumContext context, ICacheService cacheService)
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
        public List<MemberShipUserTopicIGo> GetAll()
        {
            return _context.MemberShipUserTopicIGo.ToList();
        }

        /// <inheritdoc />
        public List<MemberShipUserTopicIGo> GetByObject(MemberShipUserTopicIGo item)
        {
            return _context.MemberShipUserTopicIGo
                .Where(x => x.IdUser == item.IdUser)
                .Where(x => x.IdTopic == item.IdTopic).ToList();
        }

        /// <inheritdoc />
        public List<MemberShipUserTopicIGo> GetByUser(Guid id)
        {
            return _context.MemberShipUserTopicIGo.Where(x => x.IdUser == id).ToList();
        }

        /// <inheritdoc />
        public List<MemberShipUserTopicIGo> GetByTopic(Guid id)
        {
            return _context.MemberShipUserTopicIGo.Where(x => x.IdTopic == id).ToList();
        }

        /// <inheritdoc />
        public MemberShipUserTopicIGo Add(MemberShipUserTopicIGo usuarioTopicIGo)
        {
            return _context.MemberShipUserTopicIGo.Add(usuarioTopicIGo);
        }

        /// <inheritdoc />
        public void Delete(MemberShipUserTopicIGo item)
        {
            _context.MemberShipUserTopicIGo.Remove(item);
        }
    }
}
