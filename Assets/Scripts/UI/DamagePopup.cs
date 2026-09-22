using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private float moveYSpeed = 1.5f;
    [SerializeField] private float disappearTimer = 0.6f;
    [SerializeField] private float fadeSpeed = 3f;

    private Color textColor;

    private void Awake()
    {
        if (damageText == null)
            damageText = GetComponent<TMP_Text>();

        textColor = damageText.color;
    }

    public void Setup(int damageAmount, Color color)
    {
        damageText.text = damageAmount.ToString();
        damageText.color = color;
        textColor = color;

        transform.position += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0f, 0.2f), 0);
    }

    private void Update()
    {
        transform.position += Vector3.up * (moveYSpeed * Time.deltaTime);

        disappearTimer -= Time.deltaTime;
        if(disappearTimer <= 0)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            damageText.color = textColor;

            if(textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
