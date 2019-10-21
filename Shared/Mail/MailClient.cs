namespace Shared.Mail
{
	using System;
	using System.Threading.Tasks;
	using System.Net.Mail;
	using System.Net;
    using System.Collections.Generic;
	using NLog;

	/// <summary>
	/// Класс для работы с почтой.
	/// </summary>
	public class MailClient : IMailClient
    {
		/// <summary>
		/// Инициализирует объект в памяти.
		/// </summary>
		/// <param name="host">Адрес SMTP сервера.</param>
		/// <param name="port">Порт SMTP сервера.</param>
		/// <param name="senderMail">Исходящий почтовый ящик.</param>
		/// <param name="publicName">Имя для отображения.</param>
		/// <param name="login">Логин на почтовом сервере.</param>
		/// <param name="password">Пароль на почтовом сервере.</param>
		/// <param name="useSSL">Использовать SSL.</param>
		/// <param name="logger">Объект логгера.</param>
		public MailClient(string host, ushort port, string senderMail, string publicName, string login, string password, bool useSSL)
		{
			this.logger = LogManager.GetCurrentClassLogger();

			try
			{
				this.smtpClient = new SmtpClient(host, port);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Task.Run(() => this.logger.Error(string.Format("Не удалось создать объект SMTP клиента с указанным адресом сервера {0} и портом {1}.", host, port), ex));
			}

			this.smtpClient.Credentials = new NetworkCredential(login, password);
			this.smtpClient.EnableSsl = useSSL;

			try
			{
				this.senderMail = new MailAddress(senderMail, publicName);
			}
			catch (FormatException ex)
			{
				Task.Run(() => this.logger.Error(string.Format("Неверный формат почты отправителя: {0}.", senderMail), ex));
			}


		}

		/// <summary>
		/// Отправить сообщение.
		/// </summary>
		/// <param name="recipientMail">Почтовый ящик получателя.</param>
		/// <param name="header">Заголовок сообщения.</param>
		/// <param name="message">Тело сообщения.</param>
		/// <param name="isMessageHtml">Определять собщение как HTML-разметку.</param>
		public void SendMessage(string recipientMail, string header, string message, bool isMessageHtml)
		{
			MailMessage mailMessage = new MailMessage(senderMail, new MailAddress(recipientMail));
			mailMessage.Subject = header;
			mailMessage.Body = message;
			mailMessage.IsBodyHtml = isMessageHtml;

			try
			{
				this.smtpClient.Send(mailMessage);
			}
			catch (SmtpFailedRecipientsException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", recipientMail));
			}
			catch (SmtpException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", recipientMail));
			}
		}

		/// <summary>
		/// Отправить сообщение, асинхронная версия.
		/// </summary>
		/// <param name="recipientMail">Почтовый ящик получателя.</param>
		/// <param name="header">Заголовок сообщения.</param>
		/// <param name="message">Тело сообщения.</param>
		/// <param name="isMessageHtml">Определять собщение как HTML-разметку.</param>
		public void SendMessageAsync(string recipientMail, string header, string message, bool isMessageHtml)
		{
			MailMessage mailMessage = new MailMessage(senderMail, new MailAddress(recipientMail));
			mailMessage.Subject = header;
			mailMessage.Body = message;
			mailMessage.IsBodyHtml = isMessageHtml;

			try
			{
				this.smtpClient.SendAsync(mailMessage, null);
			}
			catch (SmtpFailedRecipientsException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", recipientMail));
			}
			catch (SmtpException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", recipientMail));
			}
		}

        #region INotificationClient

        public void SendMessage(string destination, string caption, string message)
        { 
            MailMessage mailMessage = new MailMessage(senderMail, new MailAddress(destination));
            mailMessage.Subject = caption;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            try
            {
                this.smtpClient.Send(mailMessage);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", destination));
            }
            catch (SmtpException ex)
            {
                Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", destination));
            }
        }

        public async Task SendMessageAsync(string destination, string caption, string message)
        {
            MailMessage mailMessage = new MailMessage(senderMail, new MailAddress(destination));
            mailMessage.Subject = caption;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            try
            {
                this.smtpClient.SendAsync(mailMessage, null);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                await Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", destination));
            }
            catch (SmtpException ex)
            {
                await Task.Run(() => this.logger.Error(ex, "Не удалось отправить сообщение на электронную почту {0}", destination));
            }
        }

        public void SendMessageToManyDestinations(IEnumerable<string> destinations, string caption, string message)
        {
            string destinationsStr = string.Join(" ", destinations);
            MailMessage mailMessage = new MailMessage(senderMail, new MailAddress(destinationsStr));
            mailMessage.Subject = caption;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            try
            {
                this.smtpClient.Send(mailMessage);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Task.Run(() => this.logger.Error(ex, $"Не удалось отправить сообщение на электронные почты {destinationsStr}"));
            }
            catch (SmtpException ex)
            {
                Task.Run(() => this.logger.Error(ex, $"Не удалось отправить сообщение на электронные почты {destinationsStr}"));
            }
        }

        public async Task SendMessageToManyDestinationsAsync(IEnumerable<string> destinations, string caption, string message)
        {
            string destinationsStr = string.Join(" ", destinations);
            MailMessage mailMessage = new MailMessage(senderMail, new MailAddress(destinationsStr));
            mailMessage.Subject = caption;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            try
            {
                this.smtpClient.SendAsync(mailMessage, null);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                await Task.Run(() => this.logger.Error(ex, $"Не удалось отправить сообщение на электронные почты {destinationsStr}"));
            }
            catch (SmtpException ex)
            {
                await Task.Run(() => this.logger.Error(ex, $"Не удалось отправить сообщение на электронные почты {destinationsStr}"));
            }
        }

        #endregion

        private readonly MailAddress senderMail;
		private readonly SmtpClient smtpClient;
		private readonly Logger logger;
    }
}
