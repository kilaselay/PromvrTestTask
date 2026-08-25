using UnityEngine;

namespace PromvrTestTask
{
    internal readonly struct PlaneGeneratorVariables
    {
        internal static readonly int planeSideSize = Shader.PropertyToID(nameof(planeSideSize));
        internal static readonly int verticesPerSide = Shader.PropertyToID(nameof(verticesPerSide));
        internal static readonly int quadsPerSide = Shader.PropertyToID(nameof(quadsPerSide));
        internal static readonly int verticesData = Shader.PropertyToID(nameof(verticesData));
        internal static readonly int triangles = Shader.PropertyToID(nameof(triangles));
    }
}
