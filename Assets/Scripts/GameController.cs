﻿using System;
using System.Collections;
using System.Collections.Generic;
using TeeEnum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public const string NAME = "GameController";

    public GameObject camera;
    public GameObject ball;
    public GameObject teeFront;
    public GameObject teeBack;

    public Material green;
    public Material fairway;
    public Material rough;
    public Material bunker;
    public Material water;

    public Canvas gameUI;

    void Start()
    {
        // We need to control the game for the whole game! Don't we?!?
        gameUI.enabled = false;
        DontDestroyOnLoad(this);
    }

    void Update() { }

    public static GameController GetInstance()
    {
        GameObject gameObject = GameObject.Find(NAME);
        if (gameObject != null)
        {
            return gameObject.GetComponent<GameController>();
        }
        else
        {
            throw new InvalidOperationException("GameController GameObject not found.");
        }
    }

    /// <summary>
    /// Called from the main menu.
    /// Resets old game data, and initializes new data.
    /// Starts the game loop by loading the first hole.
    /// </summary>
    public void StartGame()
    {
        gameUI.enabled = true;
        GameDataManager.ResetGameData();

        GameObject godObject = GodObject.Create();
        godObject.AddComponent<Game>();
        Game game = godObject.GetComponent<Game>();
        game.CreateGameData();

        // Get next hole
        string nextHole = game.GetHoleBag().GetHole();
        
        // Save and destroy
        GameDataManager.SaveGameData(game);
        UnityEngine.Object.Destroy(godObject);

        // Load scene
        LoadScene(nextHole);
    }
    
    public void LoadScene(string nextHole)
    {
        StartCoroutine(AsyncSceneLoad(nextHole));
    }

    /// <summary>
    /// Loads the scene asynchronously, then preps the new scene by calling NextHole()
    /// </summary>
    /// <param name="targetScene">Name of the new scene to load</param>
    /// <returns></returns>
    IEnumerator AsyncSceneLoad(string targetScene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        NextHole();
    }

    public void NextHole()
    {
        // Load persistent game data
        GameObject godObject = GodObject.Create();
        godObject.AddComponent<Game>();
        Game game = godObject.GetComponent<Game>();
        game.Init();

        /* Modify Scene */
        //  Add lighting
        // TODO - we're just using stock lighting for now

        //  Add ball
        ball = Instantiate(ball);
        game.ballObject = ball;

        // Add camera and controls
        camera = Instantiate(camera);
        MouseOrbitImproved orbitalControls = camera.GetComponent<MouseOrbitImproved>();
        game.orbitalControls = orbitalControls;

        string holeName = SceneManager.GetActiveScene().name;

        // Modify blender scene
        GameObject terrain = GameObject.Find(holeName);
        Transform[] allChildren;
        try
        {
            allChildren = terrain.GetComponentsInChildren<Transform>();
        }
        catch (NullReferenceException e)
        {
            throw new InvalidOperationException("Terrain not found on scene " + holeName + ". Is the .blend file missing in the project?");
        }

        // Create props lists to process after ground colliders have been created
        List<GameObject> pinList = new List<GameObject>();
        GameObject teeFrontTemp = null;
        GameObject teeBackTemp = null;
        List<GameObject> treeList = new List<GameObject>();

        foreach (Transform child in allChildren)
        {
            GameObject childObject = child.gameObject;
            string childName = childObject.name;
            // Skip iteration if component is the parent
            if (childName == holeName || childName == NAME) continue;
            // Else if prop, add to approprate prop list
            else if (childName.StartsWith("Pin"))
            {
                pinList.Add(childObject);
            }
            else if (childName.StartsWith("TeeF"))
            {
                teeFrontTemp = childObject;
            }
            else if (childName.StartsWith("TeeB"))
            {
                teeBackTemp = childObject;
            }
            else if (childName.StartsWith("Tree"))
            {
                treeList.Add(childObject);
            }
            // Else a ground mesh
            else
            {
                // Add mesh collier
                childObject.AddComponent<MeshCollider>();

                // Modify materials of ground
                Renderer renderer = childObject.GetComponent<Renderer>();
                char type = childName[0];
                switch (type)
                {
                    case 'B':
                        renderer.material = bunker;
                        break;
                    case 'F':
                        renderer.material = fairway;
                        break;
                    case 'G':
                        renderer.material = green;
                        break;
                    case 'R':
                        renderer.material = rough;
                        break;
                    case 'W':
                        renderer.material = water;
                        break;
                    default:
                        UnityEngine.Debug.Log("Candidate material not found for mesh " + childName);
                        break;
                }
            }
        }

        // Sentinel position to check for errors
        Vector3 nullPosition = new Vector3(Single.NaN, Single.NaN, Single.NaN);

        // Get pin index and add prop
        int pinIndex = UnityEngine.Random.Range(0, pinList.Count-1);
        Vector3 holePosition = nullPosition;
        for (int i = 0; i < pinList.Count; i++)
        {
            GameObject pin = pinList[i];
            if (i == pinIndex)
            {
                // TODO - actually pass in the pin prefab instead of null
                Vector3? maybeHolePosition = AddProp(pin, null);
                holePosition = maybeHolePosition ?? nullPosition;
                if (holePosition == nullPosition)
                {
                    throw new InvalidOperationException("Pin" + pinIndex + " not found. Is there no corresponding Pin object in the .blender file, or is the Raycast not working?");
                }
            }
            else
            {
                AddProp(pin, null);
            }
        }

        // Add trees
        foreach (GameObject tree in treeList)
        {
            AddProp(tree, null);
        }

        // Add tee props and get tee position
        Vector3? maybeTeeFrontPosition = AddProp(teeFrontTemp, teeFront);
        Vector3? maybeTeeBackPosition = AddProp(teeBackTemp, teeBack);
        Vector3 teeFrontPosition = maybeTeeFrontPosition ?? nullPosition;
        Vector3 teeBackPosition = maybeTeeBackPosition ?? nullPosition;
        // Verify tees
        if (teeFrontPosition == nullPosition)
        {
            throw new InvalidOperationException("Front tee not found. Is there no TeeFront object in the .blender file, or is the Raycast not working?");
        }
        if (teeBackPosition == nullPosition)
        {
            throw new InvalidOperationException("Back tee not found. Is there no TeeBack object in the .blender file, or is the Raycast not working?");
        }

        // Set HoleInfo
        game.SetHoleInfo(new HoleInfo(game.GetHoleBag().GetHoleCount(), Tee.FRONT, teeFrontPosition, teeBackPosition, holePosition));

        // Set ball
        game.GetBall().SetPosition(game.GetHoleInfo().GetTeePosition());

        // Reset per-hole data
        //game.ResetStrokes();

        // Set state
        game.SetState(new PrepareState(game));
    }
    
    // Vector3 is a non-nullable type; we need the '?' operator to be able to null it.
    public Vector3? AddProp(GameObject gameObject, GameObject prop)
    {
        if (gameObject == null)
        {
            UnityEngine.Debug.Log(prop.name + "does not exist in .blender file");
            return null;
        }
        try
        {
            RaycastHit hit = RaycastVertical(gameObject);
            Destroy(gameObject);
            if (prop != null)
            {
                prop = Instantiate(prop);
                prop.transform.position = hit.point;
            }
            return hit.point;
        }
        catch (InvalidOperationException e)
        {
            UnityEngine.Debug.Log(e);
            return null;
        }
    }

    public RaycastHit RaycastVertical(GameObject gameObject)
    {
        RaycastHit hit;
        // Check down
        if (Physics.Raycast(new Ray(gameObject.transform.position, Vector3.down), out hit))
        {
            return hit;
        }
        // Check up
        else if (Physics.Raycast(new Ray(gameObject.transform.position, Vector3.up), out hit))
        {
            return hit;
        }
        // Exception if not found
        else
        {
            throw new InvalidOperationException("RaycastHit not found for " + gameObject.name);
        }
    }
}
