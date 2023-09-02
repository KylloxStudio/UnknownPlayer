using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviour
{
    public Image fadePanel;

    // Start is called before the first frame update
    private void Start()
    {
        fadePanel.gameObject.SetActive(true);
        UIManager.Instance.FadeIn(fadePanel, 1.5f);
        StartCoroutine(NetworkManager.Instance.EmitWaiting());
    }

    // Update is called once per frame
    private void Update()
    {
        if (NetworkManager.Instance.isMatched)
            SceneManager.LoadScene("GameScene");
    }
}