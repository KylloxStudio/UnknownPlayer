using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    public Text title;
    public Text startMsg;

    private bool isActiveTitle;
    private bool isActiveStartMsg;

    // Start is called before the first frame update
    private void Start()
    {
        title.text = "";
        UIManager.TypingEffect(title, "Unknown Player");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isActiveTitle && title.text == "Unknown Player")
        {
            isActiveTitle = true;
        }

        if (isActiveTitle && !isActiveStartMsg)
        {
            startMsg.gameObject.SetActive(true);
            UIManager.BlinkEffect(startMsg, 0.5f, false);
            isActiveStartMsg = true;
        }

        if (isActiveStartMsg && Input.anyKeyDown)
        {
            NetworkManager.Connect();
        }
    }
}