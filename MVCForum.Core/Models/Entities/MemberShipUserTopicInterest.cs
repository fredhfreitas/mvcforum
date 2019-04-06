namespace MvcForum.Core.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Interfaces;
    using Utilities;
   
    public partial class MembershipUser_Topic_Interest : ExtendedDataEntity
    {
        public Guid IdTopic { get; set; }
       
        public Guid IdUser { get; set; }
    }
}