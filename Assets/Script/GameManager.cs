using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public const int MachingCount = 3;
    private PieceManager selectedPiecce;
    [SerializeField]
    private Board  board;


    // Use this for initialization
    void Start () {
        board.InitializeBoard(6, 5);
        Debug.Log("Start!!");
        StartCoroutine(Idling());
	}
	
	// Update is called once per frame
	void Update () {
        
         

	}

    public IEnumerator Idling()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedPiecce = board.GetNearestPiece(Input.mousePosition);

                StartCoroutine(PieceMove());
                break;

            }
            Debug.Log("待機");
            yield return null;
        }

    }

    public IEnumerator PieceMove()
    {
        while (true)
        {
            Debug.Log("PieceMove");
            if (Input.GetMouseButton(0))
            {
                var piece = board.GetNearestPiece(Input.mousePosition);
                if (piece != selectedPiecce)
                {
                    board.SwitchPiece(selectedPiecce, piece);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(MatchCheck());
                
            }
            yield return null;
        }
    }

    public IEnumerator MatchCheck()
    {
        while (true)
        {
            if (board.HasMatch())
            {
                StartCoroutine(DeletePiece());
                
            }
            else
            {
                StartCoroutine(Idling());
                
            }
            yield return null;
        }
    }
    public IEnumerator DeletePiece()
    {
       
        board.DeleteMarchPiece();
        StartCoroutine(FillPiece());
        yield break;
        
    }
    public IEnumerator FillPiece()
    {
        board.FillPiece();
        StartCoroutine(MatchCheck());
        yield break;
    }
}
