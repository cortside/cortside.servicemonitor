using System;

namespace Cortside.ServiceMonitor.WebApi.Models.Responses {

    /// <summary>
    /// Represents a single loan
    /// </summary>
    public class ServiceMonitorModel {
        /// <summary>
        /// Unique identifier for a ServiceMonitor
        /// </summary>
        public Guid ServiceMonitorId { get; set; }

        /// <summary>
        /// ServiceMonitor type
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
        /// ServiceMonitor filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// ServiceMonitor file hash
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// ServiceMonitor file size
        /// </summary>
        public long Size { get; set; }


        /// <summary>
        /// ServiceMonitor set id for application
        /// </summary>
        public long? ServiceMonitoretId { get; set; }

        /// <summary>
        /// Date ServiceMonitor were uploaded
        /// </summary>
        public DateTime? ServiceMonitorUploadDate { get; set; }

        /// <summary>
        /// Date contractor printed ServiceMonitor
        /// </summary>
        public DateTime? ContractorPrintedDate { get; set; }
    }
}
