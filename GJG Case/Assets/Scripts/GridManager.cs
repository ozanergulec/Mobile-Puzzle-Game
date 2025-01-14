using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject explosionEffectPrefab;
    public bool isProcessing = false;
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
        do
        {
            ClearGrid();

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    SpawnBlock(row, column);
                }
            }
        }
        while (!HasMatchableBlocks()); 
    }

    private void ClearGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (gridArray[row, column] != null)
                {
                    Destroy(gridArray[row, column]);
                    gridArray[row, column] = null;
                }
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
        if (isProcessing) return; 

        List<Block> connectedBlocks = GetConnectedBlocks(startBlock);

        if (connectedBlocks.Count >= 2)
        {
            isProcessing = true; 

            foreach (Block block in connectedBlocks)
            {
                Vector2 blockPosition = block.transform.position;
                Instantiate(explosionEffectPrefab, blockPosition, Quaternion.identity);
                Vector2Int gridPosition = new Vector2Int(block.row, block.column);
                gridArray[gridPosition.x, gridPosition.y] = null;
                Destroy(block.gameObject);
            }

            StartCoroutine(UpdateGrid());
        }
        else
        {
            Debug.Log("Yeterli bağlantı yok: " + connectedBlocks.Count);
        }
    }

    public List<Block> GetConnectedBlocks(Block startBlock)
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

    private IEnumerator UpdateGrid()
    {
        yield return StartCoroutine(DropBlocksWithAnimation());
        RefillGrid();
        yield return new WaitForSeconds(0.2f); 
        isProcessing = false; 
    }

    private IEnumerator DropBlocksWithAnimation()
    {
        for (int column = 0; column < columns; column++)
        {
            for (int row = rows - 1; row > 0; row--)
            {
                if (gridArray[row, column] == null) 
                {
                    for (int checkRow = row - 1; checkRow >= 0; checkRow--)
                    {
                        if (gridArray[checkRow, column] != null) 
                        {
                            GameObject block = gridArray[checkRow, column];
                            gridArray[checkRow, column] = null; 
                            gridArray[row, column] = block; 

                            Block blockComponent = block.GetComponent<Block>();
                            blockComponent.row = row;
                            blockComponent.column = column;

                            
                            yield return StartCoroutine(MoveBlock(block, GetWorldPosition(row, column), 0.1f));
                            break; 
                        }
                    }
                }
            }
        }
    }
    //Gridde patlatılabilir blok var mı kontrol et
    private bool HasMatchableBlocks()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (gridArray[row, col] != null)
                {
                    List<Block> connected = GetConnectedBlocks(gridArray[row, col].GetComponent<Block>());
                    if (connected.Count >= 2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void RefillGrid()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int column = 0; column < columns; column++)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                if (gridArray[row, column] == null)
                {
                    emptyPositions.Add(new Vector2Int(row, column));
                }
            }
        }

        
        foreach (Vector2Int pos in emptyPositions.OrderBy(p => p.x))
        {
            GameObject newBlock;
            if (pos == emptyPositions[emptyPositions.Count - 1])
            {

                newBlock = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)],
                                    GetWorldPosition(pos.x, pos.y),
                                    Quaternion.identity);

                newBlock.GetComponent<Block>().row = pos.x;
                newBlock.GetComponent<Block>().column = pos.y;
                gridArray[pos.x, pos.y] = newBlock;

                if (!HasMatchableBlocks())
                {
                    Destroy(newBlock);
                    newBlock = CreateSuitableBlock(pos.x, pos.y);
                }
            }
            else
            {
                newBlock = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)],
                                    GetWorldPosition(pos.x, pos.y),
                                    Quaternion.identity);
            }

            newBlock.GetComponent<Block>().row = pos.x;
            newBlock.GetComponent<Block>().column = pos.y;
            gridArray[pos.x, pos.y] = newBlock;

            Vector3 targetPosition = GetWorldPosition(pos.x, pos.y);
            StartCoroutine(MoveBlock(newBlock, targetPosition, 0.1f));
        }
    }

    private GameObject CreateSuitableBlock(int row, int column)
    {
        List<GameObject> validPrefabs = new List<GameObject>();

        foreach (GameObject prefab in blockPrefabs)
        {
            GameObject tempBlock = Instantiate(prefab, Vector3.one * 1000, Quaternion.identity);
            Block tempBlockComponent = tempBlock.GetComponent<Block>();
            tempBlockComponent.row = row;
            tempBlockComponent.column = column;
            gridArray[row, column] = tempBlock;

            if (HasMatchableBlocksForPosition(row, column))
            {
                validPrefabs.Add(prefab);
            }

            gridArray[row, column] = null;
            Destroy(tempBlock);
        }

        if (validPrefabs.Count > 0)
        {
            return Instantiate(validPrefabs[Random.Range(0, validPrefabs.Count)],
                             GetWorldPosition(row, column),
                             Quaternion.identity);
        }
        return Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)],
                         GetWorldPosition(row, column),
                         Quaternion.identity);
    }

    // Çevredeki bloklarla eşleşme kontrol et
    private bool HasMatchableBlocksForPosition(int row, int column)
    {
        
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };

        foreach (Vector2Int dir in directions)
        {
            int newRow = row + dir.x;
            int newCol = column + dir.y;

            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < columns)
            {
                if (gridArray[newRow, newCol] != null)
                {
                    List<Block> connected = GetConnectedBlocks(gridArray[newRow, newCol].GetComponent<Block>());
                    if (connected.Count >= 2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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