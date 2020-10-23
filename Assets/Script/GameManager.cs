using UnityEngine;


public class GameManager : MonoBehaviour {

    public const int MachingCount = 3;
    private PieceManager selectedPiecce;
    [SerializeField]
    private Board  board;

    public enum GameState
    {
        Idle,
        PieceMove,
        MatchCheck,
        DeletePiece,
        FillPiece,
		Wait,
    }

    private GameState state;

    // Use this for initialization
    void Start () {
        board.InitializeBoard(6, 5);
        Debug.Log("Start!!");
        state = GameState.Idle;
	}
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case GameState.Idle:
                Idling();
                break;
			case GameState.PieceMove:
				PieceMove();
				break;
            case GameState.MatchCheck:
                MatchCheck();
                break;
			case GameState.DeletePiece:
				DeletePiece();
				break;
			case GameState.Wait:
				break;
			case GameState.FillPiece:
				FillPiece();
				break;
        }

	}

    public void Idling()
    {
        
            if (Input.GetMouseButtonDown(0))
            {
                selectedPiecce = board.GetNearestPiece(Input.mousePosition);

            state = GameState.PieceMove;
                

            }
            
            
        

    }

    public void PieceMove()
    {
        
            
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
            state = GameState.MatchCheck;
                
            }
            
        
    }

    public void MatchCheck()
    {
        
            if (board.HasMatch())
            {
            state = GameState.DeletePiece;
                
            }
            else
            {
            state = GameState.Idle;
                
            }
            
        
    }
    public void DeletePiece()
    {
		state = GameState.Wait;
		StartCoroutine(board.DeleteMarchPiece(() => state=GameState.FillPiece));
        
    }
    public void FillPiece()
    {
		state = GameState.Wait;
		StartCoroutine(board.DeleteMarchPiece(() => state=GameState.MatchCheck));
        
    }
}
