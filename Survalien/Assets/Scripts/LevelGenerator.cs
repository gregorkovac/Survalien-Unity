using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int size = 10;
    public int maxSpaceParts = 3;

    public GameObject[] freeTiles;
    public GameObject[] wallTiles;
    public GameObject[] spacePartTiles;
    public GameObject mapBorder;
    public GameObject startTile;

    private int[,] level;
    private bool isLevelGood;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        level = new int[size, size];
        isLevelGood = false;

        while (!isLevelGood)
            Generate();

        Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Generate()
    {
        // Set all to 0
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    level[i, j] = 3;
                else
                    level[i, j] = 0;
            }
        }

        // Pick start tile
        int startX = Random.Range(1, size - 1);
        int startY = Random.Range(1, size - 1);

        /*

        0 = wall
        1 = path
        2 = space part
        3 = border
        10 = start
        */

        level[startY, startX] = 10;

        int spacePartCount = 0;

        // Create empty stack
        Stack<int[]> stack = new Stack<int[]>();

        // Push start tile to stack
        stack.Push(new int[] { startX, startY, 0 });

        // While stack is not empty
        while (stack.Count > 0) {
            int[] tile = stack.Pop();

            if (level[tile[1], tile[0]] == 0)
                level[tile[1], tile[0]] = 1;

            // Randomly set space part to tile
            if (tile[2] > 5 && Random.Range(0, 100) < Mathf.Min(100, tile[2]*2)) {
                level[tile[1], tile[0]] = 2;
                spacePartCount++;

                if (spacePartCount >= maxSpaceParts)
                    break;
                else if (spacePartCount >= maxSpaceParts / 2)
                    isLevelGood = true;
                continue;
            }

            // Randomly choose neighbours to continue path
            for (int i = 0; i < 4; i++) {
                if (Random.Range(0, 100) > 20) {
                    int[] neighbour = new int[3];
                    
                    neighbour[0] = tile[0];
                    neighbour[1] = tile[1];
                    neighbour[2] = tile[2] + 1;

                    if (i == 0) neighbour[0] += 1;
                    else if (i == 1) neighbour[0] -= 1;
                    else if (i == 2) neighbour[1] += 1;
                    else if (i == 3) neighbour[1] -= 1;

                    if (neighbour[0] < 0 || neighbour[0] >= size || neighbour[1] < 0 || neighbour[1] >= size || level[neighbour[1], neighbour[0]] != 0)
                        continue;

                    stack.Push(neighbour);
                }
            }
        }

        // Add random deformations
        for (int i = 1; i < size - 1; i++) {
            for (int j = 1; j < size - 1; j++) {
                if (level[i, j] == 1 && Random.Range(0, 100) < 50 && level[i + 1, j] == 1 && level[i - 1, j] == 1 && level[i, j + 1] == 1 && level[i, j - 1] == 1 &&
                    level[i + 1, j + 1] == 1 && level[i - 1, j - 1] == 1 && level[i + 1, j - 1] == 1 && level[i - 1, j + 1] == 1) {
                        level[i, j] = 0;
                }
                else if (level[i, j] == 0 && Random.Range(0, 100) < 50 && (level[i + 1, j] != 0 || level[i - 1, j] != 0 || level[i, j + 1] != 0 || level[i, j - 1] != 0)) {
                    level[i, j] = 1;
                }
            }
        }

        string levelString = "";

        // make levelString string
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (level[i, j] == 0) 
                    levelString += "#";
                else if (level[i, j] == 1)
                    levelString += "O";
                else if (level[i, j] == 2)
                    levelString += "S";
                else if (level[i, j] == 10)
                    levelString += "X";
            }
            levelString += "\n";
        }

        Debug.Log(levelString);


    }

    private void Create() {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {

                int x = j * 20;
                int z = i * 20;

                switch (level[i, j]) {
                    case 0: // Wall
                        Instantiate(wallTiles[Random.Range(0, wallTiles.Length)], new Vector3(x, 0, z), Quaternion.identity);
                    break;

                    case 1: // Path
                        Instantiate(freeTiles[Random.Range(0, freeTiles.Length)], new Vector3(x, 0, z), Quaternion.identity);
                    break;

                    case 2: // Space part
                        Instantiate(spacePartTiles[Random.Range(0, spacePartTiles.Length)], new Vector3(x, 0, z), Quaternion.identity);
                    break;

                    case 3: // Border
                        Instantiate(mapBorder, new Vector3(x, 0, z), Quaternion.identity);
                    break;

                    case 10: // Start
                        Instantiate(startTile, new Vector3(x, 0, z), Quaternion.identity);
                        player.transform.position = new Vector3(x, 2, z);

                    break;
                }
            }
        }
    }
}
