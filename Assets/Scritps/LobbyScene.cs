using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    public Image fadePanel;

    // Start is called before the first frame update
    void Start()
    {
        fadePanel.gameObject.SetActive(true);
        UIManager.Instance.FadeIn(fadePanel, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
