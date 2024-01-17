using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviour
{
    public Image fadePanel;

    // Start is called before the first frame update
    void Start()
    {
        fadePanel.gameObject.SetActive(true);
        UIManager.FadeIn(fadePanel, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}