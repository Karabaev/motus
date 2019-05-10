namespace Tools
{
	using System;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.IO;
	using Newtonsoft.Json;
	using NLog;

	/// <summary>
	/// Ключи параметров конфигурации.
	/// </summary>
	public enum ConfigKeys
	{
		SMTP_HOST,
		SMTP_PORT,
		SENDER_MAIL,
		SENDER_NAME,
		MAIL_LOGIN,
		MAIL_PASSWORD,
		SMTP_USE_SSL,
		MAIL_MESSAGE_BODY_ADDED_NEW_EPISODE,
		MAIL_MESSAGE_BODY_ADDED_NEW_SEASON,
		MAIL_MESSAGE_CAPTION,
		VIDEO_MATERIAL_AUTHOR_MAIL
	}

	/// <summary>
	/// Менеджер конфигурации.
	/// </summary>
	public class ConfigManager
	{
		/// <summary>
		/// Имя файла конфигурации.
		/// </summary>
		private const string ConfigFileName = "Settings.cfg";
		/// <summary>
		/// Имя файла конфигурации с параметрами по умолчанию.
		/// </summary>
		private const string DefaultConfigFileName = "Def_Settings.cfg";
		/// <summary>
		/// Параметры конфингурации по умолчанию.
		/// </summary>
		private readonly Dictionary<ConfigKeys, object> DefaultConfig = new Dictionary<ConfigKeys, object>
		{
			{ ConfigKeys.SMTP_HOST, "smtp.yandex.ru" },
			{ ConfigKeys.SMTP_PORT, 25 },
			{ ConfigKeys.SENDER_MAIL, "info@motus-cinema.com" },
			{ ConfigKeys.SENDER_NAME, "Система оповещений Motus" },
			{ ConfigKeys.MAIL_LOGIN, "info@motus-cinema.com" },
			{ ConfigKeys.MAIL_PASSWORD, "buffalo2016" },
			{ ConfigKeys.SMTP_USE_SSL, true },
			{ ConfigKeys.MAIL_MESSAGE_BODY_ADDED_NEW_EPISODE, "<p>&nbsp;&nbsp;&nbsp;&nbsp;Уважаемый {0}, вы подписаны на обновление сериала \"{1}\". Уже сегодня вы можете насладиться просмотром новой {2} серии {3} сезона в озвучке \"{4}\" у нас на портале <a href = \"motus-cinema.com\">Motus cinema</a>.</p><p><font size = \"-1\"><a href = \"\">Отписаться от обновлений</a></font></p>" },
			{ ConfigKeys.MAIL_MESSAGE_BODY_ADDED_NEW_SEASON, "<p>&nbsp;&nbsp;&nbsp;&nbsp;Уважаемый {0}, вы подписаны на обновление сериала \"{1}\". Уже сегодня вы можете насладиться просмотром нового {2} сезона в озвучке \"{3}\" у нас на портале <a href = \"motus-cinema.com\">Motus cinema</a>.</p><p><font size = \"-1\"><a href = \"\">Отписаться от обновлений</a></font></p>" },
			{ ConfigKeys.MAIL_MESSAGE_CAPTION, "Уведомление" },
			{ ConfigKeys.VIDEO_MATERIAL_AUTHOR_MAIL, "maxkarab@motus-cinema.com" }
		};
		/// <summary>
		/// Объект логгера.
		/// </summary>
		private readonly Logger logger = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// Параметры конфигурации.
		/// </summary>
		public Dictionary<ConfigKeys, object> Config { get; set; }

		/// <summary>
		/// Инициализирует объект в памяти и загружает конфигурацию из файла. В случае, если заведена некорректная информация
		/// в файле конфигурации или файл конфигурации будет не найден будет создан файл с параметрами по умолчанию.
		/// </summary>
		private ConfigManager()
		{
			this.Initialization();
		}

		private void Initialization()
		{
			string fileText = string.Empty;

			try
			{
				fileText = File.ReadAllText(ConfigManager.ConfigFileName);
			}
			catch (FileNotFoundException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Файл конфигурации {0} не найден", ConfigManager.ConfigFileName));
				this.CreateDefaultConfigFile();
				return;
			}
			catch (UnauthorizedAccessException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Не достаточно прав для открытия файла конфигурации {0}", ConfigManager.ConfigFileName));
				return;
			}
			catch (IOException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Не известная ошибка при открытии файла конфигурации {0}", ConfigManager.ConfigFileName));
				return;
			}

			try
			{
				this.Config = JsonConvert.DeserializeObject<Dictionary<ConfigKeys, object>>(fileText);
				Task.Run(() => this.logger.Info("Файл конфигурации {0} успешно прочитан и распарсен", ConfigManager.ConfigFileName));
			}
			catch (JsonException ex)
			{
				Task.Run(() => this.logger.Error(ex, "Ошибка парсинга конфигурации {0}.", fileText));
				this.CreateDefaultConfigFile();
			}
		}

		/// <summary>
		/// Создает файл конфигурации с параметрами по умолчанию.
		/// </summary>
		private void CreateDefaultConfigFile()
		{
			var newFileText = JsonConvert.SerializeObject(this.DefaultConfig);

			try
			{
				File.WriteAllText(ConfigManager.DefaultConfigFileName, newFileText);
				this.logger.Info("Файл конфигурации {0} со стандартными настройками создан", ConfigManager.DefaultConfigFileName);
			}
			catch (Exception ex1)
			{
				this.logger.Error(ex1, "Не известная ошибка при создании файла конфигурации {0} со стандартными настройками", ConfigManager.DefaultConfigFileName);
			}
		}

		/// <summary>
		/// Экземпляр объекта (синглтон).
		/// </summary>
		private static ConfigManager instance;

		/// <summary>
		/// Если не создан объект в памяти, сохдает его и возвращает.
		/// </summary>
		/// <param name="logger">Объект логгера.</param>
		public static ConfigManager GetInstance()
		{
			if (ConfigManager.instance == null)
				ConfigManager.instance = new ConfigManager();

			return ConfigManager.instance;
		}
	}
}
