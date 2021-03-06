﻿using UnityEngine;

public abstract class PlayerSoldier: MonoBehaviour {

    [SerializeField] private short rank;
    [SerializeField] private int price;
    [SerializeField] protected float offset_x;
    [SerializeField] protected float offset_y;

    private StrategyEditor strategyEditor;
    private SpriteRenderer spriteRenderer;
    private Color blueColor = new Color(0, 0, 1, 0.6f);
    private Color redColor = new Color(1, 0, 0, 0.6f);
    private Color defaultColor = new Color(1, 1, 1, 1);
    protected float originOffsetX, originOffsetY;


    protected Vector3 originPosition;
    protected Animator anim;
    protected RuntimeAnimatorController originAnim;
    protected PolygonCollider2D playerCollider;
    protected bool isHidden;

    private float clickTime;
    private bool longClick;

    public short Rank { get { return rank; } }
    public int Price { get { return price; } }
    public float OffsetX { get { return offset_x; } }
    public float OffsetY { get { return offset_y; } }
    public Tile CurrentTile { get; set; }
    public GameSide CurrentSide { get; set; }
    public Animator Anim { get { return anim; } set { anim = value; } }
    public RuntimeAnimatorController OriginAnim { get { return originAnim; } set { originAnim = value; } }

    public PolygonCollider2D PlayerCollider { get { return playerCollider; } }
    public Vector3 OriginPosition { get { return originPosition; } }

    public abstract void SoldierPlacedInEditMode(bool isSoundActivated);
    public abstract void MakeNoise();

    private void Awake() {
        anim = GetComponent<Animator>();
        originAnim = anim.runtimeAnimatorController;
        playerCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        strategyEditor = StrategyEditor.Instance;
        originOffsetX = offset_x;
        originOffsetY = offset_y;
    }

    public virtual void FlipSide() {
        offset_x = originOffsetX = -offset_x;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        CurrentSide = CurrentSide == GameSide.LeftSide ? GameSide.RightSide : GameSide.LeftSide;
    }

    public bool IsEnemy(PlayerSoldier enemy) {
        return CurrentSide != enemy.CurrentSide;
    }

    protected void OnMouseDown() {
        clickTime = Time.time;
        if(strategyEditor != null && strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            originPosition = transform.position;
            TileManager.Instance.MarkAvailableBuildTiles();
        }
    }

    protected void OnMouseDrag() {
        if(strategyEditor != null && strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
        else if(Globals.IS_IN_GAME && Mathf.Abs(Time.time - clickTime) > 1f && !longClick) {
            longClick = true;
            clickTime = Time.time;
            DisplayInfo();
        }
    }

    private void DisplayInfo() {
        GameManager.Instance.DisplayInfo(this);
        longClick = false;
    }

    protected void OnMouseUp() {
        if(strategyEditor != null && strategyEditor.PlayerBtnPressed == null && StrategyEditor.IsInEdit) {
            TileManager.Instance.UnmarkAvailableBuildTiles();
            if(strategyEditor.ChangeSoldierPosition(this)) {
                MakeNoise();
                originPosition = transform.position;
            }
            else {
                transform.position = originPosition;
            }
        }
    }

    public void HideSoldier(Animator templateAnimator, float offsetX, float offsetY) {
        if(!isHidden) {
            isHidden = true;
            Anim.runtimeAnimatorController = templateAnimator.runtimeAnimatorController;
            spriteRenderer.color = CurrentSide == GameSide.LeftSide ? blueColor : redColor;
            offset_x = CurrentSide == GameSide.LeftSide ? offsetX : -offsetX;
            offset_y = offsetY;
            transform.position = new Vector2(CurrentTile.transform.position.x + offset_x, CurrentTile.transform.position.y + offset_y);
        }
    }

    public void CoverSoldier() {
        if(isHidden) {
            isHidden = false;
            Anim.runtimeAnimatorController = OriginAnim;
            spriteRenderer.color = defaultColor;
            offset_x = originOffsetX;
            offset_y = originOffsetY;
            transform.position = new Vector2(CurrentTile.transform.position.x + offset_x, CurrentTile.transform.position.y + offset_y);
        }
    }
}