using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDSprites;
    [SerializeField] List<Sprite> walkUSprites;
    [SerializeField] List<Sprite> walkLSprites;
    [SerializeField] List<Sprite> walkRSprites;
    [SerializeField] FacingDirection defaultDir = FacingDirection.Down;
    // parameters
    public float moveX { get; set; }
    public float moveY { get; set; }
    public bool isMoving { get; set; }

    //states
    SpriteAnimator walkDAnim;
    SpriteAnimator walkUAnim;
    SpriteAnimator walkLAnim;
    SpriteAnimator walkRAnim;

    SpriteAnimator currAnim;
    bool wasMoving;

    //references
    SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDAnim = new SpriteAnimator(spriteRenderer, walkDSprites);
        walkUAnim = new SpriteAnimator(spriteRenderer, walkUSprites);
        walkLAnim = new SpriteAnimator(spriteRenderer, walkLSprites);
        walkRAnim = new SpriteAnimator(spriteRenderer, walkRSprites);
        SetFacingDirection(defaultDir);

        currAnim = walkDAnim;
    }

    private void Update()
    {
        var prevAnim = currAnim;

        if (moveX == 1)
            currAnim = walkRAnim;
        else if (moveX == -1)
            currAnim = walkLAnim;
        else if (moveY == 1)
            currAnim = walkUAnim;
        else if (moveY == -1)
            currAnim = walkDAnim;

        if (currAnim != prevAnim || isMoving != wasMoving)
        {
            currAnim.Start();
        }

        if (isMoving)
            currAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currAnim.Frames[0];

        wasMoving = isMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if (dir == FacingDirection.Right)
            moveX = 1;
        else if (dir == FacingDirection.Left)
            moveX = -1;
        else if (dir == FacingDirection.Down)
            moveY = -1;
        else if (dir == FacingDirection.Up)
            moveY = 1;
    }

    public FacingDirection DefaultDir
    {
        get => defaultDir;
    }
}

public enum FacingDirection { Up, Down, Left, Right}