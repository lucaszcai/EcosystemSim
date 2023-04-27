using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static TileType;

public class Environment : MonoBehaviour
{
    // map of terrain
    int mapWidth = 300;
    int mapHeight = 300;

    int numWaterBodies = 5;

    public GameObject waterTile;
    public GameObject food;
    public GameObject dino;

    // TODO: change to a list to deal with our food objects and water objects
    int numFood = 0;

    public int numStartingDinos = 25;
    int numDinos = 0;

    public int maxFood = 100;

    TileType[,] terrain;

    List<GameObject> allDinos = new List<GameObject>();

    public Logger logger;

    float lastUpdatedLogger = 0;
    float loggerInterval = 1;

    // Start is called before the first frame update
    void Start()
    {
        initMap();
        initTerrainObjects();
        logger = gameObject.GetComponent<Logger>();

        Debug.Log("done initializing map and terrain");
    }

    // Update is called once per frame
    void Update()
    {
        if (numFood < maxFood) {
            generateFood();
        }

        if(Time.time - lastUpdatedLogger > loggerInterval)
        {
            lastUpdatedLogger = Time.time;
            if(logger == null)
            {
                Debug.Log("logger is null");
            }
            else
            {

                logger.writePopulation(getPopulation());
            }
        }
    }

    void initMap()
    {
        terrain = new TileType[mapWidth, mapHeight];

        for(int r = 0; r < mapHeight; r++)
        {
            for(int c = 0; c < mapWidth; c++)
            {
                terrain[r, c] = TileType.Ground;
            }
        }

        // generate water
        // terrain[0, 0] = TileType.Water;
        //generateWaterBody(15, 15, 10);
        //for (int i = 0; i < 10; i++)
        //{
        //    addWater();
        //}

        // generate a water tile in each quadrant
        //for(int i = 0; i < 4; i++)
        //{

        //}

        // hard coded water locations for now...
        for (int i = 0; i < 3; i++)
        {
            int waterY = 50 + i * 100;
            for (int j = 0; j < 3; j++)
            {
                int waterX = 50 + j * 100;
                addWater(waterX, waterY);
            }
        }

        // spawn in n amount of dinos
        for (int i = 0; i < numStartingDinos; i++)
        {
            spawnDino();
            numDinos++;
        }

        for (int i = 0; i < 3; i++)
        {
            int waterY = i * 100;
            for (int j = 0; j < 3; j++)
            {
                int waterX = j * 100;
                addWater(waterX, waterY);
            }
        }
    }

    void spawnDino()
    {
        // generate random coordinate
        int randX = Mathf.RoundToInt(Random.Range(0, mapWidth));
        int randY = Mathf.RoundToInt(Random.Range(0, mapHeight));

        // check if taken or water, regenerate if it is
        while (terrain[randX, randY] == TileType.Food || terrain[randX, randY] == TileType.Water)
        {
            randX = Mathf.RoundToInt(Random.Range(0, mapWidth));
            randY = Mathf.RoundToInt(Random.Range(0, mapHeight));
        }

        // generate food prefab
        GameObject newDino = Instantiate(dino, new Vector3(randX, 0, randY), Quaternion.identity);

        allDinos.Add(newDino);

        //newFood.GetComponent<FoodBush>().setCoord(randX, randY);
        //Debug.Log("Generated Food at : " + randX + " " + randY);
        //numFood++;

        // mark tile as food
        //terrain[randX, randY] = TileType.Food;
    }

    public void addDino(GameObject dino)
    {
        allDinos.Add(dino);
    }

    public void removeDino(GameObject dino)
    {
        allDinos.Remove(dino);
    }

    int getPopulation()
    {
        int nonNullCount = 0;

        foreach(GameObject dino in allDinos)
        {
            nonNullCount++;
        }

        return nonNullCount;
    }

    void addWater(int waterX, int waterY)
    {
        // x y is bottom left corner

        //Debug.Log("adding water");
        //// generate random coordinate
        //int randX = Mathf.RoundToInt(Random.Range(waterX + 20, waterX + 100 - 20));
        //int randY = Mathf.RoundToInt(Random.Range(waterY + 20, waterY + 100 - 20));

        int randX = waterX;
        int randY = waterY;

        //// check if taken, regenerate if it is
        //// TODO: fix this, still allows for overlap
        //while (terrain[randX, randY] == TileType.Water)
        //{
        //    randX = Mathf.RoundToInt(Random.Range(0, mapWidth));
        //    randY = Mathf.RoundToInt(Random.Range(0, mapHeight));
        //}

        //// generate food prefab
        //GameObject newWater = Instantiate(waterTile, new Vector3(randX, 0, randY), Quaternion.identity);
        ////newFood.GetComponent<FoodBush>().setCoord(randX, randY);
        ////Debug.Log("Generated Food at : " + randX + " " + randY);

        //// since our water is 40 x 40, set those tiles as water
        for (int x = randX - 20; x <= randX + 20; x++)
        {
            for (int y = randY - 20; y <= randY + 20; y++)
            {
                terrain[x, y] = TileType.Water;
            }
        }
    }

    // returns the nearest water given a coord and sensing range
    //Vector2 nearestWater(Vector2 coord)
    //{

    //}

    void generateWaterBody(int r, int c, int rad)
    {
        // TODO: add randomness
        for (int i = 0; i <= rad * 2; i++)
        {
            for (int j = 0; j <= rad * 2; j++)
            {
                int dx = i - rad;
                int dy = j - rad;

                float offset = Random.Range(0f, 5f);

                if (dx * dx + dy * dy + offset < rad * rad)
                {
                    int x = r + dx;
                    int y = c + dy;
                    if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                    {
                        terrain[i, j] = TileType.Water;
                    }
                }
            }
        }
    }

    void generateFood()
    {
        //Debug.Log("generating food");
        // generate random coordinate
        int randX = Mathf.RoundToInt(Random.Range(0, mapWidth));
        int randY = Mathf.RoundToInt(Random.Range(0, mapHeight));

        // check if taken or water, regenerate if it is
        while(terrain[randX, randY] == TileType.Food || terrain[randX, randY] == TileType.Water)
        {
            randX = Mathf.RoundToInt(Random.Range(0, mapWidth));
            randY = Mathf.RoundToInt(Random.Range(0, mapHeight));
        }

        // generate food prefab
        GameObject newFood = Instantiate(food, new Vector3(randX + 0.5f, 0, randY + 0.5f), Quaternion.identity);
        newFood.GetComponent<FoodBush>().setCoord(randX, randY);
        //Debug.Log("Generated Food at : " + randX + " " + randY);
        numFood++;

        // mark tile as food
        terrain[randX, randY] = TileType.Food;
    }

    // call this from plant when it's eaten
    public void removeFood(int r, int c)
    {
        //Debug.Log("removing food from terrain");
        numFood--;
        terrain[r, c] = TileType.Ground;
    }

    void initTerrainObjects()
    {
        for(int r = 0; r < mapHeight; r++)
        {
            for(int c = 0; c < mapWidth; c++)
            {
                //if(terrain[r, c] == TileType.Water)
                //{
                //    Vector3 position = new Vector3(r + 0.5f, -0.25f, c + 0.5f);
                //    GameObject newWater = Instantiate(waterTile, position, Quaternion.identity);
                //}
            }
        }
    }


}
