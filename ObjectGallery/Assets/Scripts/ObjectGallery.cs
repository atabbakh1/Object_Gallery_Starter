using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class ObjectGallery : MonoBehaviour
{

    public GalleryObject[] categoryAObjects;
    public GalleryObject[] categoryBObjects;
    public GalleryObject[] categoryCObjects;

    [Space(20)]
    [HideInInspector]
    public GameObject prefabeToPlace;
    public int maxPlacementDistance = 20;

    [Space(20)]
    public GameObject objectCategoryPanel;
    public GameObject objectGalleryPanel;
    public GameObject rightButton;
    public GameObject leftButton;
    public Image objectPreviewImage;
    public Text objectLabel;


    private GalleryObject[] selectedCategoryObjects;
    private int selectedCategoryCount;
    private int selectedObjectIndex = 0;
    private bool categoryPanelActive = false;
    private bool objectGalleryPanelActive = false;
    private bool placingPrompted = false;

    private void Start()
    {
        //turn both panels off on start
        objectCategoryPanel.SetActive(false);
        objectGalleryPanel.SetActive(false);

        UpdateNavigationButtons(selectedObjectIndex);
    }



    public void CategoryPanelSwitcher()
    {
        if(categoryPanelActive)
        {
            objectCategoryPanel.SetActive(false);
        }
        else
        {
            objectCategoryPanel.SetActive(true);
        }

        if (objectGalleryPanelActive)
        {
            objectGalleryPanel.SetActive(false);
        }

        categoryPanelActive = !categoryPanelActive;
        objectGalleryPanelActive = false;
    }

    public void BackButtonClicked()
    {
        objectCategoryPanel.SetActive(true);
        objectGalleryPanel.SetActive(false);

        objectGalleryPanelActive = false;

        selectedObjectIndex = 0;
        UpdateNavigationButtons(selectedObjectIndex);
    }

    public void CloseButtonClicked()
    {
        objectCategoryPanel.SetActive(false);
        objectGalleryPanel.SetActive(false);

        categoryPanelActive = false;
        objectGalleryPanelActive = false;

        selectedObjectIndex = 0;
        UpdateNavigationButtons(selectedObjectIndex);
    }


    public void SelectCategory(int categoryIndex)
    {
        switch (categoryIndex)
        {
            case 0:
                selectedCategoryObjects = categoryAObjects;
                break;
            case 1:
                selectedCategoryObjects = categoryBObjects;
                break;
            case 2:
                selectedCategoryObjects = categoryCObjects;
                break;
        }

        //turn off the category panel once selection is made
        objectCategoryPanel.SetActive(false);
        objectGalleryPanel.SetActive(true);
        objectGalleryPanelActive = true;

        selectedObjectIndex = 0;
        UpdateNavigationButtons(selectedObjectIndex);

        if (selectedCategoryObjects.Length > 0)
        {
            selectedCategoryCount = selectedCategoryObjects.Length;
            SetObjectDisplay(selectedObjectIndex);
        }

    }

    public void ResetObjectIndex()
    {
        selectedObjectIndex = 0;
        UpdateNavigationButtons(selectedObjectIndex);
    }


    public void RightButtonClicked()
    {
        selectedObjectIndex++;

        UpdateNavigationButtons(selectedObjectIndex);
        SetObjectDisplay(selectedObjectIndex);
    }

    public void LeftButtonClicked()
    {
        selectedObjectIndex--;

        UpdateNavigationButtons(selectedObjectIndex);
        SetObjectDisplay(selectedObjectIndex);
    }

    public void UpdateNavigationButtons(int selectedIndex)
    {
        if(selectedObjectIndex == 0)
        {
            leftButton.SetActive(false);
            rightButton.SetActive(true);
        }
        else if(selectedObjectIndex > 0 && selectedObjectIndex < selectedCategoryCount-1)
        {
            leftButton.SetActive(true);
            rightButton.SetActive(true);
        }
        else
        {
            leftButton.SetActive(true);
            rightButton.SetActive(false);
        }

    }


    public void SetObjectDisplay(int index)
    {
        Sprite thumb = selectedCategoryObjects[index].itemThumb;
        string label = selectedCategoryObjects[index].itemName;

        if (thumb != null)
        {
            objectPreviewImage.sprite = thumb;
            if(!string.IsNullOrEmpty(label))
            {
                objectLabel.text = label;
            }
        }
    }



    public void PlaceButtonClicked()
    {
        placingPrompted = true;

        prefabeToPlace = selectedCategoryObjects[selectedObjectIndex].itemPrefab;

        objectCategoryPanel.SetActive(false);
        objectGalleryPanel.SetActive(false);
    }

    private void Update()
    {
        if(placingPrompted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //On left click spawn selected prefab and align its rotation to a surface normal
                Vector3[] spawnData = GetClickPositionAndNormal();
                if (spawnData[0] != Vector3.zero)
                {
                    GameObject go = Instantiate(prefabeToPlace, spawnData[0], Quaternion.FromToRotation(prefabeToPlace.transform.up, spawnData[1]));
                    go.name += " _instantiated";

                    objectGalleryPanelActive = false;
                    categoryPanelActive = false;
                    placingPrompted = false;
                }          
            }
        }
    }

    Vector3[] GetClickPositionAndNormal()
    {
        Vector3[] positions = new Vector3[] { Vector3.zero, Vector3.zero }; //0 = spawn poisiton, 1 = surface normal
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, maxPlacementDistance))
        {
            positions[0] = hit.point;
            positions[1] = hit.normal;
        }
        return positions;
    }
}


[Serializable]
public class GalleryObject
{
    [Tooltip("The prefab with the gameobject for this item")]
    public GameObject itemPrefab;
    [Tooltip("the image to be displayed as a thumbnail for this item")]
    public Sprite itemThumb;
    [Tooltip("The name of the item")]
    public string itemName;
}
