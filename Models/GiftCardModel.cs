using System;

namespace Plugin.Sample.GiftCards.Models
{
    public class GiftCardModel
    {
        public string Code { get; set; } 
        public string OrderId { get; set; }
        public DateTimeOffset CreatedOn { get; set; } 
        public string CreatedBy { get; set; } 
        public DateTimeOffset ExpiresOn { get; set; } 
        public decimal Amount { get; set; } 
        public decimal AmountUsed { get; set; } 
        public bool Enabled { get; set; } 
        public string Note { get; set; }   
    }
}