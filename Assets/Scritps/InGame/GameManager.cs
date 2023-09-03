using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerController playerController;
    public Player1 player1;
    public Player2 player2;
    public Grid grid;
    public Tilemap tilemap;
    public GameCamera gameCamera;
    public Slider hpBar;
    public Text hpText;
    public Slider staminaBar;
    public Text staminaText;
    public Text enemyHpText;

    public float maxHp;
    public float curHp;
    public float maxStamina;
    public float curStamina;
    public float imsiHp;
    public float imsiStamina;
    public float oppoMaxHp;
    public float oppoCurHp;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        NetworkManager.Instance.EmitGameLoad();

        if (NetworkManager.Instance.isPlayer1)
        {
            playerController = player1.GetComponent<PlayerController>();
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            playerController = player2.GetComponent<PlayerController>();
        }

        maxHp = playerController.health;
        curHp = playerController.health;
        maxStamina = playerController.stamina;
        curStamina = playerController.stamina;

        hpBar.value = curHp / maxHp;
        hpText.text = curHp + " / " + maxHp;
        staminaBar.value = curStamina / maxStamina;
        staminaText.text = curStamina + " / " + maxStamina;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleHp();
        HandleStamina();
    }

    private void HandleHp()
    {
        curHp = playerController.health;
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
        curStamina = playerController.stamina;
        imsiStamina = curStamina / maxStamina;
        staminaText.text = curStamina + " / " + maxStamina;
        staminaBar.value = Mathf.Lerp(staminaBar.value, imsiStamina, Time.deltaTime * 10);
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