using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
[System.Serializable]
public class MultiDimensionalGO
{
    public GameObject[] GOArray;
}

[System.Serializable]
public class MultiDimensionalInt
{
    public int[] intArray;
}
*/
public class GameManager : MonoBehaviour
{
    
    public static int BoardWidth;
    public static int BoardHeight;
    public GameObject[] Prefabs;
    public Sprite[] SpriteList;
    public TextAsset textfile;
    public GameObject[,] BoardStateGO;
    public int[,] BoardState;

    void Start()
    {
        ReadBoard();
        InitializeBoard();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero);
            if (cubeHit)
            {
                //gamestates 6+ are clickables disabled due to others in line
                if (cubeHit.transform.gameObject.GetComponent<SquareManager>().GameState < 6)
                {
                    FlipIt(cubeHit.transform.gameObject);
                    CheckWin();
                }
            }
        }
#endif

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero);
            if (cubeHit)
            {
                //gamestates 6+ are clickables disabled due to others in line
                if (cubeHit.transform.gameObject.GetComponent<SquareManager>().GameState < 6)
                {
                    FlipIt(cubeHit.transform.gameObject);
                    CheckWin();
                }
            }
        }
#endif
    }

    void ReadBoard()
    {
        string ReadFile = textfile.text;
        int i = 0, j = 0;
        //get dimensions. file length ends up being 2x^2+x
        for (int k = 5; k < 16; k++)
        {
            if ((k * k * 2) + k == ReadFile.Length)
                BoardHeight = k;
        }
        BoardWidth = BoardHeight;
        int[,] result = new int[BoardHeight, BoardWidth];
        foreach (var row in ReadFile.Split('\n'))
        {
            if (row.Length < 3)
            {
                break;
            }
            j = 0;
            foreach (var col in row.Trim().Split(','))
            {
                result[i, j] = int.Parse(col.Trim());
                j++;
            }
            i++;
        }
        BoardState = result;

    }

    void InitializeBoard()
    {
        BoardStateGO = new GameObject[BoardHeight, BoardWidth];
        for (int y = 0; y < BoardHeight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                //reverse y during this so output matches map generator
                int yReverse = (BoardHeight - y) - 1;
                GameObject GridSquare = Instantiate(Prefabs[BoardState[yReverse, x]], new Vector3((-4.6f + x) / 2, (-5f + y) / 2, 0f), Quaternion.identity);
                //this eventually needs cleaned up for resizing to board size and resolution
                SquareManager state = GridSquare.GetComponent<SquareManager>();
                state.GameState = BoardState[yReverse, x];
                state.x = x;
                state.y = yReverse;
                BoardStateGO[yReverse, x] = GridSquare;
            }
        }

    }

    void FlipIt(GameObject HomeSquare)
    {
        bool Grow = false;
        // clickable on to clickable off and vice versa
        if (HomeSquare.GetComponent<SquareManager>().GameState == 5)
        {
            HomeSquare.GetComponent<SquareManager>().GameState = 4;
            Grow = true;
        }
        else if (HomeSquare.GetComponent<SquareManager>().GameState == 4)
        {
            HomeSquare.GetComponent<SquareManager>().GameState = 5;
            Grow = false;
        }
        HomeSquare.GetComponent<SpriteRenderer>().sprite = SpriteList[HomeSquare.GetComponent<SquareManager>().GameState];
        FlipUp(HomeSquare, Grow);
        FlipDown(HomeSquare, Grow);
        FlipRight(HomeSquare, Grow);
        FlipLeft(HomeSquare, Grow);

    }

    void FlipUp(GameObject HomeSquare, bool Grow)
    {
        for (int z = (HomeSquare.GetComponent<SquareManager>().y + 1); z < BoardHeight; z++)
        {
            GameObject Flipper = BoardStateGO[z, HomeSquare.GetComponent<SquareManager>().x];
            if (Flipper.GetComponent<SquareManager>().GameState == 0)
            {
                break;
            }
            if (Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState + 1;
            }
            if (!Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState - 1;
            }
            Flipper.GetComponent<SpriteRenderer>().sprite = SpriteList[Flipper.GetComponent<SquareManager>().GameState];
        }
    }

    void FlipDown(GameObject HomeSquare, bool Grow)
    {
        for (int z = (HomeSquare.GetComponent<SquareManager>().y - 1); z >= 0; z--)
        {
            GameObject Flipper = BoardStateGO[z, HomeSquare.GetComponent<SquareManager>().x];
            if (Flipper.GetComponent<SquareManager>().GameState == 0)
            {
                break;
            }
            if (Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState + 1;
            }
            if (!Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState - 1;
            }
            Flipper.GetComponent<SpriteRenderer>().sprite = SpriteList[Flipper.GetComponent<SquareManager>().GameState];
        }
    }

    void FlipRight(GameObject HomeSquare, bool Grow)
    {
        for (int z = (HomeSquare.GetComponent<SquareManager>().x + 1); z < BoardWidth; z++)
        {
            GameObject Flipper = BoardStateGO[HomeSquare.GetComponent<SquareManager>().y, z];
            if (Flipper.GetComponent<SquareManager>().GameState == 0)
            {
                break;
            }
            if (Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState + 1;
            }
            if (!Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState - 1;
            }
            Flipper.GetComponent<SpriteRenderer>().sprite = SpriteList[Flipper.GetComponent<SquareManager>().GameState];
        }
    }

    void FlipLeft(GameObject HomeSquare, bool Grow)
    {
        for (int z = (HomeSquare.GetComponent<SquareManager>().x - 1); z >= 0; z--)
        {
            GameObject Flipper = BoardStateGO[HomeSquare.GetComponent<SquareManager>().y, z];
            if (Flipper.GetComponent<SquareManager>().GameState == 0)
            {
                break;
            }
            if (Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState + 1;
            }
            if (!Grow)
            {
                Flipper.GetComponent<SquareManager>().GameState = Flipper.GetComponent<SquareManager>().GameState - 1;
            }
            Flipper.GetComponent<SpriteRenderer>().sprite = SpriteList[Flipper.GetComponent<SquareManager>().GameState];
        }
    }

    void CheckWin()
    {
        for (int y = BoardHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                if (BoardStateGO[y, x].GetComponent<SquareManager>().GameState == 1 || BoardStateGO[y, x].GetComponent<SquareManager>().GameState == 5)
                {
                    return;
                }
            }
        }
        Debug.Log("You Win!");
    }
}

