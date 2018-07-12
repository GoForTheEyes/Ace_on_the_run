using UnityEngine;

public class Enemy : Character
{
    [SerializeField] float scoreValue;

    protected override void Dead()
    {
        ScoreManager.instance.UpdateScoreExtra(scoreValue);
        base.Dead();
    }

}
