using System;
using DataAccess;

namespace Api.DataAccess.Models
{
    public class StorageFile : EntityBase
    {
        public Guid StorageId { get; set; }

        /// <summary>
        /// Публичное имя файла, с которым он был загружен в систему.
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Внутреннее имя, с которым файл был сохранен в хранилище.
        /// </summary>
        public string StoredFileName { get; set; }

        public long FileSizeInBytes { get; set; }
    }
}
