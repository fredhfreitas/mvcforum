using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Models.Entities;

namespace MvcForum.Core.Interfaces.Services
{
    public interface IMembershipUserTopicInterestService
    {
        MembershipUserTopicInterest Add(MembershipUserTopicInterest usuarioTopicInterest);
        void Delete(MembershipUserTopicInterest item);
        List<MembershipUserTopicInterest> GetAll();
        List<MembershipUserTopicInterest> GetByObject(MembershipUserTopicInterest item);
        List<MembershipUserTopicInterest> GetByTopic(Guid id);
        List<MembershipUserTopicInterest> GetByUser(Guid id);
        void RefreshContext(IMvcForumContext context);
        Task<int> SaveChanges();
    }
}