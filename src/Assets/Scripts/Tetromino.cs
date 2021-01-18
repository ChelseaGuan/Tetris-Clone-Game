using UnityEngine;

public class Tetromino : MonoBehaviour
{
    // Tetromino moving down
    float fall = 0;
    private float fallSpeed;

    // Tetromino rotation
    public bool allowRotation = true;
    public bool limitRotation = false;

    // Scoring
    public int individualScore = 40;
    private float individualScoreTime;

    // Sound
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    private AudioSource audioSource;

    // Player controls
    private float continuousVerticalSpeed = 0.05f;  // Movement speed when down arrow is held down
    private float continuousHorizontalSpeed = 0.1f;  // Movement speed when down arrow is held down
    private float buttonDownWaitMax = 0.2f; // Time it takes for tetromino to recognize button is being held down
    private float horizontalTimer = 0;
    private float verticalTimer = 0;
    private float buttonDownWaitTimer = 0;
    private bool movedImmediateHorizontal = false;  // Allows for single movement when clicking button once
    private bool movedImmediateVertical = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
        UpdateIndividualScore();
        UpdateFallSpeed();
    }


    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            horizontalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimer = 0;
            movedImmediateHorizontal = false;
            movedImmediateVertical = false;
        }

        // Moves tetromino downward
        else if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            // Moves tetromino as long as key is held down
            if (movedImmediateVertical)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }

                if (verticalTimer < continuousVerticalSpeed)
                {
                    verticalTimer += Time.deltaTime;
                    // Exits method and does not move tetromino since not enough time has elapsed
                    return;
                }
            }
            if (!movedImmediateVertical)
                movedImmediateVertical = true;
            verticalTimer = 0;

            transform.position += new Vector3(0, -1, 0);
            if (IsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                if (Input.GetKey(KeyCode.DownArrow))
                    audioSource.PlayOneShot(moveSound);
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);
                FindObjectOfType<Game>().DeleteRow();
                if (FindObjectOfType<Game>().IsAboveGrid(this))
                    FindObjectOfType<Game>().GameOver();
                audioSource.PlayOneShot(landSound);
                FindObjectOfType<Game>().SpawnNextTetromino();
                Game.currentScore += individualScore * Game.currentLevel;
                enabled = false;
                tag = "Untagged";
            }
            fall = Time.time;
        }

        // Moves tetromino to the right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            // Moves tetromino as long as key is held down
            if (movedImmediateHorizontal)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }

                if (horizontalTimer < continuousHorizontalSpeed)
                {
                    horizontalTimer += Time.deltaTime;
                    // Exits method and does not move tetromino since not enough time has elapsed
                    return;
                }
            }
            if (!movedImmediateHorizontal)
                movedImmediateHorizontal = true;

            horizontalTimer = 0;

            transform.position += new Vector3(1, 0, 0);
            if (IsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                audioSource.PlayOneShot(moveSound);
            }

            else
                transform.position += new Vector3(-1, 0, 0);

        }

        // Moves tetromino to the left
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            // Moves tetromino as long as key is held down
            if (movedImmediateHorizontal)
            {
                if (buttonDownWaitTimer < buttonDownWaitMax)
                {
                    buttonDownWaitTimer += Time.deltaTime;
                    return;
                }

                if (horizontalTimer < continuousHorizontalSpeed)
                {
                    horizontalTimer += Time.deltaTime;
                    // Exits method and does not move tetromino since not enough time has elapsed
                    return;
                }
            }
            if (!movedImmediateHorizontal)
                movedImmediateHorizontal = true;

            horizontalTimer = 0;

            transform.position += new Vector3(-1, 0, 0);
            if (IsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                audioSource.PlayOneShot(moveSound);
            }
            else
                transform.position += new Vector3(1, 0, 0);

        }

        // Rotates tetromino
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (allowRotation)
            {
                if (limitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                        transform.Rotate(0, 0, -90);
                    else
                        transform.Rotate(0, 0, 90);
                }

                else
                    transform.Rotate(0, 0, 90);

                if (IsValidPosition())
                {
                    FindObjectOfType<Game>().UpdateGrid(this);
                    audioSource.PlayOneShot(rotateSound);
                }
                else
                {
                    if (limitRotation)
                    {
                        if (transform.rotation.eulerAngles.z >= 90)
                            transform.Rotate(0, 0, -90);
                        else
                            transform.Rotate(0, 0, 90);
                    }
                    else
                        transform.Rotate(0, 0, -90);

                }
            }
        }

    }

    // Checks if tetromino is within grid
    bool IsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);

            if (!FindObjectOfType<Game>().IsInsideGrid(pos))
                return false;
            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
                return false;
        }
        return true;
    }


    // Keeps count of points gained by a dropping a single tetromino. Score starts at 40 and decrements the longer the uer takes to land the tetromino
    void UpdateIndividualScore()
    {
        if (individualScoreTime < 1)
            individualScoreTime += Time.deltaTime;
        else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 4, 4);
        }
    }


    void UpdateFallSpeed()
    {
        fallSpeed = Game.fallSpeed;
    }

}
