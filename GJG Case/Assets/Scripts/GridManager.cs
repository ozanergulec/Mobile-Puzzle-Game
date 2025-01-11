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
    public GameObject explosionEffectPrefab;

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
        for (int column = 0; column < columns; column++)
        {
            int emptySpaces = 0;
            for (int row = rows - 1; row >= 0; row--)
            {
                if (gridArray[row, column] == null)
                {
                    emptySpaces++;
                }
            }

            for (int i = 0; i < emptySpaces; i++)
            {
                int newRow = emptySpaces - 1 - i;
                GameObject newBlock;

                // Eğer grid'de hiç patlatılabilir blok yoksa 
                if (i == emptySpaces - 1 && column == columns - 1 && !HasMatchableBlocks())
                {
                    newBlock = CreateSuitableBlock(newRow, column);
                }
                else
                {
                    // Normal rastgele spawn
                    newBlock = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)],
                                        GetWorldPosition(newRow, column),
                                        Quaternion.identity);
                }

                newBlock.GetComponent<Block>().row = newRow;
                newBlock.GetComponent<Block>().column = column;
                gridArray[newRow, column] = newBlock;

                Vector3 targetPosition = GetWorldPosition(newRow, column);
                StartCoroutine(MoveBlock(newBlock, targetPosition, 0.1f));
            }
        }
    }
    //Deadlock olmaması için uygun blok oluştur
    private GameObject CreateSuitableBlock(int row, int column)
    {
        List<GameObject> validPrefabs = new List<GameObject>();
        List<GameObject> allValidPrefabs = new List<GameObject>();

        
        foreach (GameObject prefab in blockPrefabs)
        {
            GameObject tempBlock = Instantiate(prefab, Vector3.one * 1000, Quaternion.identity);
            Block tempBlockComponent = tempBlock.GetComponent<Block>();
            tempBlockComponent.row = row;
            tempBlockComponent.column = column;

            gridArray[row, column] = tempBlock;

            // Bu blokla eşleşme var mı kontrol et
            List<Block> connected = GetConnectedBlocks(tempBlockComponent);

            gridArray[row, column] = null;
            Destroy(tempBlock);

            // 2-3 eşleşme yapan blokları öncelikli listeye al
            if (connected.Count >= 2 && connected.Count <= 3)
            {
                validPrefabs.Add(prefab);
            }
            // Tüm geçerli prefabları da ayrı bir listede tut
            if (connected.Count >= 2)
            {
                allValidPrefabs.Add(prefab);
            }
        }

        // Önce 2-3 eşleşmeli prefabları dene
        if (validPrefabs.Count > 0)
        {
            return Instantiate(validPrefabs[Random.Range(0, validPrefabs.Count)],
                             GetWorldPosition(row, column),
                             Quaternion.identity);
        }
        // Yoksa tüm geçerli prefabları dene
        else if (allValidPrefabs.Count > 0)
        {
            return Instantiate(allValidPrefabs[Random.Range(0, allValidPrefabs.Count)],
                             GetWorldPosition(row, column),
                             Quaternion.identity);
        }
        // Hiç uygun prefab bulunamazsa rastgele seç
        else
        {
            return Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)],
                             GetWorldPosition(row, column),
                             Quaternion.identity);
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