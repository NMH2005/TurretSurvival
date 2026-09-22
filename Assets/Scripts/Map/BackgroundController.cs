using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Update()
    {
        gameObject.transform.position = player.position;
    }
}
