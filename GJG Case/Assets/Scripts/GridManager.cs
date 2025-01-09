using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance; 
    public int rows = 8; 
    public int columns = 8; 
    public float cellSize = 1.0f; 
    public float spacing = 0.1f; 
    public GameObject[] blockPrefabs; 
    public GameObject[,] gridArray; 

    private void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        gridArray = new GameObject[rows, columns];

        
        float gridWidth = columns * (cellSize + spacing) - spacing;
        float gridHeight = rows * (cellSize + spacing) - spacing;

       
        Vector2 startPosition = new Vector2(-gridWidth / 2f, -gridHeight / 2f);

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
               
                float xPosition = column * (cellSize + spacing) + startPosition.x;
                float yPosition = row * (cellSize + spacing) + startPosition.y;
                Vector2 position = new Vector2(xPosition, yPosition);

               
                GameObject randomBlockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];

                
                GameObject block = Instantiate(randomBlockPrefab, position, Quaternion.identity, transform);

               
                Block blockComponent = block.GetComponent<Block>();
                blockComponent.row = row;
                blockComponent.column = column;

               
                gridArray[row, column] = block;

                block.name = $"Block ({row}, {column})";
            }
        }
    }

    public void CheckAndDestroyBlocks(Block startBlock)
    {
        
        Debug.Log($"Týklanan Blok: {startBlock.name}, Tag: {startBlock.tag}");

        
        List<Block> connectedBlocks = GetConnectedBlocks(startBlock);

        
        if (connectedBlocks.Count >= 2)
        {
            foreach (Block block in connectedBlocks)
            {
                Vector2Int gridPosition = new Vector2Int(block.row, block.column);
                gridArray[gridPosition.x, gridPosition.y] = null; 
                Destroy(block.gameObject); 
            }
        }
        else
        {
            Debug.Log("Yeterli baðlantý yok: " + connectedBlocks.Count);
        }
    }


    private List<Block> GetConnectedBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = new List<Block>();
        Queue<Block> queue = new Queue<Block>();
        HashSet<Block> visited = new HashSet<Block>();

        queue.Enqueue(startBlock);
        visited.Add(startBlock);

        while (queue.Count > 0)
        {
            Block currentBlock = queue.Dequeue();
            connectedBlocks.Add(currentBlock);

            // Komþularý kontrol et
            foreach (Block neighbor in GetNeighbors(currentBlock))
            {
                if (!visited.Contains(neighbor) && neighbor.CompareTag(startBlock.tag))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return connectedBlocks;
    }

    private List<Block> GetNeighbors(Block block)
    {
        List<Block> neighbors = new List<Block>();
        if (block.row > 0 && gridArray[block.row - 1, block.column] != null)
            neighbors.Add(gridArray[block.row - 1, block.column].GetComponent<Block>());

        if (block.row < rows - 1 && gridArray[block.row + 1, block.column] != null)
            neighbors.Add(gridArray[block.row + 1, block.column].GetComponent<Block>());

        if (block.column > 0 && gridArray[block.row, block.column - 1] != null)
            neighbors.Add(gridArray[block.row, block.column - 1].GetComponent<Block>());

        if (block.column < columns - 1 && gridArray[block.row, block.column + 1] != null)
            neighbors.Add(gridArray[block.row, block.column + 1].GetComponent<Block>());

        return neighbors;
    }
}
