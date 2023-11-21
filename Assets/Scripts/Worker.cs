using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Worker : MonoBehaviour
{
    List<Paper> paperList = new List<Paper>();
    [SerializeField]List<Paper> moneyList = new List<Paper>();
    private bool canProduceMoney = true;
    Coroutine waitingRoutine;
    float waitingToUseMoney = 0.2f;
    public float buyCost = 3000;
    private float baseBuyCost;
    public bool isActive = false;

    private Vector3 heightOffset = new Vector3(0, 0.0044f, 0);

    public GameObject stackPaper;
    public GameObject stackMoney;
    public GameObject money;

    public GameObject laptop;
    public GameObject chair;
    public GameObject table;
    public GameObject givePaper;
    public GameObject stackPaperParent;
    public GameObject stackMoneyParent;
    public GameObject worker;
    public Image loading;
    public Canvas canvas;

    private void Awake()
    {
        baseBuyCost = buyCost;
    }
    // Update is called once per frame
    private void Update()
    {
        if (paperList.Count > 0 && canProduceMoney)
        {
            canProduceMoney = false;
            StartCoroutine(ProduceMoney());
        }
    }

    private IEnumerator ProduceMoney()
    {
        while (paperList.Count > 0)
        {      
            Paper paperToRemove = paperList[paperList.Count - 1];
            paperList.RemoveAt(paperList.Count - 1);
            paperToRemove.DestroyGameObject();                 
            GameObject moneyPaper = Instantiate(money, stackMoney.transform.position, stackMoney.transform.rotation);
            moneyPaper.transform.parent = transform.Find("StackMoney");
            moneyList.Add(moneyPaper.GetComponent<Paper>());
            stackMoney.transform.position = stackMoney.transform.position + heightOffset;
            stackPaper.transform.position = stackPaper.transform.position - heightOffset;
            yield return new WaitForSeconds(0.8f);
        }
        canProduceMoney = true;
    }
    public void AddPaperToList(Paper paper)
    {
        paperList.Add(paper);
        paper.SetPosition(stackPaper);
        stackPaper.transform.position = stackPaper.transform.position + heightOffset;

    }


    public int CollectMoney(GameObject player) 
    {
        int totalMoney = 0;
        for (int i = 0; i < moneyList.Count; i++)
        {
            moneyList[i].StartMoving(player, null);
            stackMoney.transform.position = stackMoney.transform.position - heightOffset;
            totalMoney += 50;
        }
        moneyList.Clear();

        return totalMoney;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {

            ActivateTable(other);
    
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (waitingRoutine != null)
        {
            StopCoroutine(waitingRoutine);
        }   
    }
    public void ActivateTable(Collider player)
    {
        waitingRoutine = StartCoroutine(WaitingToValidate(player)); 
    }
    private IEnumerator WaitingToValidate(Collider player)
    {
        while (buyCost > 0)
        {
            
            yield return new WaitForSeconds(waitingToUseMoney);
            int sentMoney = player.GetComponent<Player>().UseMoney(transform.gameObject);
            if (sentMoney != 0)
            {
                buyCost -= sentMoney;
            }
            else
                StopCoroutine(waitingRoutine);
            Debug.Log(buyCost / baseBuyCost);
            loading.fillAmount = 1f - (buyCost / baseBuyCost);
        }
        laptop.SetActive(true);
        chair.SetActive(true);
        table.SetActive(true);
        givePaper.SetActive(true);
        stackPaperParent.SetActive(true);
        stackMoneyParent.SetActive(true);
        stackPaper.SetActive(true);
        stackMoney.SetActive(true);
        worker.SetActive(true);
        canvas.enabled = false;
        GetComponent<Collider>().enabled = false;
        isActive = true;
    }

    public int ShowPaperCount()
    {
        if (isActive)
        {
            return paperList.Count;
        }
        else
        {
            return 999999;
        }
       
    }
    //Destroy paper !!!!!!!!!!!!!!!
}
