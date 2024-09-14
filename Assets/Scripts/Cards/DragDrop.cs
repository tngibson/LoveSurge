using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    [SerializeField] Dropzone DropZone;
    [SerializeField] Card card;
    [SerializeField] PlayerArea playerArea;
    private bool isDragging = false;
    private GameObject startParent;
    private Vector2 startPos;
    private GameObject dropZone;
    private bool isOverDropZone;
    
    void Start()
    {
        dropZone = GameObject.Find("Dropzone");
        DropZone = dropZone.GetComponent<Dropzone>();
        playerArea = GameObject.Find("PlayerArea").GetComponent<PlayerArea>();
    }

    void Update()
    {
     if (isDragging)
        {
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y,0);
        }   
    }
    public void startDrag()
    {
        //Debug.Log("startdrag");
        isDragging = true;
        startParent = transform.parent.gameObject;
        startPos = transform.position;


    }
    public void endDrag() 
    {
        isDragging = false;
        if (isOverDropZone)
        {
            if (DropZone == null) { print("DropZone is null"); }
            transform.SetParent(dropZone.transform, false);
            DropZone.AddCard(GetComponent<Card>());
            this.GetComponent<GridElementSwapper>().setFirstSelectedElement(null);
            playerArea = null;
        }
        else
        {
            transform.position = startPos;
            transform.SetParent(startParent.transform, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isOverDropZone= false;
        dropZone = null;
    }

    public bool getIsDragging()
    {
        return isDragging;
    }

    public PlayerArea getPlayerArea()
    {
        return playerArea;
    }

    public Dropzone getDropzone()
    {
        return DropZone;
    }
}
