//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KrupaBuildGallery.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_ChatRoom
    {
        public long Pk_ChatRoomId { get; set; }
        public string RoomUniqueId { get; set; }
        public string RoomName { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<bool> IsPrivate { get; set; }
    }
}
