using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Paper : MonoBehaviour
{
    GameObject moveToPosition;
    GameObject parent;
    GameObject target;
    public float moveTime;
    public void StartMoving(GameObject destination, GameObject parent)
    {
        //Debug.Log("startmoving");
        moveToPosition = destination;
        StartCoroutine(MovePaper());
        this.parent = parent;
    }
    private IEnumerator MovePaper()
    {
        var elapsedTime = 0f;
        float moveTime;
        var startPos = transform.position;
        if (transform.CompareTag("Money"))
            moveTime = 0.1f;
        else
            moveTime = 0.2f;
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, moveToPosition.transform.position, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, moveToPosition.position, 1f * Time.deltaTime);
            yield return null;
        }
        if (parent != null)
        {
            if (moveToPosition.CompareTag("StackPaper"))
            {
                if (moveToPosition.transform.parent.CompareTag("Player"))
                {
                    target = GameObject.FindGameObjectWithTag("Player");
                    target.GetComponent<Player>().HaveArrived(this);
                }
                else if (moveToPosition.transform.parent.CompareTag("WalkingWorker"))
                {
                    target = moveToPosition.transform.parent.gameObject;
                    target.GetComponent<WalkingWorker>().HaveArrived(this);
                }
                
            }
            else if(parent.transform.parent.CompareTag("PrinterSide"))
            {
                target = parent.transform.parent.gameObject;
                target.GetComponent<PrinterSide>().AddList(this);
            }
            else if (moveToPosition.transform.parent.name == "StackPaper")
            {
                target = parent.transform.parent.gameObject;
                target.GetComponent<Worker>().AddPaperToList(this);
            }
            transform.SetParent(parent.transform);

        }
        else
        {
            DestroyGameObject();
        }

    }

    public void SetPosition(GameObject position)
    {
        transform.position = position.transform.position;
        transform.rotation = position.transform.localRotation;
    }
    public void DestroyGameObject()
    {
        Destroy(transform.gameObject);
    }
}
