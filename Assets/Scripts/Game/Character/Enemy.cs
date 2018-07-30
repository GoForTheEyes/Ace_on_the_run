using UnityEngine;

public class Enemy : Character
{
#pragma warning disable 0649
    [SerializeField] float scoreValue;
#pragma warning restore

    protected override void Dead()
    {
        ScoreManager.instance.UpdateScoreExtra(scoreValue);
        base.Dead();
    }

}
