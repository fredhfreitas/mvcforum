namespace MvcForum.Core.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Interfaces;
    using Utilities;
   
    public partial class MemberShipUserTopicIGo : ExtendedDataEntity
    {
        public MemberShipUserTopicIGo()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid IdTopic { get; set; }
       
        public Guid IdUser { get; set; }
    }
}