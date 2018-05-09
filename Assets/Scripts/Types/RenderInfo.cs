using System;
using UnityEngine;
using Display = PhiOS.Scripts.PhiOS.Display;

namespace Types
{
    [Serializable]
    public struct RenderInfo
    {
        public string Representation;
        public Color ForeColor;
        public Color BackColor;
        public Color HiddenTint;

        public RenderInfo(RenderInfo r)
        {
            Representation = r.Representation;
            ForeColor = r.ForeColor;
            BackColor = r.BackColor;
            HiddenTint = r.HiddenTint;
        }
    }
}