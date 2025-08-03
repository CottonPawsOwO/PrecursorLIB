using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrecursorLibrary
{
    public abstract class BindablePrefab : IBindablePrefab
    {
        public PrefabInfo Info { get; set; }

        // Token: 0x060002D1 RID: 721
        protected abstract void Configure(ICustomPrefab customPrefab);
        void IBindablePrefab.Configure(ICustomPrefab customPrefab)
        {
            this.Configure(customPrefab);
        }

        void IBindablePrefab.Register()
        {
            CustomPrefab customPrefab = new CustomPrefab(this.Info);
            this.Configure(customPrefab);
            customPrefab.Register();
        }

        /// <summary>
        /// Sets a function as the game object constructor of this custom prefab. This is an asynchronous version.
        /// </summary>
        /// <param name="prefabAsync">The function to set.</param>
        public void SetGameObject(ICustomPrefab customPrefab, Func<IOut<GameObject>, IEnumerator> prefabAsync)
        {
            CustomPrefab customPrefab2 = customPrefab as CustomPrefab;
            if (customPrefab2 == null)
            {
                return;
            }
            customPrefab2.SetGameObject(prefabAsync);
        }

        /// <summary>
        /// Sets a prefab template as the game object constructor of this custom prefab.
        /// </summary>
        /// <param name="prefabTemplate">The prefab template object to set.</param>

        public void SetGameObject(ICustomPrefab customPrefab, PrefabTemplate prefabTemplate)
        {
            CustomPrefab customPrefab2 = customPrefab as CustomPrefab;
            if (customPrefab2 == null)
            {
                return;
            }
            customPrefab2.SetGameObject(prefabTemplate);
        }

        /// <summary>
        /// Sets a game object as the prefab of this custom prefab.
        /// </summary>
        /// <remarks>Only use this overload on GameObjects that are loaded from asset bundles <b>without</b> instantiating them. For objects that could be destroyed on scene load, use <see cref="!:SetGameObject(System.Func&lt;UnityEngine.GameObject&gt;)" /> instead.</remarks>
        /// <param name="prefab">The game object to set.</param>
        public void SetGameObject(ICustomPrefab customPrefab, GameObject prefab)
        {
            CustomPrefab customPrefab2 = customPrefab as CustomPrefab;
            if (customPrefab2 == null)
            {
                return;
            }
            customPrefab2.SetGameObject(prefab);
        }

        /// <summary>
        /// Sets a function as the game object constructor of this custom prefab. This is a synchronous version.
        /// </summary>
        /// <param name="prefab">The function to set.</param>
        public void SetGameObject(ICustomPrefab customPrefab, Func<GameObject> prefab)
        {
            CustomPrefab customPrefab2 = customPrefab as CustomPrefab;
            if (customPrefab2 == null)
            {
                return;
            }
            customPrefab2.SetGameObject(prefab);
        }

        /// <summary>
        /// Sets a post processor for the prefab. This is a synchronous version.
        /// </summary>
        /// <param name="postProcessor">The post processor to set.</param>
        
        public void SetPrefabPostProcessor(ICustomPrefab customPrefab, Action<GameObject> postProcessor)
        {
            CustomPrefab customPrefab2 = customPrefab as CustomPrefab;
            if (customPrefab2 == null)
            {
                return;
            }
            customPrefab2.SetPrefabPostProcessor(postProcessor);
        }

        /// <summary>
        /// Sets a post processor for the prefab. This is an asynchronous version.
        /// </summary>
        /// <param name="postProcessorAsync">The post processor to set.</param>
       
        public void SetPrefabPostProcessor(ICustomPrefab customPrefab, Func<GameObject, IEnumerator> postProcessorAsync)
        {
            CustomPrefab customPrefab2 = customPrefab as CustomPrefab;
            if (customPrefab2 == null)
            {
                return;
            }
            customPrefab2.SetPrefabPostProcessor(postProcessorAsync);
        }
    }
}
