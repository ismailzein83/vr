using System;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public static class Helper
    {
        public static bool CheckIfPasswordExpired(User user, DateTime passwordChangeTime, out int? passwordExpirationDaysLeft)
        {
            passwordExpirationDaysLeft = null;
            ConfigManager configManager = new ConfigManager();
            if (user.Settings == null || !user.Settings.EnablePasswordExpiration)
                return false;

            int? age = configManager.GetPasswordAgeInDays();
            int? exparitionDaysToNotify = configManager.GetPasswordExpirationDaysToNotify();

            if (age.HasValue)
            {
                int settingsPasswordAge = age.Value;
                int passwordAge = (int)((DateTime.Now - passwordChangeTime).TotalDays);
                int totalDaysToExpirePassword = settingsPasswordAge - passwordAge;
                int daysToNotify = exparitionDaysToNotify.HasValue ? exparitionDaysToNotify.Value : 0;
                if (totalDaysToExpirePassword <= daysToNotify)
                {
                    passwordExpirationDaysLeft = totalDaysToExpirePassword;
                }
                return passwordAge >= settingsPasswordAge;
            }
            return false;
        }
    }
}
