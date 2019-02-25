namespace MvcForum.Web.ViewModels.ExtensionMethods
{
    using Core.Models.Entities;
    using Member;

    /// <summary>
    /// Extension methods for the member edit view model
    /// </summary>
    public static partial class MemberFrontEndEditViewModelExtensions
    {
        /// <summary>
        /// Creates a MembershipUser from the view model
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static MembershipUser ToMembershipUser(this MemberFrontEndEditViewModel viewModel, MembershipUser user)
        {
            user.Id = viewModel.Id;
            user.UserName = viewModel.UserName;
            user.Email = viewModel.Email;
            user.Signature = viewModel.Signature;
            user.Age = viewModel.Age;
            user.Location = viewModel.Location;
            user.Website = viewModel.Website;
            user.Twitter = viewModel.Twitter;
            user.Facebook = viewModel.Facebook;
            if (!string.IsNullOrWhiteSpace(viewModel.Avatar))
            {
                user.Avatar = viewModel.Avatar;
            }           
            user.DisableEmailNotifications = viewModel.DisableEmailNotifications;   
            



            return user;
        }

        /// <summary>
        ///     Creates view model
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static MemberFrontEndEditViewModel PopulateMemberViewModel(this MembershipUser user)
        {
            var viewModel = new MemberFrontEndEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Signature = user.Signature,
                Age = user.Age,
                Location = user.Location,
                Website = user.Website,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                DisableFileUploads = user.DisableFileUploads == true,
                Avatar = user.Avatar,
                DisableEmailNotifications = user.DisableEmailNotifications == true,
                AmountOfPoints = user.TotalPoints,
                Estado = user.Estado,
                Cidade = user.Cidade,
                                
                FuncaoEmpresario = user.FuncaoEmpresario,
                FuncaoOperador = user.FuncaoOperador,
                FuncaoMecanico = user.FuncaoMecanico,
                FuncaoOutro = user.FuncaoOutro,


                Marca1 = user.Marca1,
                Modelo1 = user.Modelo1,
                AnoFabricacao1 = user.AnoFabricacao1,
                Empresa1 = user.Empresa1,

                Marca2 = user.Marca2,
                Modelo2 = user.Modelo2,
                AnoFabricacao2 = user.AnoFabricacao2,
                Empresa2 = user.Empresa2,

                Marca3 = user.Marca3,
                Modelo3 = user.Modelo3,
                AnoFabricacao3 = user.AnoFabricacao3,
                Empresa3 = user.Empresa3,

                Marca4 = user.Marca4,
                Modelo4 = user.Modelo4,
                AnoFabricacao4 = user.AnoFabricacao4,
                Empresa4 = user.Empresa4,

                Marca5 = user.Marca5,
                Modelo5 = user.Modelo5,
                AnoFabricacao5 = user.AnoFabricacao5,
                Empresa5 = user.Empresa5,
            };
            return viewModel;
        }
    }
}