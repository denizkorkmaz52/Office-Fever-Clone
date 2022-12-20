using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    private GameObject stackPaper;
    private GameObject stack;
    public GameObject moneyPrefab;
    private GameManager gameManager;
    [SerializeField] private int paperCount;
    //private int paperCapacityLevel = 1;
    private int paperCapacity = 50;
    private int paperStackCapacity = 25;
    public bool[][] stacks = new bool[2][];
    List<Paper> paperList = new List<Paper>();
    [SerializeField] private int totalMoney;

    [SerializeField]private bool canCollect = true;
    [SerializeField]private bool waitTimeOver = true;
    private bool canGivePaper = true;
    [SerializeField] private float paperCollectTime = 0.3f;
    //[SerializeField] private int paperCollectSpeedLevel = 1;
    private Vector3 stackOffset = new Vector3(0.22f, 0, 0);
    private Vector3 heightOffset = new Vector3(0, 0.006f, 0);
    //private int previousIndex = 0;

    /*bool isNextStack = false;

    int[] index = new int[2];*/
    private void Awake()
    {
        
        animator = GetComponentInChildren<Animator>();
        stackPaper = GameObject.FindGameObjectWithTag("StackPaper");
        stack = transform.Find("Stack").gameObject;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        paperCount = 0;
        totalMoney = 0;
        InitializeAnimParameters();
    }


    private void OnTriggerStay(Collider other)
    {
        if (paperCount >= paperCapacity)
        {
            canCollect = false;
        }
        else
        {
            canCollect = true;
        }
        if (other.CompareTag("TakePaperSide") && canCollect && waitTimeOver)
        {
            PrinterSide printer = other.gameObject.GetComponentInParent<PrinterSide>();

            //index = IsCapacityFull();

            if (paperCount < paperCapacity)
            {
                //Debug.Log(stackPaper.transform.position.x + "----" + stackPaper.transform.position.y + "---" + stackPaper.transform.position.z);
                Paper takenPaper = printer.TakePaper(stackPaper, stack);
                if (takenPaper != null)
                {              
                    StartCoroutine(WaitToTakePaper());
                }
            }

        }
        else if (other.CompareTag("GivePaperSide"))
        {

            if (paperCount > 0 && canGivePaper)
            {
                Worker worker = other.transform.parent.GetComponent<Worker>();
                GameObject parent = worker.stackPaperParent;
                GameObject stack = worker.stackPaper;
                paperList[paperCount - 1].StartMoving(stack, parent);
                paperList.RemoveAt(paperCount - 1);
                paperCount--;
                if ((paperCount % paperStackCapacity == paperStackCapacity - 1))
                {
                    //Debug.Log("1");
                    stackPaper.transform.localPosition = stackPaper.transform.localPosition - stackOffset + (paperStackCapacity - 1) * heightOffset;
                    //previousIndex = index[0];
                }
                else if ((paperCount % paperStackCapacity != paperStackCapacity - 1))
                {
                    //Debug.Log("2");
                    stackPaper.transform.localPosition = stackPaper.transform.localPosition - heightOffset;
                }
                StartCoroutine(WaitToGivePaper());
            }

        }
        else if (other.CompareTag("StackMoney"))
        {
            Worker script = other.transform.parent.GetComponent<Worker>();
            totalMoney += script.CollectMoney(transform.gameObject);
            gameManager.RefreshCanvas(totalMoney);
        }
        
    }
    public void HaveArrived(Paper paper)
    {;
        paperList.Add(paper);
        paper.SetPosition(stackPaper);
        paperCount++;
        if ((paperCount != 0) && (paperCount % paperStackCapacity == 0))
        {
            stackPaper.transform.localPosition = stackPaper.transform.localPosition + stackOffset - (paperStackCapacity - 1) * heightOffset;
        }
        else if ((paperCount == 0) || (paperCount % paperStackCapacity != 0))
        {
            stackPaper.transform.localPosition = stackPaper.transform.localPosition + heightOffset;
        }
    }
    public int ShowMoney()
    {
        return totalMoney;
    }
    public int UseMoney(GameObject buyObject)
    {
        if (totalMoney > 0)
        {
            Paper money = Instantiate(moneyPrefab, transform.position + new Vector3(0, 0.58f, 0), transform.rotation).GetComponent<Paper>(); ;
            money.StartMoving(buyObject, null);
            if (totalMoney >= 100)
            {
                totalMoney -= 100;
                gameManager.RefreshCanvas(totalMoney);
                return 100;
            }
            else
            {
                int sentMoney = totalMoney;
                totalMoney = 0;
                gameManager.RefreshCanvas(totalMoney);
                return sentMoney;
            }
        }
        else
            return 0;
        
    }
    public int ShowPaperCount()
    {
        return paperCount;
    }
    private IEnumerator WaitToTakePaper()
    {
        waitTimeOver = false;
        yield return new WaitForSeconds(paperCollectTime);
        waitTimeOver = true;
    }
    private IEnumerator WaitToGivePaper()
    {
        canGivePaper = false;
        yield return new WaitForSeconds(paperCollectTime);
        canGivePaper = true;
    }


    public void SetAnimVar(string key, bool value)
    {
        //Debug.Log(key + "---" + value.ToString());
        animator.SetBool(key, value);
    }
    public void SetAnimVar(string key, float value)
    {
        animator.SetFloat(key, value);
    }
    /*public void SetAnimVar(string key, int value)
    {

    }*/

    private void InitializeAnimParameters()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", false);
        animator.SetBool("Carrying", false);
        //animator.SetBool("RunningTurnleft", false);
        //animator.SetBool("RunningTurnRight", false);
        //animator.SetBool("RunningTurn180", false);
    }
}
