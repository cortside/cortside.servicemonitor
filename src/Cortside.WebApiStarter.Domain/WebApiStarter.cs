using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cortside.WebApiStarter.Domain {

    [Table("WebApiStarter")]
    public class WebApiStarter : AuditableEntity {

        public WebApiStarter() {

        }

        /// <summary>
        /// Unique identifier for a WebApiStarter
        /// </summary>
        [Key]
        public Guid WebApiStarterId { get; set; }


    }
}
