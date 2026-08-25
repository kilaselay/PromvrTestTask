using UnityEngine;

namespace PromvrTestTask
{
    internal readonly struct FillingHeightMapVariables
    {
        internal static readonly int heightMap = Shader.PropertyToID(nameof(heightMap));
        internal static readonly int textureSize = Shader.PropertyToID(nameof(textureSize));
        internal static readonly int planeSize = Shader.PropertyToID(nameof(planeSize));
        internal static readonly int accumulationMaterialSpeed = Shader.PropertyToID(nameof(accumulationMaterialSpeed));
        internal static readonly int deltaTime = Shader.PropertyToID(nameof(deltaTime));
        internal static readonly int previousWorldSphere = Shader.PropertyToID(nameof(previousWorldSphere));
        internal static readonly int currentWorldSphere = Shader.PropertyToID(nameof(currentWorldSphere));
    }
}
