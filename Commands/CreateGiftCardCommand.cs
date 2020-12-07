using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Sample.GiftCards.Components;
using Plugin.Sample.GiftCards.Models;
using Plugin.Sample.GiftCards.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Entitlements;
using Sitecore.Commerce.Plugin.GiftCards;
using Sitecore.Commerce.Plugin.ManagedLists;
using GiftCardComponent = Plugin.Sample.GiftCards.Components.GiftCardComponent;

namespace Plugin.Sample.GiftCards.Commands
{
    public class CreateGiftCardCommand : CommerceCommand
    {
        private readonly CommerceCommander commander;

        public CreateGiftCardCommand(CommerceCommander commander)
        {
            this.commander = commander;
        }
        
        public virtual async Task<GiftCard> Process(CommerceContext commerceContext, GiftCardModel giftCardModel)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var giftCardId = giftCardModel.Code.Replace("-", "_");
                var giftCardEntityId = $"{CommerceEntity.IdPrefix<GiftCard>()}{giftCardId}";
                    
                var giftCard = await commander.GetEntity<GiftCard>(commerceContext, giftCardEntityId);
                if (giftCard == null)
                {
                    // Create new gift card
                    giftCard = new GiftCard
                    {
                        Id = giftCardEntityId,
                        Name = giftCardModel.Code,
                        Balance = new Money("EUR", giftCardModel.Amount - giftCardModel.AmountUsed),
                        ActivationDate = giftCardModel.CreatedOn,
                        OriginalAmount = new Money("EUR", giftCardModel.Amount),
                        FriendlyId = giftCardModel.Code,
                        GiftCardCode = giftCardModel.Code,
                        Order = new EntityReference(giftCardModel.OrderId)
                    };

                    // Add giftcard to lists 
                    giftCard.SetComponent(new ListMembershipsComponent
                    {
                        Memberships = new List<string>
                        {
                            $"{CommerceEntity.ListName<Entitlement>()}",
                            $"{CommerceEntity.ListName<GiftCard>()}"
                        }
                    });

                    giftCard.CreatedBy = giftCardModel.CreatedBy;

                    // Add additional specific components and policies
                    var giftCardComponent = giftCard.GetComponent<GiftCardComponent>();
                    giftCardComponent.Notes = giftCardModel.Note;

                    var giftCardPolicy = giftCard.GetPolicy<GiftCardPolicy>();
                    giftCardPolicy.ExpiresOn = giftCardModel.ExpiresOn;
                    giftCardPolicy.Enabled = giftCardModel.Enabled;
                    
                    // Save the new gift card
                    await commander.PersistEntity(commerceContext, giftCard);
                    
                    // Create and save an entity index based on the giftcard code
                    var indexByGiftCardCode = new EntityIndex(giftCard, giftCard.GiftCardCode)
                    {
                        Id = giftCardEntityId
                    };
                    await commander.PersistEntity(commerceContext, indexByGiftCardCode);
                }

                return giftCard;
            }
        }
    }
}