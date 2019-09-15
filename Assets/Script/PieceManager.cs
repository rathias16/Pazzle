using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceManager : MonoBehaviour {

    public Image Piece;

    public Sprite[] Images = new Sprite[6];

    public enum PieceColor
    {
        Yellow,
        Green,
        Blue,
        Purple,
        Pink,
        Turquoise,
    }
    public PieceColor piececolor;

    public bool Delete;

    private RectTransform rect;

    public void SetColor(int num)
    {
        this.Piece.sprite = Images[num];

        switch (num)
        {
            case 0:
                this.piececolor = PieceColor.Yellow;
                break;
            case 1:
                this.piececolor = PieceColor.Green;
                break;
            case 2:
                this.piececolor = PieceColor.Blue;
                break;
            case 3:
                this.piececolor = PieceColor.Purple;
                break;
            case 4:
                this.piececolor = PieceColor.Pink;
                break;
            case 5:
                this.piececolor = PieceColor.Turquoise;
                break;
        }
    }

    public void SetSize(int size)
    {
        this.rect.sizeDelta = Vector2.one * size;

    }

    public PieceColor GetColor()
    {
        return this.piececolor;
    }

	// Use this for initialization
	void Awake () {
        rect = GetComponent<RectTransform>();

        Delete = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
