using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    // Grid varible keeps track of minoes' positions within the grid and allows them to stack over one another
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    // Score
    public int[] scoreForRowsCleared = { 40, 100, 300, 1200 };
    private static int numRowsClearedThisTurn = 0;
    public static int currentScore = 0;
    public Text hudScore;

    public static float fallSpeed = 1.0f;

    // Sound
    public AudioClip clearedSingleSound;
    public AudioClip clearedDoubleSound;
    public AudioClip clearedTripleSound;
    public AudioClip clearedTetrisSound;
    private AudioSource audioSource;

    // Previewing tetrominoes
    static private int rear = 0;   // Pointer for previewTetromino queue;
    static int[] next7 = new int[7];
    private GameObject nextTetromino;
    private GameObject[] previewTetromino = new GameObject[3];

    private GameObject savedTetromino;

    private Vector2[] previewTetrominoPositions = { new Vector2(16.0f, 15.5f), new Vector2(16.0f, 13.0f), new Vector2(16.0f, 10.5f) };
    private Vector2 savedTetrominoPosition = new Vector2(-6.0f, 15.5f);

    public int maxSwaps = 1;
    private int currentSwaps = 0;


    // Levels
    public static int currentLevel = 1;
    private int numRowsCleared = 0;
    public Text hudLevel;
    public Text hudLines;

    // Changing levels using slider
    public static int startingLevel = 1;


    // Start is called before the first frame update
    void Start()
    {
        currentLevel = startingLevel;
        SpawnNextTetromino();
        audioSource = GetComponent<AudioSource>();
    }


    // Updates every frame
    private void Update()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
        CheckUserInput();
    }


    // Iterates over each mino of the passed tetromino object, take their positions and assigns them into the grid array
    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // Updates grid as the mino falls, therefore if mino moves down one, removes the previous position
                if (grid[x, y] != null)
                    if (grid[x, y].parent == tetromino.transform)
                        grid[x, y] = null;
            }
        }

        // Stores mino's position
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)    // Prevents index out of bounds excemption
                grid[(int)pos.x, (int)pos.y] = mino;
        }
    }


    // Returns a Transform object if there exists a mino at the passed position, else returns null
    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
            return null;
        else
            return grid[(int)pos.x, (int)pos.y];
    }


    // Checks if the tetromino is within the walls of the grid
    public bool IsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }


    // Returns a new vector where its values are the x and y positions of the passed vector rounded to the nearest integer
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }


    // Spawns a random tetromino at the top of the grid
    public void SpawnNextTetromino()
    {
        if (nextTetromino == null)
        {
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 19.0f), Quaternion.identity);
            previewTetromino[0] = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPositions[0], Quaternion.identity);
            previewTetromino[1] = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPositions[1], Quaternion.identity);
            previewTetromino[0].GetComponent<Tetromino>().enabled = false;
            previewTetromino[1].GetComponent<Tetromino>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";
        }
        else
        {
            previewTetromino[0].transform.localPosition = new Vector2(5.0f, 19.0f);
            nextTetromino = previewTetromino[0];
            
            previewTetromino[0] = previewTetromino[1];
            previewTetromino[1] = previewTetromino[2];
            previewTetromino[0].transform.localPosition = previewTetrominoPositions[0];
            previewTetromino[1].transform.localPosition = previewTetrominoPositions[1];

            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.tag = "currentActiveTetromino";
        }
        previewTetromino[2] = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPositions[2], Quaternion.identity);
        previewTetromino[2].GetComponent<Tetromino>().enabled = false;

        currentSwaps = 0;
    }


    public void SaveTetromino(Transform t)
    {
        currentSwaps++;

        if (currentSwaps > maxSwaps)
            return;

        // There is currently a tetromino being held
        if (savedTetromino != null)
        {
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavedTetromino");
            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);

            if (!isValidPosition(tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;
                return;
            }
            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);
        }

        // There is currently no tetromino being held
        else
        {
            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            nextTetromino = new GameObject();
            SpawnNextTetromino();
        }
    }


    // Helper function to SpawnNextTetromino() that randomly generates the name of a tetromino and returns it 
    string GetRandomTetromino()
    {
        // Ensures that there is not too much repetition in tetrominoes by having every 7 be different
        bool present;
        if (rear == 0)
        {
            for (int i = 0; i < 7; i++)
            {
                present = false;
                int num = Random.Range(0, 7);
                for (int j = 0; j < i; j++)
                {
                    if (next7[j] == num)
                        present = true;
                }
                if (!present)
                    next7[i] = num;
                else
                    i--;
            }
        }

        string randomTetrominoName = "";

        switch (next7[rear])
        {
            case 0:
                randomTetrominoName = "tetrominoT";
                break;
            case 1:
                randomTetrominoName = "tetrominoI";
                break;
            case 2:
                randomTetrominoName = "tetrominoJ";
                break;
            case 3:
                randomTetrominoName = "tetrominoL";
                break;
            case 4:
                randomTetrominoName = "tetrominoS";
                break;
            case 5:
                randomTetrominoName = "tetrominoZ";
                break;
            case 6:
                randomTetrominoName = "tetrominoO";
                break;
        }
        rear = (rear + 1) % 7;
        return "Prefabs/" + randomTetrominoName;
    }


    // Deletes full row and moves the above row down
    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                y--;
            }
        }
    }

    /* Helper functions to DeleteRow(): */

    // Checks if a row is full at passed y height
    public bool IsFullRowAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            // If a position returns null instead of a transform object, the row is not full
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        // Since full row, increment numRowsClearedThisTurn counter
        numRowsClearedThisTurn++;
        return true;
    }

    // Iterates over all x values to delete minoes at y position
    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    // Moves all rows above and including height y down by one
    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    // Helper function to MoveAllRowsDown(int) which iterates over a row (all x values in grid array) and moves each transform down by 1
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    /* End of helper functions to DeleteRow(): */


    bool isValidPosition(GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (!IsInsideGrid(pos))
                return false;
            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
                return false;
        }
        return true;
    }


    // Checks if a tetromino falls above the grid, meaning game over
    public bool IsAboveGrid(Tetromino tetromino)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);
                if (pos.y > gridHeight - 1)
                    return true;

            }
        }
        return false;
    }


    // Ends game and loads game over scene
    public void GameOver()
    {
        currentScore = -40 * currentLevel;
        SceneManager.LoadScene("GameOver");
    }


    /* Update functions */

    // Updates score for rows cleared
    public void UpdateScore()
    {
        if (numRowsClearedThisTurn > 0)
        {
            currentScore += scoreForRowsCleared[numRowsClearedThisTurn - 1] * currentLevel;
            numRowsCleared += numRowsClearedThisTurn;
            // Plays different sound according to the number of lines cleared
            switch (numRowsClearedThisTurn)
            {
                case 1:
                    audioSource.PlayOneShot(clearedSingleSound);
                    break;
                case 2:
                    audioSource.PlayOneShot(clearedDoubleSound);
                    break;
                case 3:
                    audioSource.PlayOneShot(clearedTripleSound);
                    break;
                case 4:
                    audioSource.PlayOneShot(clearedTetrisSound);
                    break;
            }
            numRowsClearedThisTurn = 0;
        }
    }

    // Updates the scores, lines and levels text in the scene
    public void UpdateUI()
    {
        hudScore.text = currentScore.ToString();
        hudLevel.text = currentLevel.ToString();
        hudLines.text = numRowsCleared.ToString();
    }

    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SaveTetromino(tempNextTetromino.transform);
        }
    }


    void UpdateLevel()
    {
        currentLevel = numRowsCleared / 10 + startingLevel;
    }


    void UpdateSpeed()
    {
        fallSpeed = 1.0f - (float)(currentLevel - 1) * 0.1f;
    }
}
