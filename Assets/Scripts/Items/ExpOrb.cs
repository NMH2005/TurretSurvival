using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] private ExpData expdata;

    private ExpConsumer target;
    private float collectDis = 0.2f;

    public void StartAttract(ExpConsumer consumer)
    {
        target = consumer;
    }

    private void Update()
    {
        if (target == null) return;
        Vector2 targetPos = target.EntityPos;
        Vector2 currentPos = transform.position;

        if (Vector2.Distance(targetPos, currentPos) <= collectDis)
        {
            target.AddExp(expdata.expAmount);
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(currentPos, targetPos, expdata.speed * Time.deltaTime);
    }

}
