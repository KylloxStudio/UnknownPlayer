using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    public Image fadePanel;

    // Start is called before the first frame update
    private void Start()
    {
        fadePanel.gameObject.SetActive(true);
        UIManager.Instance.FadeIn(fadePanel, 1.5f);
        NetworkManager.Instance.EmitWaiting();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}