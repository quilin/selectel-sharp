﻿using Newtonsoft.Json;
using SelectelSharpCore.Headers;
using System;

namespace SelectelSharpCore.Models.File
{
    public class FileInfo
    {
        /// <summary>
        /// Имя файла
        /// </summary>        
        public string Name { get; set; }

        /// <summary>
        /// Размер файла
        /// </summary>
        [Header(HeaderKeys.ContentLength)]
        public long Bytes { get; set; }

        /// <summary>
        /// Тип файла
        /// </summary>
        [JsonProperty("content_type")]
        [Header(HeaderKeys.ContentType)]
        public string ContentType { get; set; }

        /// <summary>
        /// MD5 Hash
        /// </summary>
        [Header(HeaderKeys.ETag)]
        public string Hash { get; set; }

        /// <summary>
        /// Время изменения файла
        /// </summary>
        [JsonProperty("last_modified")]
        [Header(HeaderKeys.LastModified)]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Время создания файла
        /// </summary>
        [Header(HeaderKeys.Date)]
        public DateTime Date { get; set; }
    }
}