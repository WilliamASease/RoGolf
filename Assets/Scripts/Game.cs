﻿using GameModeEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using TargetEnum;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace TargetEnum
{
    public enum Target { BALL, CURSOR, FREE }
}

public class Game : MonoBehaviour
{
    public GameController gc;

    public GameMode gameMode;

    // GameObject objects
    private GameObject cameraObject;
    private GameObject ballObject;
    private List<GameObject> cursorList;
    public GameObject freeFocus;
    public float panSensitivity = 1; //for FreeFocus
    public GameObject cursorTextObject;
    public GameObject cursorSubtextObject;

    public Target target;
    public MouseOrbitImproved orbitalControls;

    // Other game objects (that aren't game objects)
    private State state;
    private InputController inputController;

    // Game data objects
    private HoleBag holeBag;
    private ItemBag itemBag;
    private ItemBag badItemBag;
    private PlayerAttributes playerAttributes;
    private TerrainAttributes terrainAttributes;

    // GAME OBJECT (not GameObject)
    private HoleInfo holeInfo;
    private Wind wind;
    private Ball ball;
    private Cursor cursor;
    private CurrentDistance currentDistance;
    private Bag bag;
    private Powerbar powerbar;
    private ShotMode shotMode;
    private Score score;

    // Graphical-related helper classes
    private BallGraphics ballGraphics;
    private CursorGraphics cursorGraphics;
    private GraphicDebug graphicDebug;

    /// <summary>
    /// Performs initialization of Game object.
    /// </summary>
    public void Initialize(GameMode gameMode)
    {
        this.gameMode = gameMode;
        this.state = new NoState(this);
        gc = GameObject.Find(GameController.NAME).GetComponent<GameController>();
        
        // Initialize fields
        this.holeBag = new HoleBag(gameMode);
        this.itemBag = new ItemBag("good");
        this.badItemBag = new ItemBag("bad");
        this.playerAttributes = new PlayerAttributes();
        this.terrainAttributes = new TerrainAttributes();

        inputController = new InputController(this);
        target = Target.BALL;

        wind = new Wind(this);
        ball = new Ball(this);
        cursor = new Cursor(this);
        currentDistance = new CurrentDistance(this);
        bag = new Bag(this);
        powerbar = new Powerbar(this);
        shotMode = new ShotMode(this);
        score = new Score(this);

        // Send Game reference to other objects
        GodOfUI ui = GameObject.Find(GodOfUI.NAME).GetComponent<GodOfUI>();
        ui.gameRef = this;
    }

    public void Begin()
    {
        // Initialize graphics helpers
        ballGraphics = new BallGraphics(this);
        cursorGraphics = new CursorGraphics(this);
        graphicDebug = new GraphicDebug(this);
    }

    /// <summary>
    /// Game Start function.
    /// Just call Initialize().
    /// </summary>
    void Start() { }

    /// <summary>
    /// Game Update function.
    /// Propagate Update to all relevant objects.
    /// </summary>
    void Update()
    {
        inputController.Tick();
        state.Tick();

        // Update camera target position
        if (Input.GetMouseButton(MouseOrbitImproved.LEFT_MOUSE_BUTTON))
        {
            target = Target.FREE;
            orbitalControls.targetPosition = freeFocus.transform.position;

            freeFocus.transform.LookAt(cameraObject.transform);
            freeFocus.transform.Translate(Input.GetAxis("Mouse X") * panSensitivity, 0, Input.GetAxis("Mouse Y") * panSensitivity);
            RaycastHit hit;
            Vector3 positionHigh = new Vector3(freeFocus.transform.position.x, freeFocus.transform.position.y + 1000, freeFocus.transform.position.z);
            Vector3 temp = freeFocus.transform.position;
            if (Physics.Raycast(new Ray(positionHigh, Vector3.down), out hit))
            {
                temp.y = hit.point.y;
                freeFocus.transform.position = temp;
            }
        }
        if (target == Target.BALL) orbitalControls.targetPosition = ball.GetPosition();
        if (target == Target.CURSOR) orbitalControls.targetPosition = cursor.GetPosition();

        ballGraphics.Tick();
        cursorGraphics.Tick();
        graphicDebug.Tick();
    }

    public void SetState(State state)
    {
        this.state.OnStateExit();
        this.state = state;
        this.state.OnStateEnter();
    }

    public void ToggleTarget()
    {
        if (target == Target.CURSOR) ResetTarget();
        else
        {
            target = Target.CURSOR;
            freeFocus.transform.position = cursor.GetPosition();
        }
    }

    public void ResetTarget()
    {
        target = Target.BALL;
        freeFocus.transform.position = ball.GetPosition();
    }

    public void ToggleGraphicDebug() { graphicDebug.Toggle(); }

    public GameMode GetGameMode() { return gameMode; }
    public GameController GetGameController() { return gc; }

    public GameObject GetCameraObject() { return cameraObject; }
    public GameObject GetBallObject() { return ballObject; }
    public List<GameObject> GetCursorList() { return cursorList; }
    public GameObject GetCursorTextObject() { return cursorTextObject; }
    public GameObject GetCursorSubtextObject() { return cursorSubtextObject; }

    public void SetCameraObject(GameObject cameraObject) { this.cameraObject = cameraObject; }
    public void SetBallObject(GameObject ballObject) { this.ballObject = ballObject; }
    public void SetCursorList(List<GameObject> cursorList) { this.cursorList = cursorList; }
    public void SetCursorTextObject(GameObject to) { this.cursorTextObject = to; }
    public void SetCursorSubtextObject(GameObject cursorSubtextObject) { this.cursorSubtextObject = cursorSubtextObject; }

    public State GetState() { return state; }
    
    public HoleBag GetHoleBag() { return holeBag; }
    public ItemBag GetItemBag() { return itemBag; }
    public ItemBag GetBadItemBag() { return badItemBag; }
    public PlayerAttributes GetPlayerAttributes() { return playerAttributes; }
    public TerrainAttributes GetTerrainAttributes() { return terrainAttributes; }
    public void SetHoleBag(HoleBag holeBag) { this.holeBag = holeBag; }
    public void SetItemBag(ItemBag itemBag) { this.itemBag = itemBag; }
    public void SetBadItemBag(ItemBag badItemBag) {this.badItemBag = badItemBag;}
    public void SetPlayerAttributes(PlayerAttributes playerAttributes) { this.playerAttributes = playerAttributes; }
    public void SetTerrainAttributes(TerrainAttributes terrainAttributes) { this.terrainAttributes = terrainAttributes; }
    public void SetHoleInfo(HoleInfo holeInfo) { this.holeInfo = holeInfo; }
    public void SetPanSensitivity(float n) {this.panSensitivity = n;}

    public Target GetTarget() { return target; }
    public HoleInfo GetHoleInfo() { return holeInfo; }
    public Wind GetWind() { return wind; }
    public Ball GetBall() { return ball; }
    public GameObject GetFreeFocus() { return freeFocus; }
    public Cursor GetCursor() { return cursor; }
    public CurrentDistance GetCurrentDistance() { return currentDistance; }
    public Bag GetBag() { return bag; }
    public Powerbar GetPowerbar() { return powerbar; }
    public ShotMode GetShotMode() { return shotMode; }
    public Score GetScore() { return score; }

    public CursorGraphics GetCursorGraphics() { return cursorGraphics; }
}
