using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingWorker : MonoBehaviour
{
    private int paperCount;
    private int paperCapacity = 10;
    private int paperStackCapacity = 10;
    [SerializeField] private float paperCollectTime = 0.3f;
    //[SerializeField] private int paperCollectSpeedLevel = 1;
    private Vector3 stackOffset = new Vector3(0.20f, 0, 0);
    private Vector3 heightOffset = new Vector3(0, 0.0044f, 0);
    List<Paper> paperList = new List<Paper>();
    private bool canCollect = true;
    private bool waitTimeOver = true;
    private bool canGivePaper = true;
    private bool isStopped = false;
    private GameObject stackPaper;
    private GameObject stack;
    GameManager gameManager;
    GameObject target;
    NavMeshAgent agent;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        target = gameManager.ChoosePrinterSide();
        isStopped = false;
        stackPaper = transform.Find("StackPaper").gameObject;
        stack = transform.Find("Stack").gameObject;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        InitializeAnimParameters();
        paperCount = 0;
        agent.SetDestination(target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        ControlAnim();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TakePaperSide") || other.CompareTag("GivePaperSide"))
        {
            isStopped = true;
            agent.isStopped = true;
            if (paperCount >= paperCapacity)
            {
                canCollect = false;
                target = gameManager.ChooseWorker();
                agent.SetDestination(target.transform.position);
                bool istrue = agent.SetDestination(target.transform.position);
                isStopped = false;
                Debug.Log("1" + istrue);
            }
            else
            {
                canCollect = true;
            }
            if (other.CompareTag("TakePaperSide") && canCollect && waitTimeOver)
            {
                isStopped = true;
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
                else if (paperCount == 0)
                {
                    target = gameManager.ChoosePrinterSide();
                    bool istrue = agent.SetDestination(target.transform.position);
                    isStopped = false;
                    Debug.Log("2" + istrue);
                }

            }
            agent.isStopped = false;
        }
        
    }
    public void HaveArrived(Paper paper)
    {
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
    private void ControlAnim()
    {
        if (isStopped)
        { 
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", true);
            if (paperCount > 0)
            {
                animator.SetBool("Carrying", true);
            }
            else
            {
                animator.SetBool("Carrying", false);
            }
        }
        else
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walking", true);
            if (paperCount > 0)
            {
                animator.SetBool("Carrying", true);
            }
            else
            {
                animator.SetBool("Carrying", false);
            }
        }
    }
    private void InitializeAnimParameters()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", false);
        //animator.SetBool("RunningTurnleft", false);
        //animator.SetBool("RunningTurnRight", false);
        //animator.SetBool("RunningTurn180", false);
    }
}
