using System;
using Microsoft.AspNet.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.GiftCards
{
    public class ConfigureOpsServiceApiBlock : SyncPipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        public override ODataConventionModelBuilder Run(ODataConventionModelBuilder arg, CommercePipelineExecutionContext context)
        {
            var createGiftCardAction = arg.Function("CreateGiftCard");
            createGiftCardAction.Parameter<string>("code");
            createGiftCardAction.Parameter<DateTimeOffset>("createdOn");
            createGiftCardAction.Parameter<string>("createdBy");
            createGiftCardAction.Parameter<DateTimeOffset>("expiresOn");
            createGiftCardAction.Parameter<decimal>("amount");
            createGiftCardAction.Parameter<decimal>("amountUsed");
            createGiftCardAction.Parameter<bool>("enabled");
            createGiftCardAction.Parameter<string>("note");
            createGiftCardAction.ReturnsFromEntitySet<CommerceCommand>("Commands");
            
            return arg;
        }
    }
}