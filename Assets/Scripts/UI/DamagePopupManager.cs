using UnityEngine;

public class DamagePopupManager : MonoBehaviour {
    public static DamagePopupManager Instance { get; private set; }

    [SerializeField] private DamagePopup popupPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Show(int damage, Vector3 position, Color? color = null)
    {
        if (popupPrefab == null) return;

        DamagePopup popup = Instantiate(popupPrefab, position, Quaternion.identity);
        popup.Setup(damage, color ?? Color.white);
    }
}
