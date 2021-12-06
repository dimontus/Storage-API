using System;

namespace Api.Storage.Models
{
    public class CreateStorageRequest
    {
        /// <summary>
        /// Id проекта, для которого производится импорт, опционально
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Время, на которое создается хранилище, по умолчанию - 1 день
        /// </summary>
        public int TTLInSeconds { get; set; } = (int)TimeSpan.FromDays(1).TotalSeconds;

        /// <summary>
        /// Имя хранилища, опционально
        /// </summary>
        public string Name { get; set; }
    }
}
