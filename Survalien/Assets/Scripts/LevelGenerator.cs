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
    public GameObject enemy;
    public GameObject civilian;

    public GameObject[] spaceParts;

    private int[,] level1;
    private int[,] level2;
    private int[,] level3;
    private bool isLevelGood;
    private GameObject player;
    private GameObject spaceship;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spaceship = GameObject.FindGameObjectWithTag("Spaceship");

        level1 = new int[size, size];
        level2 = new int[size, size];
        level3 = new int[size, size];
        

        for (int i = 1; i <= 3; i++) {
            isLevelGood = false;
           // while (!isLevelGood)
                if (i == 1) Generate(i, level1);
                else if (i == 2) Generate(i, level2);
                else if (i == 3) Generate(i, level3);
        }

        Create(level1, 0, 0);
        // Create(level2, - size/2 * 20, 70);
        // Create(level3, 70, - size * 17 + 10);

        SpawnNPCs(level1, 0, 0, 30);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Generate(int levelNum, int[,] level)
    {

        // FIXME: Spawnaj space part
        // FIXME: Pot do space parta ni nujno reachable

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
        int pathCount = 0;

        // Create empty stack
        Stack<int[]> stack = new Stack<int[]>();

        // Push start tile to stack
        stack.Push(new int[] { startX, startY, 0 });

        bool isFirst = true;

        // While stack is not empty
        while (stack.Count > 0) {
            int[] tile = stack.Pop();

            if (level[tile[1], tile[0]] == 0) {
                level[tile[1], tile[0]] = 1;
                pathCount++;
            }

            // Randomly set space part to tile
          /*  if (tile[2] > 5 && Random.Range(0, 100) < Mathf.Min(100, tile[2]*2)) {
                level[tile[1], tile[0]] = 2;
                spacePartCount++;

                if (spacePartCount >= maxSpaceParts)
                    break;
                else if (spacePartCount >= maxSpaceParts / 2)
                    isLevelGood = true;
                continue;
            }*/

            // Randomly choose neighbours to continue path
            for (int i = 0; i < 4; i++) {
                if (Random.Range(0, 100) > tile[2] * 7 || isFirst) {

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

            isFirst = false;
        }

        int spacePartRandom = Random.Range(pathCount/2, pathCount - 1);

        int pathCountCurr = 0;
        //int spacePartCnt = 0;

        // Add random deformations
        for (int i = 1; i < size - 1; i++) {
            for (int j = 1; j < size - 1; j++) {
                /*if (level[i, j] == 1 && Random.Range(0, 100) < 50 && level[i + 1, j] == 1 && level[i - 1, j] == 1 && level[i, j + 1] == 1 && level[i, j - 1] == 1 &&
                    level[i + 1, j + 1] == 1 && level[i - 1, j - 1] == 1 && level[i + 1, j - 1] == 1 && level[i - 1, j + 1] == 1) {
                        level[i, j] = 0;
                }
                else */if (level[i, j] == 0 && Random.Range(0, 100) < 10 && (level[i + 1, j] != 0 || level[i - 1, j] != 0 || level[i, j + 1] != 0 || level[i, j - 1] != 0)) {
                    level[i, j] = 1;
                } else if (level[i, j] == 1 && pathCountCurr < spacePartRandom) {
                    pathCountCurr++;
                    if (pathCountCurr >= spacePartRandom) {
                        //level[i, j] = 2;
                        //spacePartCnt++;
                    } else if (Random.Range(0, 100) < 20 && level[i + 1, j] != 0 && level[i - 1, j] != 0 && level[i, j + 1] != 0 && level[i, j - 1] != 0 && level[i + 1, j + 1] != 0 && level[i - 1, j - 1] != 0 && level[i + 1, j - 1] != 0 && level[i - 1, j + 1] != 0) {
                        level[i, j] = 0;
                    }
                }
            }
        }

        for (int i = 0; i < 3; i++) {
            int spacePartX, spacePartY;
            do {
                spacePartX = Random.Range(1, size - 1);
                spacePartY = Random.Range(1, size - 1);
            } while (level[spacePartY, spacePartX] != 1);

            level[spacePartY, spacePartX] = 2;
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

    private void Create(int[,] level, int offsetX, int offsetY) {

        int spacePartCnt = 0;

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {

                int x = j * 20 + offsetX;
                int z = i * 20 + offsetY;

                switch (level[i, j]) {
                    case 0: // Wall
                        Instantiate(wallTiles[Random.Range(0, wallTiles.Length)], new Vector3(x, 0, z), Quaternion.identity);
                    break;

                    case 1: // Path
                        Instantiate(freeTiles[Random.Range(0, freeTiles.Length)], new Vector3(x, 0, z), Quaternion.identity);
                    break;

                    case 2: // Space part
                        Instantiate(spacePartTiles[Random.Range(0, spacePartTiles.Length)], new Vector3(x, 0, z), Quaternion.identity);
                        Instantiate(spaceParts[spacePartCnt], new Vector3(x, 2, z), Quaternion.identity);
                        spacePartCnt++;
                    break;

                    case 3: // Border
                        Instantiate(mapBorder, new Vector3(x, 0, z), Quaternion.identity);
                    break;

                    case 10: // Start
                        Instantiate(startTile, new Vector3(x, 0, z), Quaternion.identity);
                        player.transform.position = new Vector3(x, 2, z - 4);
                        spaceship.transform.position = new Vector3(x, 2, z + 1);
                    break;
                }
            }
        }
    }

    private void SpawnNPCs(int[,] level, int offsetX, int offsetY, int difficulty) {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {

                int x = j * 20 + offsetX;
                int z = i * 20 + offsetY;

                switch (level[i, j]) {
                    case 1:
                        for (int k = 0; k < 5; k ++) {
                            if (Random.Range(0, 100) < difficulty) {
                                Instantiate(enemy, new Vector3(x + k , 2, z + k), Quaternion.identity);
                            } else {
                                break;
                            }

                            if (Random.Range(0, 100) < 30) {
                                Instantiate(civilian, new Vector3(x - k, 2, z - k), Quaternion.identity);
                            }
                        }
                    break;

                    break;
                }
            }
        }
    }
}
