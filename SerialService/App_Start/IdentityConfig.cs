namespace SerialService
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using DAL.Entities;
    using System.Net.Mail;
    using DAL.Repository;
	using Shared.Mail;

	public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
			MailClient mailClient = new MailClient("smtp.yandex.ru", 25, "info@motus-cinema.com", "Motus cinema", "info@motus-cinema.com", "buffalo2016", true);
			return Task.Run(() => mailClient.SendMessage(message.Destination, message.Subject, message.Body, true));
		}

		public void Send(IdentityMessage message)
		{
			MailClient mailClient = new MailClient("smtp.yandex.ru", 25, "info@motus-cinema.com", "Motus cinema", "info@motus-cinema.com", "buffalo2016", true);
			mailClient.SendMessage(message.Destination, message.Subject, message.Body, true);
		}
	}

    // Настройка диспетчера входа для приложения.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
