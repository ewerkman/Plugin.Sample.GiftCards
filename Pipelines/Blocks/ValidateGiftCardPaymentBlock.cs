using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.GiftCards.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.GiftCards;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.GiftCards.Pipelines.Blocks
{
    /// <summary>
    ///     This blocks checks whether the Gift Card is still valid (has not expired and is enabled).
    /// </summary>
    public class ValidateGiftCardPaymentBlock : AsyncPipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline findEntityPipeline;

        public ValidateGiftCardPaymentBlock(IFindEntityPipeline findEntityPipeline)
        {
            this.findEntityPipeline = findEntityPipeline;
        }
        
        public override async Task<Cart> RunAsync(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The cart can not be null");
            
            // The CartPaymentsArgument was added to the context earlier in the pipeline
            var argument = context.CommerceContext.GetObject<CartPaymentsArgument>();
            if (argument == null)
            {
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "ArgumentNotFound",
                        new object[]
                        {
                            typeof(CartPaymentsArgument).Name
                        },
                        $"Argument of type {typeof(CartPaymentsArgument).Name} was not found in context.").ConfigureAwait(false),
                    context);
                return null;
            }

            var payment = argument.Payments?.OfType<GiftCardPaymentComponent>().FirstOrDefault();
            if (payment == null)
            {
                return arg;
            }
            
            var giftCardId = payment.GiftCardCode.Replace("-", "_");
            var giftCardEntityId = $"{CommerceEntity.IdPrefix<GiftCard>()}{giftCardId}";
            var giftCard = await findEntityPipeline.RunAsync(new FindEntityArgument(typeof(GiftCard), giftCardEntityId), context).ConfigureAwait(false) as GiftCard;
            if (giftCard != null)
            {
                var isValid = await ValidateGiftCard(giftCard, context).ConfigureAwait(false);
                if (isValid)
                {
                    return arg;
                }

                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "GiftCardPaymentNotValid",
                        new object[]
                        {
                            payment.GiftCardCode
                        },
                        $"Gift card payment '{payment.GiftCardCode}' is not valid.").ConfigureAwait(false),
                    context);
                return arg;
                var giftCardPolicy = giftCard.GetPolicy<GiftCardPolicy>();
                if (!giftCardPolicy.Enabled)
                {
                    
                }
            }
            
            context.Abort(
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "GiftCardNotFound",
                    new object[]
                    {
                        payment.GiftCardCode
                    },
                    $"Gift card {payment.GiftCardCode} was not found.").ConfigureAwait(false),
                context);
            return arg;
        }

        protected virtual async Task<bool> ValidateGiftCard(GiftCard giftCard, CommercePipelineExecutionContext context)
        {
            var giftCardPolicy = giftCard.GetPolicy<GiftCardPolicy>();
            if (!giftCardPolicy.Enabled)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "GiftCardNotEnabled",
                    new object[]
                    {
                        giftCard.GiftCardCode
                    },
                    $"Gift card '{giftCard.GiftCardCode}' is not enabled.").ConfigureAwait(false);
                
                return false;
            }

            if (DateTimeOffset.Now > giftCardPolicy.ExpiresOn)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "GiftCardHasExpired",
                    new object[]
                    {
                        giftCard.GiftCardCode
                    },
                    $"Gift card '{giftCard.GiftCardCode}' has expired ({giftCardPolicy.ExpiresOn}).").ConfigureAwait(false);
                
                return false;
            }
            
            return true;
        }
    }
}