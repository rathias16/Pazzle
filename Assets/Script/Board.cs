using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Board : MonoBehaviour {

    [SerializeField]
    private GameObject piecePrefab;

    private PieceManager[,] board;
	private static Vector2[] directions = new Vector2[]{Vector2.up,Vector2.down,Vector2.left,Vector2.right};
    private int width;
    private int height;
    private int pieceWidth;
    private int randomSeed;
	//ゲームセット
    public void InitializeBoard(int boardWidth, int boardHeight)
    {
        width = boardWidth;
        height = boardHeight;

        pieceWidth = Screen.width / boardWidth;

        board = new PieceManager[width, height];

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                CreatePiece(new Vector2(i, j));
            }
        }
    }
	//一番近いピースを取得
    public PieceManager GetNearestPiece(Vector3 input)
    {
        var minDist = float.MaxValue;
        PieceManager nearestPiece = null;

        foreach (var p in board)
        {
            if (p != null) {
                var dist = Vector3.Distance(input, p.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestPiece = p;
                }
            }
        }

        return nearestPiece;

    }
	//ピースを入れ替える
    public void SwitchPiece(PieceManager p1,PieceManager p2)
    {
        var p1Position = p1.transform.position;
        p1.transform.position = p2.transform.position;
        p2.transform.position = p1Position;

        var p1BoardPos = GetPieceBoardPos(p1);
        var p2BoardPos = GetPieceBoardPos(p2);
        board[(int)p1BoardPos.x, (int)p1BoardPos.y] = p2;
        board[(int)p2BoardPos.x, (int)p2BoardPos.y] = p1;

    }
	//マッチを確認
    public bool HasMatch()
    {
        foreach (var piece in board)
        {
            if (IsMatchPiece(piece))
            {
                return true;
            }
        }
        return false;
    }
	//マッチしたピースを削除
    public IEnumerator DeleteMarchPiece(Action endCallBack)
    {
        
        foreach (var piece in board)
        {
            if(piece != null && IsMatchPiece(piece))
            {
				var pos = GetPieceBoardPos(piece);
				DestroyMatchPiece(pos, piece.GetColor());
				yield return new WaitForSeconds(0.3f);
			}
        }
		endCallBack();
    }
	//欠けたピースを補充
    public IEnumerator FillPiece(Action endCallBack)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                FillPiece(new Vector2(i, j));
            }
        }
		yield return new WaitForSeconds(0.5f);
		endCallBack();
    }
	//マッチしている場合ほかのマッチしたピースとともに削除

	private void DestroyMatchPiece(Vector2 pos,PieceManager.PieceColor color) {
		if (!IsInBoard(pos))
			return;
		var piece = board[(int)pos.x, (int)pos.y];
		if (piece == null || piece.Delete || piece.GetColor() != color)
			return;
		if (!IsMatchPiece(piece))
			return;
		piece.Delete = true;
		foreach (var dir in directions)
		{
			DestroyMatchPiece(pos + dir, color);
		}
		Destroy(piece.gameObject);
	}
	
	//ピースのワールド座標を取得
    private Vector3 GetPieceWorldPos(Vector2 boardPos)
    {
        return new Vector3(boardPos.x * pieceWidth + (pieceWidth / 2), boardPos.y * pieceWidth + (pieceWidth / 2), 0);
    }

	//ピースを生成
    private void CreatePiece(Vector2 position)
    {
        var createPos = GetPieceWorldPos(position);

        int kind = UnityEngine.Random.Range(0,6);
        var piece = Instantiate(piecePrefab, createPos, Quaternion.identity).GetComponent<PieceManager>();
        piece.transform.SetParent(transform);
        piece.SetSize(pieceWidth);
        piece.SetColor(kind);

        board[(int)position.x, (int)position.y] = piece;

    }
	//ピースのボード上の座標を取得
	private Vector2 GetPieceBoardPos(PieceManager piece)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (board[i, j] == piece)
                {
                    return new Vector2(i, j);
                }
            }
        }

        return Vector2.zero;
    }
	//ピースがマッチしているかを判定
    private bool IsMatchPiece(PieceManager piece)
    {
        var pos = GetPieceBoardPos(piece);
        var kind = piece.GetColor();

        var verticalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.up) + GetSameKindPieceNum(kind, pos, Vector2.down) + 1;
        var horizontalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.right) + GetSameKindPieceNum(kind, pos, Vector2.left) + 1;

        return verticalMatchCount >= GameManager.MachingCount || horizontalMatchCount >= GameManager.MachingCount;

    }
	//特定の色のピースの数をカウント
    private int GetSameKindPieceNum(PieceManager.PieceColor kind, Vector2 pos,Vector2 SearchDir)
    {
        int count = 0;
      

        while (true)
        {
            pos += SearchDir;
            if (IsInBoard(pos) &&board[(int) pos.x,(int) pos.y].GetColor() == kind) {
                count++;
            } else
            {
                break;
            }
            
        }
        return count;
    }
	//ボードのマスキング
    private bool IsInBoard(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    }
	//ピースの補充
    private void FillPiece(Vector2 pos)
    {
        var piece = board[(int)pos.x,(int)pos.y];
        if(piece != null && !piece.Delete)
        {
            return;
        }

        var checkPos = pos + Vector2.up;
        while (IsInBoard(checkPos))
        {
            var checkPiece = board[(int)checkPos.x, (int)checkPos.y];
            if(checkPiece != null && !checkPiece.Delete)
            {
                checkPiece.transform.position = GetPieceWorldPos(pos);
                board[(int)pos.x, (int)pos.y] = checkPiece;
                board[(int)checkPos.x, (int)checkPos.y] = null;
                return;
            }
            checkPos += Vector2.up;
        }
        CreatePiece(pos);
    }

}
