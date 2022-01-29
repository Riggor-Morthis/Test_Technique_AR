using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovementScript : MonoBehaviour
{
    #region Fields
    //Privates
    [SerializeField, Range(1f, 100f)] private float wheelSpeed = 10f; //Vitesse de rotation de la molette

    private bool isSelected = false; //Est-ce qu'on est actuellement en train de bouger
    private Vector3 previousMouseProjection, currentMouseProjection; //La position de notre souris, projettee sur le plat plan
    private Camera mainCamera; //Notre camera
    private Plane flatPlane = new Plane(Vector3.up, Vector3.zero); //Le plan plat, normale verticale et passant par le "pied" de l'objet
    private float currentDistance; //Distance sur le rayon pour le plat
    private Ray currentRay; //Un rayon
    private Vector3 currentMovementVector; //Le mouvement de notre souris
    private bool verticalTranslation = false; //Est-ce qu'on est en train de faire un mouvement vertical
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
            //On redefinit notre plan de projection au passage
            flatPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
            //On recupere un point de reference
            previousMouseProjection = GetMouseProjection();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) isSelected = false;
        //On agit que si on est selectionne
        else if (isSelected)
        {
            //Est-ce qu'on passe en mode translation verticale ?
            if (Input.GetMouseButtonDown(1))
            {
                verticalTranslation = true;
                previousMouseProjection = GetVerticalMouse();
            }
            //Est-ce qu'on passe en mode translation horizontale ?
            else if (Input.GetMouseButtonUp(1) && verticalTranslation)
            {
                verticalTranslation = false;
                flatPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
                previousMouseProjection = GetMouseProjection();
            }

            //On s'assure qu'on fait bien la bonne translation
            if (!verticalTranslation) FollowTheMouse();
            else FollowTheVerticalMouse();

            //Quoi qu'il arrive on peut faire tourner le sceau
            WheelRotation();
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