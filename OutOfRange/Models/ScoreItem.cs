//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OutOfRange.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ScoreItem
    {
        public System.Guid ItemID { get; set; }
        public string UserID { get; set; }
        public Nullable<decimal> Score { get; set; }
        public Nullable<System.DateTime> Added { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Question Question { get; set; }
        public virtual Comment Comment { get; set; }
        public virtual Answer Answer { get; set; }
    }
}