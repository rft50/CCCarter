using Nanoray.PluginManager;
using Nickel;

namespace Carter;

internal interface IRegisterable
{
    static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}