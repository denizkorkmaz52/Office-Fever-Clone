using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public GameObject ps1;
    public GameObject ps2;
    public GameObject ps3;
    private List<GameObject> psList = new List<GameObject>();

    public GameObject worker1;
    public GameObject worker2;
    public GameObject worker3;
    public GameObject worker4;
    private List<GameObject> workerList = new List<GameObject>();

    public TextMeshProUGUI moneyText;
    private Player player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        psList.Add(ps1);
        psList.Add(ps2);
        psList.Add(ps3);

        workerList.Add(worker1);
        workerList.Add(worker2);
        workerList.Add(worker3);
        workerList.Add(worker4);
    }
    public void RefreshCanvas(int money)
    {
        moneyText.text = money + " $";
    }
    public GameObject ChoosePrinterSide()
    {
        GameObject returnObj = null;
        for (int i = 0; i < psList.Count; i++)
        {
            if (returnObj == null && psList[i].GetComponent<PrinterSide>().isActive)
            {
                returnObj = psList[i];
            }
            else if (returnObj != null && psList[i].GetComponent<PrinterSide>().isActive && returnObj.GetComponent<PrinterSide>().ShowPaperCount() < psList[i].GetComponent<PrinterSide>().ShowPaperCount())
            {
                returnObj = psList[i];
            }
        }
        return returnObj;
    }

    public GameObject ChooseWorker()
    {
        GameObject returnObj = null;
        for (int i = 0; i < workerList.Count; i++)
        {
            if (returnObj == null && workerList[i].GetComponent<Worker>().isActive)
            {
                returnObj = workerList[i];
            }
            else if (returnObj != null && workerList[i].GetComponent<Worker>().isActive && returnObj.GetComponent<Worker>().ShowPaperCount() > workerList[i].GetComponent<Worker>().ShowPaperCount())
            {
                returnObj = workerList[i];
            }
        }
        return returnObj;
    }
}
