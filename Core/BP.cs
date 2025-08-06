using Nautilus.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrecursorLibrary
{
    public interface IBindablePrefab
    {
        PrefabInfo Info { get; }
        void Configure(ICustomPrefab customPrefab);
        void Register();
    }
}
