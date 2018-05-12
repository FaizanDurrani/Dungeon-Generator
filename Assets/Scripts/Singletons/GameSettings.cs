using DungeonGeneration;
using Structs;

namespace Singletons
{
    public class GameSettings : Singleton<GameSettings>
    {
        public RenderInfo wallRenderInfo;

        public RenderInfo roomRenderInfo;

        public RenderInfo playerRenderInfo;
        public RenderInfo debugRenderInfo1, debugRenderInfo2, debugRenderInfo3;
    }
}