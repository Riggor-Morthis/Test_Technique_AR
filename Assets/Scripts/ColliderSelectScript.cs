using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSelectScript : MonoBehaviour
{
    #region Fields
    private MouseMovementScript mouseMovement;
    #endregion Fields

    #region Methods
    private void Awake()
    {
        mouseMovement = transform.parent.parent.GetComponent<MouseMovementScript>();
    }

    private void OnMouseDown()
    {
        mouseMovement.GetClicked();
    }
    #endregion Methods
}
