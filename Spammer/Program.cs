namespace Updater
{
	using System;
	using System.Threading.Tasks;
	using SerialService.DAL;
	using NLog;
	using System.Collections.Generic;
	using SerialService.Infrastructure.Exceptions;
	using Shared.Mail;

	public class Program
	{
		static int Main(string[] args)
		{

			while(true)
			{
				int switcher = -1;

				try
				{
					Console.WriteLine("1. Апдейтер 2. Даунлоадер");
					switcher = int.Parse(Console.ReadLine());

					switch (switcher)
					{
						case 1:
							Program.LaunchUpdater();
							return 1;
						case 2:
                            Console.WriteLine("1. Загрузить в интервале ID 2. Загрузить по url");
                            switch (int.Parse(Console.ReadLine()))
                            {
                                case 1:
                                    Program.LaunchRangeDownloader();
                                    break;
                                case 2:
                                    Program.LaunchUrlDownloader();
                                    Console.ReadLine();
                                    break;
                            }
                            return 2;
						case 0:
							return 0;
						default:
							return -1;
					}
				}
				catch(FormatException)
				{
					Console.WriteLine("Неверный пункт меню");
					continue;
				}
			}
		}

		private static void LaunchUpdater()
		{
			DateTime startDateTime = DateTime.Now;
			Task.Run(() => Program.Logger.Info("Апдейтер запущен"));
			ConfigManager configManager = ConfigManager.GetInstance(Program.Logger);
			MailClient mailClient = null;
			int totalCheckedFilms = -1;

			try
			{
				string host = (string)configManager.Config[ConfigKeys.SMTP_HOST];
				ushort port = (ushort)(long)configManager.Config[ConfigKeys.SMTP_PORT];
				string senderMail = (string)configManager.Config[ConfigKeys.SENDER_MAIL];
				string senderName = (string)configManager.Config[ConfigKeys.SENDER_NAME];
				string login = (string)configManager.Config[ConfigKeys.MAIL_LOGIN];
				string password = (string)configManager.Config[ConfigKeys.MAIL_PASSWORD];
				bool useSsl = (bool)configManager.Config[ConfigKeys.SMTP_USE_SSL];
				mailClient = new MailClient(host, port, senderMail, senderName, login, password, useSsl);
			}
			catch (NullReferenceException ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Файл конфигурациине не загружен"));
				return;
			}
			catch (InvalidCastException ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Один из параметров имеет неверный формат"));
				return;
			}
			catch (KeyNotFoundException ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Неверный файл конфигурации"));
				return;
			}

			try
			{
				VideoMaterialUpdater updater = new VideoMaterialUpdater(new AppUnitOfWork(), mailClient, configManager, Program.Logger);
				Task.Run(() => Program.Logger.Info("Начало проверки обновлений"));
				totalCheckedFilms = updater.CheckUpdatesOfAllVideoMaterials();
				Task.Run(() => Program.Logger.Info("Окончание проверки обновлений"));
			}
			catch (Exception ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Критическая ошибка"));
			}

			DateTime endDateTime = DateTime.Now;
			TimeSpan result = endDateTime - startDateTime;

			Task.Run(() => Program.Logger.Fatal("Фильмов было проверено: {0}", totalCheckedFilms));
			Task.Run(() => Program.Logger.Fatal("Потрачено времени на проверку обновлений: {0}", result));
		}
		
		private static void LaunchRangeDownloader()
		{
			int startID = -1;
			int endID = -1;
			ConfigManager configManager = ConfigManager.GetInstance(Program.Logger);
			string authorMail = string.Empty;

			try
			{
				authorMail = (string)configManager.Config[ConfigKeys.VIDEO_MATERIAL_AUTHOR_MAIL];
			}
			catch (NullReferenceException ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Файл конфигурациине не загружен"));
				return;
			}
			catch (InvalidCastException ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Один из параметров имеет неверный формат"));
				return;
			}
			catch (KeyNotFoundException ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Неверный файл конфигурации"));
				return;
			}
			
			try
			{
				Console.WriteLine("Введи начальное значение: ");
				startID = int.Parse(Console.ReadLine());
				Console.WriteLine("Введи последнее значение: ");
				endID = int.Parse(Console.ReadLine());
			}
			catch(FormatException)
			{
				Console.WriteLine("Неверный формат ID");
				Program.LaunchRangeDownloader();
			}

			DateTime startDateTime = DateTime.Now;
			Task.Run(() => Program.Logger.Info("Даунлоадер запущен"));
			int totalDownloaded = -1;
			VideoMaterialDownloader downloader = VideoMaterialDownloader.GetInstance(new AppUnitOfWork(), Program.Logger);

			try
			{
				totalDownloaded = downloader.DownloadAllVideoMaterials(startID, endID, authorMail);
			}
			catch(EntryNotFoundException ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Ошибка при загрузке"));
			}
			catch (Exception ex)
			{
				Task.Run(() => Program.Logger.Error(ex, "Ошибка при загрузке"));
			}

			DateTime endDateTime = DateTime.Now;
			TimeSpan result = endDateTime - startDateTime;
			Task.Run(() => Program.Logger.Fatal("Фильмов было загружено: {0}", totalDownloaded));
			Task.Run(() => Program.Logger.Fatal("Потрачено времени на проверку обновлений: {0}", result));
		}

        private static void LaunchUrlDownloader()
        {
            ConfigManager configManager = ConfigManager.GetInstance(Program.Logger);
            string authorMail = string.Empty;
            try
            {
                authorMail = (string)configManager.Config[ConfigKeys.VIDEO_MATERIAL_AUTHOR_MAIL];
            }
            catch (NullReferenceException ex)
            {
                Task.Run(() => Program.Logger.Error(ex, "Файл конфигурациине не загружен"));
                return;
            }
            catch (InvalidCastException ex)
            {
                Task.Run(() => Program.Logger.Error(ex, "Один из параметров имеет неверный формат"));
                return;
            }
            catch (KeyNotFoundException ex)
            {
                Task.Run(() => Program.Logger.Error(ex, "Неверный файл конфигурации"));
                return;
            }

            const string selector = " js-film-list-item";
            string url;

            Console.WriteLine("Введи url списка фильмов:");

            url = Console.ReadLine();
            VideoMaterialDownloader downloader = VideoMaterialDownloader.GetInstance(new AppUnitOfWork(), Program.Logger);

            try
            {
                downloader.DownloadListByUrl(url, selector, authorMail);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LaunchUrlDownloader();
            }
        }

        private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
	}
}
