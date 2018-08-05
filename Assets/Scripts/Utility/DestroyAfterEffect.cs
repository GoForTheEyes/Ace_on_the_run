using UnityEngine;

public class DestroyAfterEffect : MonoBehaviour
{
    public void EliminateFromScene()
    {
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }

	
}
