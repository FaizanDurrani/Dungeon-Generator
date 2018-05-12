using System;
using UnityEngine;

namespace DungeonGeneration
{
    [Serializable]
    public class RenderInfo
    {
        public string Representation;
        public Color ForeColor;
        public Color BackColor;
        public Color HiddenTint;

        public RenderInfo(RenderInfo r)
        {
            if (r == null) return;
            Representation = r.Representation ?? "";
            ForeColor = r.ForeColor;
            BackColor = r.BackColor;
            HiddenTint = r.HiddenTint;
        }
    }
}