using UnityEngine;

public class CoreController : MonoBehaviour
{
    public static CoreController Instance;

    public enum GameState { Menu, InGame, EndGame, GameOver }
    public GameState CurrentState {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Stan gry: {newState}");
    }
}
