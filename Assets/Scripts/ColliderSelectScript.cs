using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSelectScript : MonoBehaviour
{
    #region Fields
    private MouseMovementScript mouseMovement; //Le script pour nous bouger
    #endregion Fields

    #region Methods
    private void Awake()
    {
        mouseMovement = GetComponentInParent<MouseMovementScript>();
    }

    private void OnMouseDown()
    {
        mouseMovement.GetClicked();
    }
    #endregion Methods
}
