using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Hook : MonoBehaviour
{
    public Sprite sprite;

    public void Set(Transform target, Vector2 position, ObjectReturn objectReturn)
    {
        transform.localScale = target.position.x > transform.position.x ? Vector3.one : new Vector3(1,-1,1);
        StartCoroutine(Return(target, position, objectReturn));
    }

    private IEnumerator Return(Transform target, Vector2 position, ObjectReturn objectReturn)
    {
        Vector2 temp = transform.position;
        var sr = GetComponent<SpriteRenderer>();
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;

            transform.position = Vector2.Lerp(temp, target.position, t);
            Vector2 directionVector = ((Vector2)target.position - temp).normalized;
            transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg);
            yield return null;
        }
        temp = transform.position;
        var coll = target.GetComponent<Collider2D>();
        coll.enabled = false;
        sr.sprite = sprite;

        while (t > 0)
        {
            t -= Time.deltaTime;

            Vector2 pos = Vector2.Lerp(position, temp, t);
            transform.position = pos;
            target.position = pos;
            Vector2 directionVector = (temp - position).normalized;
            transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg);
            yield return null;
        }

        objectReturn.Targets.Add(target);
        coll.enabled = true;

        while (t < 1)
        {
            t += Time.deltaTime * 5;

            sr.color = new Color(1,1,1,1 - t);
            yield return null;
        }

        Destroy(gameObject);
    }
}