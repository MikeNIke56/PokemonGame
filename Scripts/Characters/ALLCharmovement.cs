using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALLCharmovement : MonoBehaviour
{
    CharacterAnimator animator;
    public float moveSpeed;
    public bool IsMoving { get; private set; }

    public float OffsetY { get; private set; } = .3f;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }
    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + .5f;
        pos.y = Mathf.Floor(pos.y) + .5f + OffsetY;

        transform.position = pos;
    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver=null)
    {
        animator.moveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.moveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if(!IsPathClear(targetPos))
        {
            yield break;
        }

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.isMoving = IsMoving;
    }
    private bool IsPathClear(Vector3 targetpos)
    {
        var diff = targetpos - transform.position;
        var dir = diff.normalized;
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(.2f, .2f), 0f, dir, diff.magnitude - 1, GameLayers.I.SolidLayer | GameLayers.I.InteracbleLayer | GameLayers.I.PlayerLayer) == true)
            return false;
        return true;
    }
    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.I.SolidLayer | GameLayers.I.InteracbleLayer) != null)
        {
            return false;
        }

        return true;
    }
    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.moveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.moveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.LogError("The character cant look that way");
    }

    public CharacterAnimator Animator
    {
        get => animator;
    }
}
