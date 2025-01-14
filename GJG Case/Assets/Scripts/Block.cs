using UnityEngine;

public class Block : MonoBehaviour
{
    public int row;
    public int column;
    public Sprite[] sprite;

    private void Update()
    {
        // Mobil dokunma kontrolü
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) 
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    OnBlockTapped();
                }
            }
        }

        // Fare týklama kontrolü (debug amaçlý bilgisayarda da çalýþacak)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnBlockTapped();
            }
        }

        // Blok tipi ve baðlý blok sayýsýna göre sprite deðiþtirme
        int count = GridManager.Instance.GetConnectedBlocks(this).Count;
        if (this.tag == "Blue")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[18];
            }
            else if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[0];
            }
            else if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[1];
            }
            else if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[2];
            }
        }
        else if (this.tag == "Green")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[19];
            }
            else if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[3];
            }
            else if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[4];
            }
            else if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[5];
            }
        }
        else if (this.tag == "Pink")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[20];
            }
            else if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[6];
            }
            else if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[7];
            }
            else if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[8];
            }
        }
        else if (this.tag == "Purple")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[21];
            }
            else if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[9];
            }
            else if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[10];
            }
            else if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[11];
            }
        }
        else if (this.tag == "Red")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[22];
            }
            else if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[12];
            }
            else if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[13];
            }
            else if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[14];
            }
        }
        else if (this.tag == "Yellow")
        {
            if (count < 5)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[23];
            }
            else if (count >= 5 && count <= 7)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[15];
            }
            else if (count >= 8 && count <= 9)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[16];
            }
            else if (count >= 10)
            {
                this.GetComponent<SpriteRenderer>().sprite = sprite[17];
            }
        }
    }

    // Blok týklandýðýnda iþleme al
    private void OnBlockTapped()
    {
        if (GridManager.Instance.isProcessing)
        {
            Debug.Log("Ýþlem devam ediyor, lütfen bekleyin");
            return;
        }

        Debug.Log($"Týklanan/Dokunulan Blok: {name}, Pozisyon: ({row}, {column})");
        GridManager.Instance.CheckAndDestroyBlocks(this);
    }
}
