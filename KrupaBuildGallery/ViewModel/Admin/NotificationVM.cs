using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KrupaBuildGallery
{
    public class NotificationVM
    {
        public long NotificationId { get; set; }
        [Required, Display(Name = "Notification Title")]
        public string NotificationTitle { get; set; }
        [Required, Display(Name = "Notification Description")]
        public string NotificationDescription { get; set; }
        [Display(Name = "Notification Image")]
        public HttpPostedFileBase NotificationImageFile { get; set; }
        public string NotificationImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }

        // Additional fields
        public string strCreatedBy { get; set; } 
        public string strModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string NotificationImageUrl { get; set; }
    }
}