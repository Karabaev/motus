namespace SerialService.DAL.Entities
{
	using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;

    public class ApplicationUser : IdentityUser
    {
		public DateTime RegisterDateTime { get; set; }
		public DateTime ChangeDateTime { get; set; }
		public DateTime LastAuthorizationDateTime { get; set; }
		public string Parole { get; set; }
        public string AvatarURL { get; set; }
        public bool IsLocked { get; set; }
		/// <summary>
		/// Необходим для хранения уникального ключа, сгенерированного для изменения эл. почты (пока только для этого).
		/// </summary>
		public string LastConfirmationKey { get; set; } 
        public virtual List<VideoMaterial> AddedVideoMaterials { get; set; }
        public virtual List<VideoMaterial> SubscribedVideoMaterials { get; set; }
        public virtual List<VideoMark> VideoMarks { get; set; }
        public virtual List<VideoMaterialViewsByUsers> VideoMaterialsViews { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<UserParam> Params { get; set; }

        public override bool Equals(object obj)
        {
            ApplicationUser other = obj as ApplicationUser;

            if (other == null)
                return false;

            return this.UserName == other.UserName &&
				this.Email == other.Email &&
				this.Parole == other.Parole &&
				this.PasswordHash == other.PasswordHash &&
				this.IsLocked == other.IsLocked;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }

    }
}