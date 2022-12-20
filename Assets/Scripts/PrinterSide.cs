using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PrinterSide : MonoBehaviour
{
    [SerializeField] private int paperCount = 0;
    private float produceTime = 0.6f;
    private int paperLimit = 105;
    private int paperStackLimit = 15;
    private int paperStackCountLimit = 5;
    //private float paperSpeed = 1.0f;

    public GameObject paperPrefab;
    private Transform printerLocation;
    private GameObject paperSide;
    private GameObject paperStack;
    //[SerializeField]private bool[][] stacks = new bool[5][];
    private Vector3 stackOffset = new Vector3(0, 0, 0.25f);
    private Vector3 heightOffset = new Vector3(0, 0.004f, 0);
    private Vector3 stackOffset2 = new Vector3(0.2f, 0, 0);
    private Vector3 startPos;
    //private int[] index = new int[2];
    //private int previousIndex = 0;
    public List<Paper> paperList = new List<Paper>();
    public bool isActive = false;
    private float waitingToUseMoney = 0.2f;

    public GameObject printerLocationGameObj;
    public Canvas canvas;
    public Image loading;
    public float buyCost = 400;
    private float baseBuyCost;
    private Coroutine waitingRoutine;

    void Start()
    {
        baseBuyCost = buyCost;
        if (isActive)
        {
            printerLocation = transform.Find("PrinterLocation");
            paperSide = transform.Find("PaperSide").gameObject;
            paperStack = transform.Find("StackPaper").gameObject;
            startPos = paperStack.transform.position;
            StartCoroutine(ProducePaper());
        }
        
    }

    private IEnumerator ProducePaper()
    {
        while (true)
        {
            if (paperCount < paperLimit)
            {
        
                //index = IsCapacityFull();

                    GameObject paper = Instantiate(paperPrefab, printerLocation.position, Quaternion.identity);
                    var paperScript = paper.GetComponent<Paper>();
                    paperScript.StartMoving(paperStack, paperSide);
                    

                //AddPaperToField(paper);
                          
                yield return new WaitForSeconds(produceTime);
            }else
                yield return null;
        }
        
    }

   /* private void AddPaperToField(GameObject paper)
    {
        Debug.Log("addfield");
        
    }*/
    public void AddList(Paper paper)
    {
        paperList.Add(paper);
        paperCount++;
        if ((paperCount != 0) && (paperCount % paperStackLimit == 0))
        {
            paperStack.transform.position = paperStack.transform.position + stackOffset - (paperStackLimit - 1) * heightOffset;
            //paperStack.transform.position.Set(paperStack.transform.position.x, startHeight, paperStack.transform.position.z);
            //previousIndex = index[0];
        }
        else if ((paperCount == 0) || (paperCount % paperStackLimit != 0))
        {
            paperStack.transform.position = paperStack.transform.position + heightOffset;
        }
        if ((paperCount % paperStackLimit == 0) && paperCount/paperStackLimit >= paperStackCountLimit)
        {
            paperStack.transform.position = startPos + stackOffset2;
        }
        
    }
    public Paper TakePaper(GameObject stackPaper, GameObject parent)
    {
        Paper paper;
        if (paperCount > 0)
        {
            paperList[paperCount-1].StartMoving(stackPaper, parent);
            paper = paperList[paperCount - 1];
            paperList.RemoveAt(paperCount -1);
            paperCount--;
            if ((paperCount % paperStackLimit == paperStackLimit - 1))
            {
                paperStack.transform.position = paperStack.transform.position - stackOffset + (paperStackLimit - 1) * heightOffset;
                //paperStack.transform.position.Set(paperStack.transform.position.x, startHeight, paperStack.transform.position.z);
                //previousIndex = index[0];
            }
            else if ((paperCount % paperStackLimit != paperStackLimit - 1))
            {
                paperStack.transform.position = paperStack.transform.position - heightOffset;
            }

        } else
            paper = null;

        return paper;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            ActivatePrinter(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (waitingRoutine != null)
        {
            StopCoroutine(waitingRoutine);
        }
    }
    public void ActivatePrinter(Collider player)
    {
        waitingRoutine = StartCoroutine(WaitingToValidate(player));
    }
    private IEnumerator WaitingToValidate(Collider player)
    {
        
        while (buyCost > 0){
            
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

        printerLocation = transform.Find("PrinterLocation");
        paperSide = transform.Find("PaperSide").gameObject;
        paperStack = transform.Find("StackPaper").gameObject;
        paperSide.SetActive(true);
        paperStack.SetActive(true);
        printerLocationGameObj.SetActive(true);
        canvas.enabled = false;
        isActive = true;
        StartCoroutine(ProducePaper());
    }
    public int ShowPaperCount()
    {
        if (isActive)
        {
            return paperCount;
        }
        else
        {
            return 999999;
        }
        
    }
}
