namespace MvcForum.Web.ViewModels.ExtensionMethods
{
    using Core.Constants;
    using Core.ExtensionMethods;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using Member;

    public static partial class MemberAddViewModelExtensions
    {
        /// <summary>
        /// Converts a add view model to a membershipuser
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static MembershipUser ToMembershipUser(this MemberAddViewModel viewModel)
        {
            var userToSave = new MembershipUser
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                Password = viewModel.Password,
                IsApproved = viewModel.IsApproved,
                Comment = viewModel.Comment,
                Estado = viewModel.Estado,
                Cidade = viewModel.Cidade,
                FuncaoEmpresario = viewModel.FuncaoEmpresario,
                FuncaoOperador = viewModel.FuncaoOperador,
                FuncaoMecanico = viewModel.FuncaoMecanico,
                FuncaoOutro = viewModel.FuncaoOutro,
                Marca1 = viewModel.Marca1,
                Modelo1 = viewModel.Modelo1,
                AnoFabricacao1 = viewModel.AnoFabricacao1,
                Empresa1 = viewModel.Empresa1,
                Avatar = viewModel.Files[0].FileName,
                Marca2 = viewModel.Marca2,
                Modelo2 = viewModel.Modelo2,
                AnoFabricacao2 = viewModel.AnoFabricacao2,
                Empresa2 = viewModel.Empresa2,

                Marca3 = viewModel.Marca3,
                Modelo3 = viewModel.Modelo3,
                AnoFabricacao3 = viewModel.AnoFabricacao3,
                Empresa3 = viewModel.Empresa3,

                Marca4 = viewModel.Marca4,
                Modelo4 = viewModel.Modelo4,
                AnoFabricacao4 = viewModel.AnoFabricacao4,
                Empresa4 = viewModel.Empresa4,

                Marca5 = viewModel.Marca5,
                Modelo5 = viewModel.Modelo5,
                AnoFabricacao5 = viewModel.AnoFabricacao5,
                Empresa5 = viewModel.Empresa5
            };           

            if (viewModel.LoginType == LoginType.Facebook)
            {
                userToSave.FacebookAccessToken = viewModel.UserAccessToken;
            }
            if (viewModel.LoginType == LoginType.Google)
            {
                userToSave.GoogleAccessToken = viewModel.UserAccessToken;
            }
            if (viewModel.LoginType == LoginType.Microsoft)
            {
                userToSave.MicrosoftAccessToken = viewModel.UserAccessToken;
            }

            // Save the social url
            if (!string.IsNullOrWhiteSpace(viewModel.SocialProfileImageUrl))
            {
                // Save the SocialProfileImageUrl in ExtendedData as we'll need it
                userToSave.SetExtendedDataValue(Constants.ExtendedDataKeys.SocialProfileImageUrl, viewModel.SocialProfileImageUrl);
            }

            // Save the return url on the user to
            if (!string.IsNullOrWhiteSpace(viewModel.ReturnUrl))
            {
                userToSave.SetExtendedDataValue(Constants.ExtendedDataKeys.ReturnUrl, viewModel.ReturnUrl);
            }

            if (!string.IsNullOrWhiteSpace(viewModel.Avatar))
            {
                userToSave.Avatar = viewModel.Avatar;
            }

            return userToSave;
        }
    }
}