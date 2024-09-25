using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsSpawner :MonoBehaviour
{
    public GameObject mob;
    void StartT()
    {
        GameObject a=Instantiate(mob,new Vector3(0,74,0),Quaternion.identity);
        a.transform.localScale = new Vector3(3, 3, 3);
    }

  
}
