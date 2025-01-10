using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int rows = 8;
    public int columns = 8;
    public float cellSize = 1f;
    public float spacing = 0.1f;
    public GameObject[] blockPrefabs;
    public GameObject[,] gridArray;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        gridArray = new GameObject[rows, columns];
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                SpawnBlock(row, column);
            }
        }
    }

    private void SpawnBlock(int row, int column)
    {
        GameObject randomBlock = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
        GameObject block = Instantiate(randomBlock, GetWorldPosition(row, column), Quaternion.identity);
        block.GetComponent<Block>().row = row;
        block.GetComponent<Block>().column = column;
        gridArray[row, column] = block;
    }

    private Vector2 GetWorldPosition(int row, int column)
    {
        float xPosition = column * (cellSize + spacing) - (columns * (cellSize + spacing) - spacing) / 2f;
        float yPosition = -(row * (cellSize + spacing)) + (rows * (cellSize + spacing) - spacing) / 2f;
        return new Vector2(xPosition, yPosition);
    }


    public void CheckAndDestroyBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = GetConnectedBlocks(startBlock);

        if (connectedBlocks.Count >= 2)
        {
            foreach (Block block in connectedBlocks)
            {
                Vector2Int gridPosition = new Vector2Int(block.row, block.column);
                gridArray[gridPosition.x, gridPosition.y] = null; // Grid'den kaldýr
                Destroy(block.gameObject); // Bloklarý yok et
            }

            // Bloklarý yok ettikten sonra aþaðýya kaydýr
            StartCoroutine(DropBlocksWithAnimation());
        }
        else
        {
            Debug.Log("Yeterli baðlantý yok: " + connectedBlocks.Count);
        }
    }

    private List<Block> GetConnectedBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = new List<Block>();
        Queue<Block> toCheck = new Queue<Block>();
        HashSet<Block> visited = new HashSet<Block>();

        toCheck.Enqueue(startBlock);
        visited.Add(startBlock);

        while (toCheck.Count > 0)
        {
            Block current = toCheck.Dequeue();
            connectedBlocks.Add(current);

            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1), new Vector2Int(1, 0),
                new Vector2Int(0, -1), new Vector2Int(-1, 0)
            };

            foreach (Vector2Int dir in directions)
            {
                int newRow = current.row + dir.x;
                int newCol = current.column + dir.y;

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < columns)
                {
                    GameObject neighbor = gridArray[newRow, newCol];
                    if (neighbor != null && neighbor.tag == startBlock.tag)
                    {
                        Block neighborBlock = neighbor.GetComponent<Block>();
                        if (!visited.Contains(neighborBlock))
                        {
                            visited.Add(neighborBlock);
                            toCheck.Enqueue(neighborBlock);
                        }
                    }
                }
            }
        }

        return connectedBlocks;
    }

    private IEnumerator DropBlocksWithAnimation()
    {
        for (int column = 0; column < columns; column++)
        {
            for (int row = rows - 1; row > 0; row--)
            {
                if (gridArray[row, column] == null) // Eðer bu hücre boþsa
                {
                    for (int checkRow = row - 1; checkRow >= 0; checkRow--)
                    {
                        if (gridArray[checkRow, column] != null) // Eðer üstte blok varsa
                        {
                            GameObject block = gridArray[checkRow, column];
                            gridArray[checkRow, column] = null; // Eski yerini boþalt
                            gridArray[row, column] = block; // Yeni yerine yerleþtir

                            Block blockComponent = block.GetComponent<Block>();
                            blockComponent.row = row;
                            blockComponent.column = column;

                            // Blok pozisyonunu animasyonla hareket ettir
                            yield return StartCoroutine(MoveBlock(block, GetWorldPosition(row, column), 0.1f));
                            break; // Daha fazla blok arama
                        }
                    }
                }
            }
        }
    }

    private IEnumerator MoveBlock(GameObject block, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = block.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            block.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        block.transform.position = targetPosition;
    }
}
