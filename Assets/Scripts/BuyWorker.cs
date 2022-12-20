using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyWorker : MonoBehaviour
{
    [SerializeField]private float buyCost = 200;
    private float baseBuyCost;
    public GameObject worker;
    public Image loading;
    private float waitingToUseMoney = 0.2f;
    Coroutine waitingRoutine;

    private void Awake()
    {
        baseBuyCost = buyCost;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            waitingRoutine = StartCoroutine(Buying(other.GetComponent<Player>()));
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && waitingRoutine != null)
        {
            StopCoroutine(waitingRoutine);
        }    
    }
    private IEnumerator Buying(Player playerSc)
    {
        while (buyCost > 0)
        {
            
            yield return new WaitForSeconds(waitingToUseMoney);
            int sentMoney = playerSc.UseMoney(transform.gameObject);
            if (sentMoney != 0)
            {
                buyCost -= sentMoney;
            }
            else
                StopCoroutine(waitingRoutine);
            loading.fillAmount = 1f - (buyCost / baseBuyCost);
        }
        if (buyCost == 0)
        {
            Instantiate(worker, transform.position, Quaternion.identity);
            buyCost = baseBuyCost;
            loading.fillAmount = 0;
        }
        

    }
}
