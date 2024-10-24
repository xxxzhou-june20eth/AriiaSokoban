
using System;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public string Tag;
    public UnityEngine.Vector2 _CurrentPosition;
    public UnityEngine.Vector2 _NextPosition;
    public UnityEngine.Vector2 _PreviousPosition;
    public GameObject[,] Grid;
    public GameObject GridObject;
    public Boolean Movable;
    public Boolean NextPositionMovable(string Direction, UnityEngine.Vector2 _CurrentPosition){ //check if new position exceeds the grid
        switch (Direction){ //Set the new grid position based on the movement directed
            case "L":
                if (_CurrentPosition.x>1){
                    _NextPosition= new UnityEngine.Vector2 (_CurrentPosition.x-1, _CurrentPosition.y);
                }
                else{
                    Debug.Log(this.name+" L stops");
                    return false;
                }
                break;

            case "R":
                if (_CurrentPosition.x<10){
                    _NextPosition= new UnityEngine.Vector2 (_CurrentPosition.x+1, _CurrentPosition.y);
                }
                else{
                    Debug.Log(this.name+" R stops");
                    return false;
                }
                break;

            case "U":
                if (_CurrentPosition.y>1){
                    _NextPosition= new UnityEngine.Vector2 (_CurrentPosition.x, _CurrentPosition.y-1);
                }
                else{
                    Debug.Log(this.name+" U stops");
                    return false;
                }
                break;

            case "D":
                if (_CurrentPosition.y<10){
                    _NextPosition= new UnityEngine.Vector2 (_CurrentPosition.x, _CurrentPosition.y+1);
                }
                else{
                    Debug.Log(this.name+" D stops");
                    return false;
                }
                break;
        }
        return true;
    }

        public Boolean NewPositionOccupied(UnityEngine.Vector2 _NextPosition){ //check if new position is occupied by another block
        GameObject _Occupying = Grid[(int)_NextPosition.x-1,(int)_NextPosition.y-1]; 
        if (_Occupying!=null){ //if new grid position is not empty
            return true;
        }
        return false;
    }

    public Boolean NewPositionOccupantMovable(string Direction, UnityEngine.Vector2 _NextPosition){ //check if occupant is movable
        GameObject _Occupying = Grid[(int)_NextPosition.x-1,(int)_NextPosition.y-1]; 
        switch (_Occupying.gameObject.tag){ //then check if it's...
                case "Wall": //Wall -- Unmovable
                    _NextPosition=_CurrentPosition;
                    Debug.Log(this.name+" wall stops");
                    return false;

                case "Clingy": //Clingy -- Unmovable
                    _NextPosition=_CurrentPosition;
                    Debug.Log(this.name+" clingy stops");
                    return false;

                case "Smooth": //Smooth -- Push Smooth in the same direction
                    // if (_Occupying.GetComponent<Movement>().NextPositionMovable(Direction,_Occupying.GetComponent<Movement>()._CurrentPosition)){
                    //     _Occupying.GetComponent<Movement>().Move(Direction);
                    //     return true;
                    // }
                    // else{
                    //     Debug.Log(this.name+"smooth stops");
                    //     return false;
                    // }

                case "Sticky": //Sticky -- Push Sticky in the same direction
                    // if (_Occupying.GetComponent<Movement>().NextPositionMovable(Direction,_Occupying.GetComponent<Movement>()._CurrentPosition)){
                    //     _Occupying.GetComponent<Movement>().Move(Direction);
                    //     return true;
                    // }
                    // else{
                    //     Debug.Log(this.name+"sticky stops");
                    //     return false;
                    // }
                break;
        }
        return true;
    }

    public Boolean MoveTo(string Direction, UnityEngine.Vector2 _CurrentPosition){ //check if movable to the new position
        if (NextPositionMovable(Direction, _CurrentPosition)){ //if not exceeding the grid
            if (NewPositionOccupied(_NextPosition)){ //if new position is occupied
                if (NewPositionOccupantMovable(Direction, _NextPosition)){ //if occupant can move and not exceeding the grid
                    GameObject _Occupying = Grid[(int)_NextPosition.x-1,(int)_NextPosition.y-1]; //grab info of occupant
                    Boolean OccupantMoved=_Occupying.GetComponent<Movement>().MoveTo(Direction,_Occupying.GetComponent<Movement>()._CurrentPosition); //check if occupant can move
                    if (OccupantMoved){ //if occupant can move
                        _Occupying.GetComponent<Movement>().Move(Direction);
                        return true; //move this
                    }
                    else{ //if occupant cannot move then dont move this
                        return false; 
                    }
                }
                else{ //if ovvupant cannot move or exceeding the grid then dont move this
                    return false; 
                }
            }
            else{ //if new position is not occupied then move this
                return true;
            }
        }
        else{ //if exceeding the grid then dont move
            return false;
        }
    }

    public void AttachmentMovement(string Direction){
        List<GameObject> AttachmentList=this.GetComponent<AttachmentInformation>().Attached;
            foreach (GameObject obj in AttachmentList){
                Debug.Log("Attachment:"+obj.name);
                switch (obj.gameObject.tag){
                    case "Sticky":
                        string poss=obj.GetComponent<AttachmentInformation>().CheckAttachmentPosition(obj,_PreviousPosition);
                        if (poss!=Direction){
                            obj.GetComponent<Movement>().Move(Direction);
                        }
                        break;

                    case "Clingy":
                        string posc=obj.GetComponent<AttachmentInformation>().CheckAttachmentPosition(obj,_PreviousPosition);
                        Debug.Log(Direction+posc);
                        switch (Direction){
                            case "L":
                                if (posc=="R"){
                                    obj.GetComponent<Movement>().Move(Direction);
                                }
                                break;

                            case "R":
                                if (posc=="L"){
                                    obj.GetComponent<Movement>().Move(Direction);
                                }
                                break;

                            case "U":
                                if (posc=="D"){
                                    obj.GetComponent<Movement>().Move(Direction);
                                }
                                break;

                            case "D":
                                if (posc=="U"){
                                    obj.GetComponent<Movement>().Move(Direction);
                                }
                                break;
                        }
                        break;
                }
            }
    }

    public void Move(string Direction){ //move the block
        Debug.Log(this.name+"get input to move to "+Direction);
        Boolean CanMove=MoveTo(Direction,_CurrentPosition);
        if (CanMove){ //if movable then move the block
            
            //update the coordinate
            float x = GridMaker.reference.TopLeft.x + GridMaker.reference.cellWidth * (_NextPosition.x - 0.5f); 
            float y = GridMaker.reference.TopLeft.y - GridMaker.reference.cellWidth * (_NextPosition.y - 0.5f);
            this.transform.position = new UnityEngine.Vector3(x, y, 0);  //move to the new position

            //update the grid
            GridObject.GetComponent<GridManager>().Grid[(int)_CurrentPosition.x-1,(int)_CurrentPosition.y-1]=null; //delete self from current position grid
            GridObject.GetComponent<GridManager>().Grid[(int)_NextPosition.x-1,(int)_NextPosition.y-1]=this.gameObject; //add self to new position grid
            Grid=GridObject.GetComponent<GridManager>().Grid; //update the grid info every frame

            //update the position
            _PreviousPosition=_CurrentPosition;
            _CurrentPosition=_NextPosition; //update position
            this.GetComponent<AttachmentInformation>()._CurrentPosition=_CurrentPosition; //update the attachment grid pos
        }
        
        if (this.GetComponent<AttachmentInformation>().Attached.Count>0){ //if the block have attachment then move attachment first
                AttachmentMovement(Direction);
        }

        //update the attachments
        this.GetComponent<AttachmentInformation>().Attached.Clear();
        this.GetComponent<AttachmentInformation>().CheckAttachments();
    }

    // Start is called before the first frame update
    void Start()
    {
        Tag=this.gameObject.tag;
        _CurrentPosition=new UnityEngine.Vector2(this.transform.position.x+5.5f,-(this.transform.position.y-3));
        //initialize the grid
        GridObject=GameObject.Find("Grid");
        Grid=GridObject.GetComponent<GridManager>().Grid;
        GridObject.GetComponent<GridManager>().Grid[(int)_CurrentPosition.x-1,(int)_CurrentPosition.y-1]=this.gameObject;

        //initialize the attachments
        this.GetComponent<AttachmentInformation>().Attached.Clear();
        this.GetComponent<AttachmentInformation>().CheckAttachments();
    }

    // Update is called once per frame
    void Update()
    {
       //update the grid everyframe
        Grid=GridObject.GetComponent<GridManager>().Grid;
        this.GetComponent<AttachmentInformation>().Attached.Clear();
        this.GetComponent<AttachmentInformation>().CheckAttachments();
    }
}

