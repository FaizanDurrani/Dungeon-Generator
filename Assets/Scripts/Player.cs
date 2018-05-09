using Types;
using UnityEngine;
using Display = PhiOS.Scripts.PhiOS.Display;

public class Player : Singleton<Player>
{
    //private Vector2Int CameraPos => Position - new Vector2Int(Display.GetDisplayWidth() / 2, Display.GetDisplayHeight() / 2);
    private CellData _player;

    public Vector2Int Position;
    public bool CanControl;
    public float FieldOfView;

    private void Update()
    {
        if (!CanControl) return;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Position += new Vector2Int(1, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Position += new Vector2Int(-1, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Position += new Vector2Int(0, -1);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Position += new Vector2Int(0, 1);
        }

        if (_player.Position != Position)
        {
            SetFieldOfView(_player.Position, FieldOfView, false);
            _player.Position = Position;
            SetFieldOfView(_player.Position, FieldOfView, true);
        }

        _player.RenderInfo = GameSettings.Instance.playerRenderInfo;
        CellRenderer.Instance.QueueCellRender(_player);
    }

    private void SetFieldOfView(Vector2Int position, float radius, bool visible)
    {
        Dungeon currentDungeon = DungeonGenerator.Instance.CurrentDungeon;
        int width = Display.GetDisplayWidth() - 1;
        int height = Display.GetDisplayHeight() - 1;
//        int width = currentDungeon.GridSize.x;
//        int height = currentDungeon.GridSize.y;

        Vector2 center = new Vector2(position.x + .5f,
            position.y + .5f);
        int numRays = 100;

        for (int r = 0; r < numRays; r++)
        {
            float dirX = Mathf.Sin(2 * Mathf.PI * r / numRays);
            float dirY = Mathf.Cos(2 * Mathf.PI * r / numRays);
            Vector2 direction = new Vector2(dirX, dirY);

            for (int d = 1; d < radius; d++)
            {
                Vector2 relative = center + direction * d;

                int y = (int) relative.y;
                if (y < 0 || y >= width) break;

                int wrappedX = (int) relative.x;
                if (wrappedX < 0) wrappedX = width + wrappedX;
                if (wrappedX >= height) wrappedX = wrappedX - width;

                Vector2Int tilePos = new Vector2Int(wrappedX, y);
                if (!currentDungeon.TileExists(tilePos))
                    break;

                CellData cell = currentDungeon.GetLastCell(tilePos);
                cell.Discovered = true;
                cell.Visible = visible;
                currentDungeon.SetLastCell(tilePos, cell);

                if (cell.Type == CellType.Wall)
                {
                    break;
                }
            }
        }
    }
}