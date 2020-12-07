using System;
using Sitecore.Commerce.Core;

namespace Plugin.Sample.GiftCards.Policies
{
    public class GiftCardPolicy : Policy
    {
        public DateTimeOffset ExpiresOn { get; set; }
        public bool Enabled { get; set; } 
    }
}