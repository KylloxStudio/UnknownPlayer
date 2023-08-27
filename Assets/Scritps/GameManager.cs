using UnityEngine;
using UnityEngine.UI;
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
    public Slider hpBar;
    public Text hpText;
    public Slider staminaBar;
    public Text staminaText;
    public Text enemyHpText;

    private float maxHp;
    private float curHp;
    private float maxStamina;
    private float curStamina;
    private float imsiHp;
    private float imsiStamina;
    private float enemyMaxHp;
    private float enemyCurHp;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;

        maxHp = player1.health;
        curHp = player1.health;
        maxStamina = player1.stamina;
        curStamina = player1.stamina;
        enemyMaxHp = enemy.hp;
        enemyCurHp = enemy.hp;

        hpBar.value = curHp / maxHp;
        hpText.text = curHp + " / " + maxHp;
        staminaBar.value = curStamina / maxStamina;
        staminaText.text = curStamina + " / " + maxStamina;

        enemyHpText.text = "Enemy HP: " + enemyCurHp + " / " + enemyMaxHp;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleHp();
        HandleStamina();
        enemyCurHp = enemy.hp;
        enemyHpText.text = "Enemy HP: " + enemyCurHp + " / " + enemyMaxHp;
    }

    private void HandleHp()
    {
        curHp = player1.health;
        hpText.text = curHp + " / " + maxHp;
        if (curHp > 0)
        {
            imsiHp = curHp / maxHp;
            hpBar.value = Mathf.Lerp(hpBar.value, imsiHp, Time.deltaTime * 10);
        }
        else
        {
            Image hpFill = hpBar.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
            hpFill.color = new Color(1, 0, 0);
            hpBar.direction = Slider.Direction.RightToLeft;
            hpBar.value = Mathf.Lerp(hpBar.value, 1, Time.deltaTime * 10);
        }
    }

    private void HandleStamina()
    {
        curStamina = player1.stamina;
        imsiStamina = curStamina / maxStamina;
        staminaText.text = curStamina + " / " + maxStamina;
        staminaBar.value = Mathf.Lerp(staminaBar.value, imsiStamina, Time.deltaTime * 10);
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