using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int Damage;
    public float Speed;
    public Transform Target;
    public Transform Model;
    public UnitBase Shooter;
    // Start is called before the first frame update
    void Start()
    {
        Model = transform.Find("Model");
    }

    // Update is called once per frame
    void Update()
    {
        if (Target)
        {
            // 2D 게임이므로 z축만 회전(위쪽이 0도)
            Vector3 dir = Target.position - Model.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Model.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
            if (transform.position == Target.position)
            {
                Target.GetComponent<UnitBase>().TakeDamage(Damage, Shooter);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform target, int damage, UnitBase shooter, float speed = 0)
    {
        Target = target;
        Damage = damage;
        if (speed > 0) Speed = speed;
        Shooter = shooter;
    }
}
