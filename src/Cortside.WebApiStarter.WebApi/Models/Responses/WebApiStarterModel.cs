using System;

namespace Cortside.WebApiStarter.WebApi.Models.Responses {

    /// <summary>
    /// Represents a single loan
    /// </summary>
    public class WebApiStarterModel {
        /// <summary>
        /// Unique identifier for a WebApiStarter
        /// </summary>
        public Guid WebApiStarterId { get; set; }

        /// <summary>
        /// WebApiStarter type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Create Date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Create Subject
        /// </summary>
        public SubjectModel CreatedSubject { get; set; }

        /// <summary>
        /// LastModifiedDate
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// LastModifiedSubject
        /// </summary>
        public SubjectModel LastModifiedSubject { get; set; }

        /// <summary>
        /// WebApiStarter filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// WebApiStarter file hash
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// WebApiStarter file size
        /// </summary>
        public long Size { get; set; }


        /// <summary>
        /// WebApiStarter set id for application
        /// </summary>
        public long? WebApiStarteretId { get; set; }

        /// <summary>
        /// Date WebApiStarter were uploaded
        /// </summary>
        public DateTime? WebApiStarterUploadDate { get; set; }

        /// <summary>
        /// Date contractor printed WebApiStarter
        /// </summary>
        public DateTime? ContractorPrintedDate { get; set; }
    }
}
