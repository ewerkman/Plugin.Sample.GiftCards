﻿// © 2016 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.GiftCards.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.AllocateCoupon.Pipelines.Blocks.RegisteredPluginBlock")]
    public class RegisteredPluginBlock : SyncPipelineBlock<IEnumerable<RegisteredPluginModel>, IEnumerable<RegisteredPluginModel>, CommercePipelineExecutionContext>
    {
        public override IEnumerable<RegisteredPluginModel> Run(IEnumerable<RegisteredPluginModel> arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }

            var plugins = arg.ToList();
            PluginHelper.RegisterPlugin(this, plugins);

            return plugins.AsEnumerable();
        }
    }
}
