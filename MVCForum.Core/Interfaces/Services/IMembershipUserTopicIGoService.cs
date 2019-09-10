using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Models.Entities;
using MvcForum.Core.Services;

namespace MvcForum.Core.Interfaces.Services
{
    public interface IMembershipUserTopicIGoService
    {
        MemberShipUserTopicIGo Add(MemberShipUserTopicIGo usuarioTopicIGo);
        void Delete(MemberShipUserTopicIGo item);
        List<MemberShipUserTopicIGo> GetAll();
        List<MemberShipUserTopicIGo> GetByObject(MemberShipUserTopicIGo item);
        List<MemberShipUserTopicIGo> GetByTopic(Guid id);
        List<MemberShipUserTopicIGo> GetByUser(Guid id);
        void RefreshContext(IMvcForumContext context);
        Task<int> SaveChanges();
    }
}