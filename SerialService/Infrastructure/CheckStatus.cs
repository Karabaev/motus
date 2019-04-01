namespace SerialService.Infrastructure
{
    /// <summary>
    /// Статус проверки записи.
    /// </summary>
    public enum CheckStatus
    {
        /// <summary>
        /// Запись находтся на стадии проверки модератором
        /// </summary>
        Checking = 0,
        /// <summary>
        /// Запись проверена и может быть доступна пользователю
        /// </summary>
        Confirmed = 1,
        /// <summary>
        /// Запись не прошла проверку, требует корректировки данных.
        /// </summary>
        Rejected = 2
    }
}