using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClickMatchCancel()
    {
        NetworkManager.Instance.socket.Emit("quit");
        SceneManager.LoadScene("MainScene");
    }
}