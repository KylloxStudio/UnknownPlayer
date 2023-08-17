using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private Player1 player1;
    private Enemy enemy;
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
    private bool isChangingTextColor;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        player1 = GameManager.GetPlayer1();
        enemy = GameManager.GetEnemy();

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

        SetResolution();
    }

    // Update is called once per frame
    void Update()
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

    public void HighlightTextColor(Text text, Color color)
    {
        StartCoroutine(ChangeTextColor(text, color, true));
    }

    public void ChangeTextColor(Text text, Color color)
    {
        StartCoroutine(ChangeTextColor(text, color, false));
    }

    private IEnumerator ChangeTextColor(Text text, Color color, bool highlight)
    {
        if (highlight)
        {
            if (isChangingTextColor)
            {
                yield break;
            }

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
        else
        {
            text.color = color;
        }
    }

    public void SetResolution()
    {
        int setWidth = 1920;
        int setHeight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), false);

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight)
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight);
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight);
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}
