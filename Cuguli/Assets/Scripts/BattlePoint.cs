using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattlePoint : MonoBehaviour
{
    [SerializeField] private string arenaSceneName;
    [SerializeField] private bool loadOnClick = true;
    private bool isLoading;

    private void OnMouseDown()
    {
        Debug.Log($"Battle point clicked: {name}", this);
        TryLoadOnClick();
    }

    private void OnMouseUpAsButton()
    {
        TryLoadOnClick();
    }

    private void Update()
    {
        if (!loadOnClick || isLoading || Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Camera camera = Camera.main;
        if (camera == null)
            camera = FindFirstObjectByType<Camera>();

        if (camera == null)
        {
            Debug.LogError("BattlePoint could not find a camera for mouse picking.", this);
            return;
        }

        Vector2 mousePosition = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePosition);
        foreach (Collider2D hit in hits)
        {
            BattlePoint battlePoint = hit.GetComponentInParent<BattlePoint>();
            if (battlePoint == this)
            {
                TryLoadOnClick();
                return;
            }
        }

    }

    private void TryLoadOnClick()
    {
        if (loadOnClick && !isLoading)
            LoadArena();
    }

    public void LoadArena()
    {
        if (string.IsNullOrWhiteSpace(arenaSceneName))
        {
            Debug.LogError($"{nameof(BattlePoint)} on {name} has no arena scene assigned.", this);
            return;
        }

        int buildIndex = SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{arenaSceneName}.unity");
        if (buildIndex < 0)
        {
            Debug.LogError($"Arena scene '{arenaSceneName}' is not in the active Build Profile.", this);
            return;
        }

        isLoading = true;
        Debug.Log($"Loading arena scene '{arenaSceneName}' (build index {buildIndex}).", this);
        SceneManager.LoadScene(buildIndex);
    }

    private void OnValidate()
    {
        arenaSceneName = arenaSceneName == null ? string.Empty : arenaSceneName.Trim();
    }
}
