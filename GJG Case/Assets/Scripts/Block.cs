using UnityEngine;

public class Block : MonoBehaviour
{
    public int row; 
    public int column; 

    private void OnMouseDown()
    {
        Debug.Log($"T�klanan Blok: {name}, Pozisyon: ({row}, {column})");
        GridManager.Instance.CheckAndDestroyBlocks(this);
    }
}
