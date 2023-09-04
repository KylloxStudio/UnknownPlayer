using System.Collections;
using System.Collections.Generic;
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

        if (NetworkManager.Instance.isPlayer1)
        {
            playerController = player1.GetComponent<PlayerController>();
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            playerController = player2.GetComponent<PlayerController>();
        }
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        NetworkManager.Instance.EmitGameLoad();
        yield return new WaitUntil(() => NetworkManager.Instance.isGameLoaded);
        maxHp = playerController.health;
        curHp = playerController.health;
        maxStamina = playerController.stamina;
        curStamina = playerController.stamina;

        hpBar.value = curHp / maxHp;
        hpText.text = curHp + " / " + maxHp;
        staminaBar.value = curStamina / maxStamina;
        staminaText.text = curStamina + " / " + maxStamina;

        HandleHp();
        HandleStamina();
        GetPlayerData();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleHp();
        HandleStamina();
        GetPlayerData();
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

    private void GetPlayerData()
    {
        if (NetworkManager.Instance.isPlayer1)
        {
            player2.health = NetworkManager.Instance.player2Hp;
            player2.stamina = NetworkManager.Instance.player2Stamina;
            player2.transform.position = NetworkManager.Instance.player2Position;
            player2.transform.rotation = NetworkManager.Instance.player2Rotation;
            foreach (KeyValuePair<string, bool> item in NetworkManager.Instance.player2Animations)
            {
                Debug.Log(item.Key);
                Debug.Log(item.Value);
                player2.anim.SetBool(item.Key, item.Value);
            }
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            player1.health = NetworkManager.Instance.player1Hp;
            player1.stamina = NetworkManager.Instance.player1Stamina;
            player1.transform.position = NetworkManager.Instance.player1Position;
            player1.transform.rotation = NetworkManager.Instance.player1Rotation;
            foreach (KeyValuePair<string, bool> item in NetworkManager.Instance.player1Animations)
            {
                player1.anim.SetBool(item.Key, item.Value);
            }
        }
    }

    public static GameCamera GetCamera()
    {
        return Instance.gameCamera;
    }
}