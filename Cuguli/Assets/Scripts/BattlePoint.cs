using UnityEngine;
using UnityEngine.SceneManagement;

public class BattlePoint : MonoBehaviour
{
    [SerializeField] private string arenaSceneName;
    [SerializeField] private bool loadOnClick = true;

    private void OnMouseDown()
    {
        if (loadOnClick)
            LoadArena();
    }

    public void LoadArena()
    {
        if (string.IsNullOrWhiteSpace(arenaSceneName))
        {
            Debug.LogError($"{nameof(BattlePoint)} on {name} has no arena scene assigned.", this);
            return;
        }

        SceneManager.LoadScene(arenaSceneName);
    }

    private void OnValidate()
    {
        arenaSceneName = arenaSceneName == null ? string.Empty : arenaSceneName.Trim();
    }
}
