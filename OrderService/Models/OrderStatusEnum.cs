namespace OrderService.Models
{
    public enum OrderStatusEnum
    {
        /// <summary>
        /// Зарегистрирован
        /// </summary>
        Registered = 1,

        /// <summary>
        /// Сформирован
        /// </summary>
        Formed = 2,

        /// <summary>
        /// Выполнен
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Отменен
        /// </summary>
        Canceled = 4
    }
}