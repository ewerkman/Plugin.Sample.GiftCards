using System;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Plugin.Sample.GiftCards.Commands;
using Plugin.Sample.GiftCards.Models;
using Sitecore.Commerce.Core;

namespace Plugin.Sample.GiftCards.Controllers
{
    public class CommandsController : CommerceODataController
    {
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }
        
        [HttpPost]
        [ODataRoute("CreateGiftCard()", RouteName = CoreConstants.CommerceOpsApi)]
        public async Task<IActionResult> CreateGiftCard([FromBody] GiftCardModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            var command = Command<CreateGiftCardCommand>();
            await command.Process(CurrentContext, model);
            return new ObjectResult(command);
        }
    }
}