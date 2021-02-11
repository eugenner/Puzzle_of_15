using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InitBoard : MonoBehaviour
{


    public struct TilePosition
    {
        public int row;
        public int col;
        public bool isEmpty;
        public int tileNumber;
        public Vector3 tilePivot;
    }

    public bool isShaffleMode;
    public TilePosition[,] tilePositions = new TilePosition[4, 4];
    private GameObject[] tiles;
    public Vector3[] tilePivot = new Vector3[16];

    ArrayList checkPositions = new ArrayList();


    private bool isTileMoving = false;
    private GameObject fromPositionGO;
    private Vector3 toPosition;

    private bool isBoardInitialized;
    private float startTime;
    private bool isGameStarted;

    // Start is called before the first frame update
    void Start()
    {
        if (isBoardInitialized) return;
        Debug.Log("InitBoard Start()");
        checkPositions.Add(new int[] { -1, 0 });
        checkPositions.Add(new int[] { 1, 0 });
        checkPositions.Add(new int[] { 0, -1 });
        checkPositions.Add(new int[] { 0, 1 });

        // remember tiles pivot coordinates
        tiles = GameObject.FindGameObjectsWithTag("tile");
        Debug.Log("tiles count: " + tiles.Length);
        foreach (var tile in tiles)
        {
            int ind = Int32.Parse(tile.name.Replace("t", ""));
            tilePivot[ind - 1] = tile.transform.position;

            if (tile.name == "t16") // the fake tile just to get the pivot coordinates
            {
                tile.SetActive(false);
            }
        }

        // init TilePosition table
        int pNumber = 1;
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                Vector3 pc = tilePivot[row * 4 + col];
                tilePositions[row, col] = new TilePosition
                {
                    row = row,
                    col = col,
                    isEmpty = false,
                    tileNumber = pNumber,
                    tilePivot = pc
                };
                pNumber++;
            }
        }

        tilePositions[3, 3].isEmpty = true; // 16th piece is empty
        isBoardInitialized = true;
    }

    public bool TileMove(GameObject tile)
    {
        if(!isGameStarted)
            return false;
        if (isTileMoving)
            return false;
        if (!tile.name.StartsWith("t"))
            return false;

        Debug.Log("Moving tile: " + tile.name);

        int tileIndex = Int32.Parse(tile.name.Replace("t", ""));
        TilePosition currentTP = new TilePosition { };

        foreach (var tp in tilePositions)
        {
            if (tp.tileNumber == tileIndex)
            {
                currentTP = tp;
                break;
            }
        }

        bool canMove = false;

        // check the all four nearest cells
        foreach (var cp in checkPositions)
        {
            int r = ((int[])cp)[0];
            int c = ((int[])cp)[1];
            int nextRow = currentTP.row + r;
            int nextCol = currentTP.col + c;

            if ( // check if it is possible to move the current tile (will not out of the Board)
                ((nextCol >= 0 && nextCol <= 3) || c == 0) &&
                ((nextRow >= 0 && nextRow <= 3) || r == 0)
                )
            {   // check if the choosed place is free
                if (tilePositions[nextRow, nextCol].isEmpty)
                {
                    canMove = true;

                    // the current tile position
                    tilePositions[currentTP.row, currentTP.col].isEmpty = true;
                    tilePositions[currentTP.row, currentTP.col].tileNumber = 16;


                    // next position of the tile
                    tilePositions[nextRow, nextCol].isEmpty = false;
                    tilePositions[nextRow, nextCol].tileNumber = tileIndex;


                    //tile.transform.position = tilePositions[nextRow, nextCol].tilePivot;
                    fromPositionGO = tile;
                    toPosition = tilePositions[nextRow, nextCol].tilePivot;
                    isTileMoving = true;

                    break;
                }
            }
        }
        CheckGameEndPosition();
        return canMove;
    }

    public bool CheckGameEndPosition()
    {
        bool isGameComplete = true;
        for (int row = 0; row < 4; row++)
        {
            if (!isGameComplete)
                break;
            for (int col = 0; col < 4; col++)
            {
                int tileNo = row * 4 + col + 1;
                if(tilePositions[row, col].tileNumber != tileNo)
                {
                    isGameComplete = false;
                    break;
                }
            }
        }
        if (isGameComplete)
            isGameStarted = false;
        // TODO show user the "game complete" status
        UnityEngine.UI.Text text = GameObject.Find("Text").GetComponent<UnityEngine.UI.Text>();
        text.text = "Your result:";
        return isGameComplete;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isShaffleMode)
        {
            return;
        }

        if(isGameStarted)
        {
            UnityEngine.UI.Text text = GameObject.Find("log_text").GetComponent<UnityEngine.UI.Text>();
            float val = Mathf.Floor(10 * (Time.time - startTime)) / 10;
            text.text = val.ToString("F1") + " seconds";
        }


        if (isTileMoving)
        {
            fromPositionGO.transform.position = Vector3.MoveTowards(fromPositionGO.transform.position, toPosition,
                Time.deltaTime / 2.0f);

            if (Vector3.Distance(fromPositionGO.transform.position, toPosition) < 0.01f)
            {
                fromPositionGO.transform.position = toPosition;
                isTileMoving = false;
            }

        }
    }

    public void OnExitBtnClick()
    {
        Application.Quit();
    }

    public void OnShuffleBtnClick()
    {
        StartCoroutine(ShuffleCoroutine());
    }

    private IEnumerator ShuffleCoroutine()
    {

        UnityEngine.UI.Text text = GameObject.Find("Text").GetComponent<UnityEngine.UI.Text>();
        text.text = "Shaffle";
        ShuffleTiles();
        isGameStarted = true;
        startTime = Time.time;
        yield return new WaitForSeconds(1);
        text.text = "Game started";
    }

    public void ShuffleTiles()
    {
        isShaffleMode = true;
        GameObject board = GameObject.Find("Board");
        board.transform.Find("t16").gameObject.SetActive(true);
        ArrayList randomizedList = RandomizeList(new ArrayList(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }));


        // count the number of inversions
        int inversionCount = 1;
        while((inversionCount % 2) != 0)
        {
            randomizedList = RandomizeList(randomizedList);
            inversionCount = InversionCount(randomizedList);
            Debug.Log("inversionCount: " + inversionCount);
        }

        // shaffle tilePositions[] data
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                Int32 ind = row * 4 + col;
                int newInd = (int)randomizedList[ind];
                tilePositions[row, col].tileNumber = newInd + 1;
                tilePositions[row, col].isEmpty = false;
                if (newInd == 15)
                    tilePositions[row, col].isEmpty = true;
            }
        }

        // moving tiles to its new positions
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                String tileName = "t" + tilePositions[row, col].tileNumber;
                GameObject go = GameObject.Find(tileName);
                go.transform.position = tilePivot[row * 4 + col];
            }
        }

        GameObject.Find("t16").SetActive(false);

        isShaffleMode = false;
    }

    private ArrayList RandomizeList(ArrayList list)
    {

        int rndInt;
        int i;
        for (i = 0; i < 100; i++)
        {
            rndInt = Mathf.CeilToInt(Random.Range(0, 16));
            var el = list[rndInt];
            list.Remove(el);
            list.Add(el);
        }

        i = 0;
        foreach (var el in list)
        {
            Debug.Log("i: " + i++ + " el: " + el);
        }

        return list;
    }

    public int InversionCount(ArrayList list)
    {
        int inversionCount = 0;
        for(int i = 0; i < list.Count; i++)
        {
            int el = (int)list[i];
            if (el == 15) // 15 - blank tile (but important to add it's row number (starting from 1))
            {
                inversionCount += i / 4 + 1;
                continue;
            }
            // count all the next elements after el and if (el_next < el) { increase the inversionCount }
            for (int j = i + 1; j < list.Count; j++)
            {
                var next_el = (int)list[j];
                if (next_el == 15)
                    continue;                     
                if (next_el < el)
                {
                    inversionCount++;
                }
            }
        }
        return inversionCount;
    }
}
