using System;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentInformation : MonoBehaviour
{
    public List<GameObject> Attached; //things that this block attaches
    public UnityEngine.Vector2 _CurrentPosition;
    public GameObject[,] Grid;
    public GameObject GridObject;

    // Start is called before the first frame update
    void Start()
    {
        GridObject=GameObject.Find("Grid");
        Grid=GridObject.GetComponent<GridManager>().Grid;
        _CurrentPosition=new UnityEngine.Vector2(this.transform.position.x+5.5f,-(this.transform.position.y-3));
    }

    public Boolean CheckAttachmentMovable(int x, int y){
        GameObject Attach=Grid[x,y];
        switch (Attach.gameObject.tag){
            case "Wall":
                return false;

            case "Player":
                return false;

            case "Smooth":
                return false;
        }
        return true;
    }
    public void CheckAttachments(){
        int x=(int)_CurrentPosition.x-1;
        int y=(int)_CurrentPosition.y-1;

        if (x>0 && Grid[x-1,y]!=null){
            int a=x-1;
            int b=y;
            if (CheckAttachmentMovable(a,b)){
                Attached.Add(Grid[a,b]);
            }
        }

        if (x<9 && Grid[x+1,y]!=null){
            int a=x+1;
            int b=y;
            if (CheckAttachmentMovable(a,b)){
                Attached.Add(Grid[a,b]);
            }
        }

        if (y>0 && Grid[x,y-1]!=null){
            int a=x;
            int b=y-1;
            if (CheckAttachmentMovable(a,b)){
                Attached.Add(Grid[a,b]);
            }
        }

        if (y<4 && Grid[x,y+1]!=null){
            int a=x;
            int b=y+1;
            if (CheckAttachmentMovable(a,b)){
                Attached.Add(Grid[a,b]);
            }
        }
    }

    public string CheckAttachmentPosition(GameObject obj, UnityEngine.Vector2 _TCurrentPosition){
        float ax=_CurrentPosition.x;
        float ay=_CurrentPosition.y;
        float x=_TCurrentPosition.x;
        float y=_TCurrentPosition.y;
        Debug.Log(ax+","+ay+"|"+x+","+y);
        if (ax>x){
            return "R";
        }
        if (ax<x){
            return "L";
        }
        if (ay>y){
            return "D";
        }
        if (ay<y){
            return "U";
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        Grid=GridObject.GetComponent<GridManager>().Grid;
    }
}
