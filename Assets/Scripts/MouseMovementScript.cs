using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovementScript : MonoBehaviour
{
    #region Fields
    //Privates
    [SerializeField, Range(1f, 100f)] private float wheelSpeed = 10f;

    private bool isSelected = false;
    private bool oneFrame = false; //Pour s'assurer qu'on capte pas l'input de selection et de deselection durant la meme frame
    private Vector3 previousMouseProjection, currentMouseProjection;
    private Camera mainCamera;
    private Plane flatPlane = new Plane(Vector3.up, Vector3.zero);
    private float currentDistance;
    private Ray currentRay;
    private Vector3 currentMovementVector;
    private bool verticalTranslation = false;
    #endregion Fields

    #region Methods
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Lorsqu'on nous clique dessus on peut se mettre "en marche, sauf si on est deja en marche
    /// </summary>
    public void GetClicked()
    {
        if (!isSelected)
        {
            //Initialisation de nos bool
            isSelected = true;
            oneFrame = true;
            //On redefinit notre plan de projection au passage
            flatPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y * .5f, 0f));
            //On recupere un point de reference
            previousMouseProjection = GetMouseProjection();
        }
    }

    private void Update()
    {
        //On agit que si on est selectionne
        if (isSelected)
        {
            //On agit pas si on est deselectionne
            if (Input.GetMouseButtonDown(0) && !oneFrame) isSelected = false;
            else
            {
                //Si on est a la premiere frame, on arrete d'etre a la premiere frame
                if (oneFrame) oneFrame = false;

                //Est-ce qu'on passe en mode translation verticale ?
                if (Input.GetMouseButtonDown(1))
                {
                    verticalTranslation = true;
                    previousMouseProjection = GetVerticalMouse();
                }
                //Est-ce qu'on passe en mode translation horizontale ?
                else if(Input.GetMouseButtonUp(1) && verticalTranslation)
                {
                    verticalTranslation = false;
                    previousMouseProjection = GetMouseProjection();
                }

                //On s'assure qu'on fait bien la bonne translation
                if (!verticalTranslation) FollowTheMouse();
                else FollowTheVerticalMouse();

                //Quoi qu'il arrive on peut faire tourner le sceau
                WheelRotation();
            }
        }
    }

    /// <summary>
    /// Permet d'obtenir la position de la souris projetee sur le plan horizontal
    /// </summary>
    /// <returns>La position de la souris apres projection</returns>
    private Vector3 GetMouseProjection()
    {
        currentRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (flatPlane.Raycast(currentRay, out currentDistance)) return currentRay.GetPoint(currentDistance);
        return Vector3.zero;
    }

    /// <summary>
    /// Permet d'obtenir une projection modifiee pour etre utilisable lors de la translation sur l'axe y
    /// </summary>
    /// <returns>La position modifiee</returns>
    private Vector3 GetVerticalMouse()
    {
        currentRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (flatPlane.Raycast(currentRay, out currentDistance)) return new Vector3(0f, currentRay.GetPoint(currentDistance).z, 0f);
        return Vector3.zero;
    }

    /// <summary>
    /// Permet de suivre la souris a la trace
    /// </summary>
    private void FollowTheMouse()
    {
        currentMouseProjection = GetMouseProjection();
        MoveTheMouse();
    }

    /// <summary>
    /// Permet de suivre l'elevation de la souris a la trace
    /// </summary>
    private void FollowTheVerticalMouse()
    {
        currentMouseProjection = GetVerticalMouse();
        MoveTheMouse();
    }

    /// <summary>
    /// Applique nos vecteurs a notre souris
    /// </summary>
    private void MoveTheMouse()
    {
        currentMovementVector = currentMouseProjection - previousMouseProjection;
        transform.position += currentMovementVector;
        previousMouseProjection = currentMouseProjection;
    }

    /// <summary>
    /// Permet de bouger le sceau selon les mouvements de la molette
    /// </summary>
    private void WheelRotation()
    {
        transform.Rotate(Vector3.up, Input.mouseScrollDelta.y * wheelSpeed);
    }
    #endregion Methods
}