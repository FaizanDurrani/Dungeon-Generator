using UnityEngine;

namespace Types
{
    public struct RenderInfo
    {
        public readonly string Representation;
        public readonly Color32 ForeColor;
        public readonly Color32 BackColor;
        public readonly int Layer;

        public RenderInfo(string representation, string description, Color32 foreColor, Color32 backColor, int layer)
        {
            Representation = representation;
            ForeColor = foreColor;
            BackColor = backColor;
            Layer = layer;
        }
    }
}