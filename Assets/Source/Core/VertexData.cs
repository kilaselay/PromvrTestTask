using System.Runtime.InteropServices;
using UnityEngine;

namespace PromvrTestTask
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexData
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
    }
}
