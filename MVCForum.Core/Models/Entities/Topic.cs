namespace MvcForum.Core.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enums;
    using Interfaces;
    using Utilities;

    public partial class Topic : ExtendedDataEntity, IBaseEntity
    {
        public Topic()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Solved { get; set; }
        public bool? SolvedReminderSent { get; set; }
        public string Slug { get; set; }
        public int Views { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }

        public virtual Post LastPost { get; set; }
        public virtual Category Category { get; set; }
        public virtual IList<Post> Posts { get; set; }
        public virtual IList<TopicTag> Tags { get; set; }
        public virtual MembershipUser User { get; set; }
        public virtual IList<TopicNotification> TopicNotifications { get; set; }
        public virtual IList<Favourite> Favourites { get; set; }
        public virtual Poll Poll { get; set; }
        public bool? Pending { get; set; }
        public string NiceUrl => UrlTypes.GenerateUrl(UrlType.Topic, Slug);

        public int VoteCount
        {
            get { return Posts.Select(x => x.VoteCount).Sum(); }
        }

        public bool? IsCategoryNew { get; set; }
        public bool? IsCategoryUsed { get; set; }
        public bool? IsCategoryExchange { get; set; }
        public bool? IsAnuncio { get; set; }
        public Decimal? Price { get; set; }

        public string TipoAnuncio { get; set; }
        public bool? IsMecanico { get; set; }
        public bool? IsInstrutor { get; set; }
        public bool? IsOperador { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string TelefoneUsuario { get; set; }
        public string TelefoneWhatsApp { get; set; }
        public string CidadeUsuario { get; set; }
        public string EstadoUsuario { get; set; }
        public string Observacoes { get; set; }
        public string Imagem { get; set; }

        public string NomeCategoriaAnuncio
        {
            get
            {
                if (IsCategoryNew.HasValue && IsCategoryNew.Value)
                    return "NOVO";

                if (IsCategoryUsed.HasValue && IsCategoryUsed.Value)
                    return "USADO";

                if (IsCategoryExchange.HasValue && IsCategoryExchange.Value)
                    return "TROCA";

                return string.Empty;
            }
        }

        public short CodigoCategoriaAnuncio
        {
            get
            {
                if (IsCategoryNew.HasValue && IsCategoryNew.Value)
                    return 0;

                if (IsCategoryUsed.HasValue && IsCategoryUsed.Value)
                    return 1;

                if (IsCategoryExchange.HasValue && IsCategoryExchange.Value)
                    return 2;

                return -1;
            }
        }
    }
}
