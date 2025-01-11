using UnityEngine;

public class Block : MonoBehaviour
{
    public int row; 
    public int column; 
    public Sprite[] sprite;
    private void OnMouseDown()
    {
        if (GridManager.Instance.isProcessing)
        {
            Debug.Log("Ýþlem devam ediyor, lütfen bekleyin");
            return;
        }

        Debug.Log($"Týklanan Blok: {name}, Pozisyon: ({row}, {column})");
        GridManager.Instance.CheckAndDestroyBlocks(this);
    }
    private void Update()
    {
        int count = GridManager.Instance.GetConnectedBlocks(this).Count;
        if (this.tag == "Blue")
        {
            if (count <5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[18];
            }
            if (count>=5 && count<=7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[0];
            }
            if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[1];
            }
            if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[2];
            }
        }
        if (this.tag == "Green")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[19];
            }
            if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[3];
            }
            if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[4];
            }
            if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[5];
            }
        }
        if (this.tag == "Pink")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[20];
            }
            if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[6];
            }
            if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[7];
            }
            if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[8];
            }
        }
        if (this.tag == "Purple")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[21];
            }
            if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[9];
            }
            if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[10];
            }
            if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[11];
            }
        }
        if (this.tag == "Red")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[22];
            }
            if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[12];
            }
            if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[13];
            }
            if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[14];
            }
        }
        if (this.tag == "Yellow")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[23];
            }
            if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[15];
            }
            if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[16];
            }
            if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[17];
            }
        }
    }
}
