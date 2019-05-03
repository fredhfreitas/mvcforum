namespace MvcForum.Web.ViewModels.Topic
{
    using Microsoft.Ajax.Utilities;
    using MvcForum.Core.Interfaces.Services;
    using System.Collections.Generic;

    public class ActiveTopicsViewModel
    {
        public List<TopicViewModel> Topics { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public List<EventoDto> DatasEvento { get; set; }
    }
}