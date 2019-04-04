namespace SerialService.Infrastructure.Helpers
{
    using System;
    using System.Text;
    using System.Security.Cryptography;
    using Infrastructure;
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using NLog;

    public static class UserTokenGenerator
    {
        private static string GetJsonUserInfoInBase64(string id = "", string name = "", string email = "", string avatarUrl = "")
        {
			string json = string.Empty;

			if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
			{
				json = "{}";
			}
			else
			{
				json = JsonConvert.SerializeObject(new { id, name, email, avatar = avatarUrl });
			}

			UserTokenGenerator.logger.Info("Json параметро пользователя: {0}", json);
			byte[] encodeBytes = Encoding.UTF8.GetBytes(json.ToString());
            string result = Convert.ToBase64String(encodeBytes);
            return result;
        }

        private static string GetRequestMD5Sign(string userInBase64, string siteApiKey, long timeStamp)
        {
			using (MD5 md5 = MD5.Create())
			{
				byte[] decodeBytes = Encoding.UTF8.GetBytes(userInBase64 + siteApiKey + timeStamp);
				byte[] hashBytes = md5.ComputeHash(decodeBytes);
				StringBuilder result = new StringBuilder();

				foreach (var item in hashBytes)
					result.Append(item.ToString("x2"));

				return result.ToString();
			}
        }

        private static long GetCurrentDateTimeInMiliseconds()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static string GetUserSsoToken(string siteApiKey, string id = "", string name = "", string email = "", string avatarUrl = "")
        {
            string user = UserTokenGenerator.GetJsonUserInfoInBase64(id, name, email, avatarUrl);
			UserTokenGenerator.logger.Info("Json параметры пользователя в base64: {0}", user);
			long timeStamp = UserTokenGenerator.GetCurrentDateTimeInMiliseconds();
			UserTokenGenerator.logger.Info("Время запроса в милисекундах: {0}", timeStamp);
			string reqSign = UserTokenGenerator.GetRequestMD5Sign(user, siteApiKey, timeStamp);
			UserTokenGenerator.logger.Info("MD5 подпись запроса: {0}", reqSign);
			string result = string.Format("{0} {1} {2}", user, reqSign, timeStamp);
			UserTokenGenerator.logger.Info("SSO auth Token: {0}", result);
			return result;
        }

		private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}