using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackPaper : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

 
}
