using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRange : MonoBehaviour
{
    bool isPlayerInRange = false;
    public Transform player;
    public EnemyAround enemyAround;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == player)
        {
            isPlayerInRange = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange == true)
        {
            enemyAround.DiscoverPlayer();
        }
    }
}
