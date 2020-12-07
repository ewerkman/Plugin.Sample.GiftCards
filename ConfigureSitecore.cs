using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Sample.GiftCards.Pipelines.Blocks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Plugin.Sample.GiftCards
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllCommands(assembly);
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(
                config =>
                    config
                        .ConfigurePipeline<IConfigureOpsServiceApiPipeline>(c =>
                            c.Add<ConfigureOpsServiceApiBlock>()
                        )
                        .ConfigurePipeline<IRunningPluginsPipeline>(c =>
                        {
                            c.Add<Pipelines.Blocks.RegisteredPluginBlock>().After<RunningPluginsBlock>();
                        })
                        .ConfigurePipeline<IAddPaymentsPipeline>(c =>
                            c.Add<ValidateGiftCardPaymentBlock>().After<Sitecore.Commerce.Plugin.GiftCards.ValidateGiftCardPaymentBlock>()
                )
                    );
        }
    }
}