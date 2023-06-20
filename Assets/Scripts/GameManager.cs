using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeToStart = 3.0f;
    private PlayerController playerController;

    private bool _gameOver = false;
    private bool _go = false;

    public bool GameOver { get => _gameOver; set => _gameOver = value; }
    public bool Go { get => _go; set => _go = value; }

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.SetActiveAnimator(false);
        _ = StartCoroutine(CountDownCourutine());
    }

    IEnumerator CountDownCourutine()
    {
        while (timeToStart > 0)
        {
            yield return new WaitForSeconds(1f);
            timeToStart -= 1;
        }

        StartGame();

        yield return null;
    }

    private void StartGame()
    {
        playerController.SetActiveAnimator(true);
        _gameOver = false;
        _go = true;
    }

    public void EndGame()
    {
        _gameOver = true;
        _go = false;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(Screen.width-120, 0, 100, 50), timeToStart.ToString(), style);

        if (GUI.Button(new Rect(Screen.width - 120, 60, 100, 50), "RESTART"))
        {
            SceneManager.LoadScene("Prototype");
        }
    }
}
