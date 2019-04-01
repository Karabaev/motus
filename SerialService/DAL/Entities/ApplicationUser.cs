namespace SerialService.DAL.Entities
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;

    public class ApplicationUser : IdentityUser
    {
        public string Parole { get; set; }
        public string AvatarURL { get; set; }
        public string PublicName { get; set; }
        public bool IsLocked { get; set; }
        public virtual List<VideoMaterial> AddedVideoMaterials { get; set; }
        public virtual List<VideoMaterial> SubscribedVideoMaterials { get; set; }
        public virtual List<VideoMark> VideoMarks { get; set; }

        public override bool Equals(object obj)
        {
            ApplicationUser other = obj as ApplicationUser;

            if (other == null)
                return false;



            //foreach (var item in this.GetType().GetProperties())
            //{
            //    if (item.PropertyType.IsValueType)
            //    {
            //        if (item.GetValue(this) != item.GetValue(user))
            //            return false;
            //    }
            //    else
            //    {
            //        if (!item.GetValue(this).Equals(item.GetValue(user)))
            //            return false;
            //    }
            //}

            return this.UserName == other.UserName &&
				this.PublicName == other.PublicName &&
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