using System.Collections.Generic;

namespace Binboo.Core.Plugins
{
    public interface IPluginManager
    {
        ISet<IPlugin> Plugins { get; }
    }
}
