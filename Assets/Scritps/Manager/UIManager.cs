using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    static bool isChangingTextColor;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Screen.SetResolution(960, 540, false);
        //SetResolution();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private static IEnumerator ChangeTextColor(Text text, Color color, bool highlight)
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

    private static IEnumerator Typing(Text text, string msg)
    {
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i <= msg.Length; i++)
        {
            text.text = msg.Substring(0, i);
            yield return new WaitForSeconds(0.15f);
        }
    }

    private static IEnumerator Fade(Image image, float time, bool fadein)
    {
        Color color = image.color;
        float percent = 0f;
        while (percent < 1)
        {
            percent += Time.deltaTime / time;
            if (fadein) color.a = Mathf.Lerp(1f, 0f, percent);
            else color.a = Mathf.Lerp(0f, 1f, percent);
            image.color = color;
            yield return null;
        }
        image.gameObject.SetActive(false);
    }

    private static IEnumerator Fade(Text text, float time, bool fadein)
    {
        Color color = text.color;
        float percent = 0f;
        while (percent < 1)
        {
            percent += Time.deltaTime / time;
            if (fadein) color.a = Mathf.Lerp(0f, 1f, percent);
            else color.a = Mathf.Lerp(1f, 0f, percent);
            text.color = color;
            yield return null;
        }
    }

    private static IEnumerator BlinkText(Text text, float speed, bool fade)
    {
        string tx = text.text;
        if (fade)
        {
            while (true)
            {
                Color color = text.color;
                float percent = 0f;
                while (percent < 1)
                {
                    percent += Time.deltaTime / speed;
                    color.a = Mathf.Lerp(0f, 1f, percent);
                    text.color = color;
                    yield return null;
                }
                percent = 0f;
                while (percent < 1)
                {
                    percent += Time.deltaTime / speed;
                    color.a = Mathf.Lerp(1f, 0f, percent);
                    text.color = color;
                    yield return null;
                }
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                text.text = "";
                yield return new WaitForSeconds(speed);
                text.text = tx;
                yield return new WaitForSeconds(speed);
            }
        }
    }

    public static void HighlightTextColor(Text text, Color color)
    {
        text.StartCoroutine(ChangeTextColor(text, color, true));
    }

    public static void ChangeTextColor(Text text, Color color)
    {
        text.StartCoroutine(ChangeTextColor(text, color, false));
    }

    public static void TypingEffect(Text text, string msg)
    {
        text.StartCoroutine(Typing(text, msg));
    }

    public static void FadeIn(Image image, float time)
    {
        image.StartCoroutine(Fade(image, time, true));
    }

    public static void FadeOut(Image image, float time)
    {
        image.StartCoroutine(Fade(image, time, false));
    }

    public static void FadeIn(Text text, float time)
    {
        text.StartCoroutine(Fade(text, time, true));
    }

    public static void FadeOut(Text text, float time)
    {
        text.StartCoroutine(Fade(text, time, false));
    }

    public static void BlinkEffect(Text text, float speed, bool fade)
    {
        text.StartCoroutine(BlinkText(text, speed, fade));
    }

    public void SetResolution()
    {
        int setWidth = 960;
        int setHeight = 540;

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