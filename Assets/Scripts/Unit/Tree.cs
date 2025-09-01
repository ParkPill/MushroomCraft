using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : BuildingBase
{
    [SerializeField] private int initialLumber = 200;
    [SerializeField] private int CurrentLumber;
    float _giveTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        CurrentLumber = initialLumber;

        Model.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Obstacle/tree" + Random.Range(0, 7));
        float range = 0.15f;
        Model.transform.localPosition = new Vector3(Random.Range(-range, range), Random.Range(-range, range) - 0.5f, 0);
    }

    public int DigLumber(float dt)
    {
        _giveTimer += dt;
        if (_giveTimer >= Consts.LumberPerTreeGiveTime)
        {
            _giveTimer -= Consts.LumberPerTreeGiveTime;

            int lumber = Consts.LumberPerOnce;
            if (lumber > CurrentLumber)
            {
                lumber = CurrentLumber;
            }
            CurrentLumber -= lumber;
            return lumber;
        }
        return 0;
    }
}
