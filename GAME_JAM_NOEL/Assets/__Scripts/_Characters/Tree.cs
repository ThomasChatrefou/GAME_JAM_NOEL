using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Enemy
{
    private bool isInMoveMode;
    private float timer;
    [Header("Tree")]
    [SerializeField] private Sprite moveModeSprite;
    [SerializeField] private Sprite invincibleModeSprite;
    [SerializeField] private float moveModeDuration;
    [SerializeField] private float invincibleModeDuration;

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
            SwitchMode();
    }

    protected override void FixedUpdate()
    {
        if (isInMoveMode && target != null)
            base.FixedUpdate();
    }

    public void SwitchMode()
    {
        isInMoveMode = !isInMoveMode;
        timer = (isInMoveMode ? moveModeDuration : invincibleModeDuration);
        spriteRenderer.sprite = (isInMoveMode ? moveModeSprite : invincibleModeSprite);
    }

    public override void TakeDamage(Projectile projectile)
    {
        if(isInMoveMode)
            base.TakeDamage(projectile);
    }
}
