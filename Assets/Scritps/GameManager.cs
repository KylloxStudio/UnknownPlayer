using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player1 player1;
    public Enemy enemy;
    public Grid grid;
    public Tilemap tilemap;
    public GameCamera gameCamera;
    public UIManager ui;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public static Player1 GetPlayer1()
    {
        return Instance.player1;
    }

    public static Enemy GetEnemy()
    {
        return Instance.enemy;
    }

    public static UIManager GetUIManager()
    {
        return Instance.ui;
    }

    public static Grid GetGrid()
    {
        return Instance.grid;
    }

    public static Tilemap GetTilemap()
    {
        return Instance.tilemap;
    }

    public static GameCamera GetCamera()
    {
        return Instance.gameCamera;
    }
}