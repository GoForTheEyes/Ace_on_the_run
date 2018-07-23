using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorWireGizmo : MonoBehaviour {

    float _wireRadius = 0.1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _wireRadius);
    }
}
