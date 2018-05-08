using UnityEngine;

public class GameProperties : Singleton<GameProperties>
{
    public string RoomRepresentation = "█";
    public string WallRepresentation = "#";
    public Color32 WallForeColor = new Color(0.32f,0.32f,0.32f);
    public Color32 WallBackColor = new Color(0.32f,0.32f,0.32f);
    public Color32 RoomForeColor = new Color(0.16f, 0.38f, 0.16f);
    public Color32 RoomBackColor = new Color(0.16f, 0.38f, 0.16f);
}