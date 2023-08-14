using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player1 player1;
    public Grid grid;
    public Tilemap tilemap;
    public GameCamera gameCamera;
    public Text hp;
    public Text stamina;

    public bool isChangingTextColor;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        hp.text = "HP: 500";
        stamina.text = "Stamina: 2000";
    }

    // Update is called once per frame
    private void Update()
    {
        if (player1.health > int.Parse(hp.text.Split(' ')[1]))
        {
            StartCoroutine(ChangeTextColor(hp, new Color(0, 1, 0)));
        }

        if (player1.health < int.Parse(hp.text.Split(' ')[1]))
        {
            StartCoroutine(ChangeTextColor(hp, new Color(1, 0, 0)));
        }

        hp.text = "HP: " + player1.health.ToString();
        stamina.text = "Stamina: " + player1.stamina.ToString();
    }

    public IEnumerator ChangeTextColor(Text text, Color color)
    {
        isChangingTextColor = true;
        Color originColor = text.color;
        for (int i = 0; i < 2; i++)
        {
            text.color = color;
            yield return new WaitForSeconds(0.1f);
            text.color = originColor;
            yield return new WaitForSeconds(0.1f);
        }
        isChangingTextColor = false;
    }

    public Text GetText(string name)
    {
        if (name == "hp")
        {
            return hp;
        }

        if (name == "stamina")
        {
            return stamina;
        }

        return null;
    }

    public Grid GetGrid()
    {
        return grid;
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public GameCamera GetCamera()
    {
        return gameCamera;
    }
}