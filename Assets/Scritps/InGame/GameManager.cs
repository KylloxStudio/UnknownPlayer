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

        playerController.SendStatistics();
    }

    // Start is called before the first frame update
    private void Start()
    {
        NetworkManager.Instance.EmitGameLoad();

        StartCoroutine(HandleHp());
        StartCoroutine(HandleStamina());
        StartCoroutine(GetPlayersData());
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private IEnumerator HandleHp()
    {
        yield return new WaitUntil(() => NetworkManager.Instance.isGameLoaded);
        maxHp = playerController.health;
        curHp = playerController.health;
        hpBar.value = curHp / maxHp;
        hpText.text = curHp + " / " + maxHp;
        while (NetworkManager.Instance.isGameLoaded)
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
            yield return null;
        }
    }

    private IEnumerator HandleStamina()
    {
        yield return new WaitUntil(() => NetworkManager.Instance.isGameLoaded);
        maxStamina = playerController.stamina;
        curStamina = playerController.stamina;
        staminaBar.value = curStamina / maxStamina;
        staminaText.text = curStamina + " / " + maxStamina;
        while (NetworkManager.Instance.isGameLoaded)
        {
            curStamina = playerController.stamina;
            imsiStamina = curStamina / maxStamina;
            staminaText.text = curStamina + " / " + maxStamina;
            staminaBar.value = Mathf.Lerp(staminaBar.value, imsiStamina, Time.deltaTime * 10);
            yield return null;
        }
    }

    private IEnumerator GetPlayersData()
    {
        yield return new WaitUntil(() => NetworkManager.Instance.isGameLoaded);
        while (NetworkManager.Instance.isGameLoaded)
        {
            if (NetworkManager.Instance.isPlayer1)
            {
                yield return new WaitUntil(() => NetworkManager.Instance.player2Statistics.Length == 2);
                player2.health = NetworkManager.Instance.player2Statistics[0];
                player2.stamina = NetworkManager.Instance.player2Statistics[1];
                player2.transform.position = NetworkManager.Instance.player2Position;
                player2.transform.rotation = NetworkManager.Instance.player2Rotation;
                for (int i = 0; i < player2.anim.parameterCount; i++)
                {
                    yield return new WaitUntil(() => NetworkManager.Instance.player2Animations.TryGetValue(player2.anim.parameters[i].name, out bool boolean));
                    player2.anim.SetBool(player2.anim.parameters[i].name, NetworkManager.Instance.player2Animations[player2.anim.parameters[i].name]);
                }
            }
            else if (NetworkManager.Instance.isPlayer2)
            {
                yield return new WaitUntil(() => NetworkManager.Instance.player1Statistics.Length == 2);
                player1.health = NetworkManager.Instance.player1Statistics[0];
                player1.stamina = NetworkManager.Instance.player1Statistics[1];
                player1.transform.position = NetworkManager.Instance.player1Position;
                player1.transform.rotation = NetworkManager.Instance.player1Rotation;
                for (int i = 0; i < player1.anim.parameterCount; i++)
                {
                    yield return new WaitUntil(() => NetworkManager.Instance.player1Animations.TryGetValue(player1.anim.parameters[i].name, out bool boolean));
                    player1.anim.SetBool(player1.anim.parameters[i].name, NetworkManager.Instance.player1Animations[player1.anim.parameters[i].name]);
                }
            }
            yield return null;
        }
    }

    public static GameCamera GetCamera()
    {
        return Instance.gameCamera;
    }
}