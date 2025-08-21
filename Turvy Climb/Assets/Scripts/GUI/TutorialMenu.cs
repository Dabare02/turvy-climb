using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : PopupMenu
{
    [Header("Datos")]
    public TutorialMenuDataSO tutorialMenuData;

    [Header("Elementos UI")]
    [SerializeField] private TMP_Text titleTMP;
    [SerializeField] private Image previewImg;
    [SerializeField] private TMP_Text descriptionTMP;
    [SerializeField] private Button prevPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Toggle dontShowAgain;

    private int currentPage;

    void Awake()
    {
        if (titleTMP == null || previewImg == null || descriptionTMP == null
            || prevPageButton == null || nextPageButton == null)
        {
            Debug.LogWarning("Tutorial menu needs to have a reference to a" 
                + "title, description text, peview image and previous and next"
                + "page buttons.");
            GeneralManager.Instance.Quit();
        }

        if (tutorialMenuData == null || tutorialMenuData.pages == null
            || tutorialMenuData.pages.Length == 0)
        {
            Debug.LogWarning("Tutorial menu needs a TutorialMenuDataSO to display data.");
            GeneralManager.Instance.Quit();
        }
    }

    void OnEnable()
    {
        if (dontShowAgain != null) dontShowAgain.onValueChanged.AddListener(DontShowAgainToggle);

        SetPage(0);
    }

    void OnDisable()
    {
        if (dontShowAgain != null) dontShowAgain.onValueChanged.RemoveListener(DontShowAgainToggle);
    }

    // Cambia la página del menú tutorial por la siguiente página. Si es la última, vuelve a la primera página.
    public void GoToNextPage()
    {
        // Simple wrap-around counter.
        SetPage((currentPage + 1) % tutorialMenuData.pages.Length);
    }

    // Cambia la página del menú tutorial por al anterior página. Si es la primera, va a la última.
    public void GoToPrevPage()
    {
        // Simple inverse wrap-around counter.
        SetPage((currentPage - 1 + tutorialMenuData.pages.Length) % tutorialMenuData.pages.Length);
    }

    public void DontShowAgainToggle(bool cond) {
        EventManager.OnDSAValueChanged(cond);
    }

    private void SetPage(int pageNumber)
    {
        currentPage = pageNumber;

        // Activar o desactivar botones como corresponda.
        if (pageNumber == 0)
        {
            prevPageButton.interactable = false;
            nextPageButton.interactable = true;
        }
        else if (pageNumber < tutorialMenuData.pages.Length - 1)
        {
            prevPageButton.interactable = true;
            nextPageButton.interactable = true;
        }
        else if (pageNumber == tutorialMenuData.pages.Length - 1)
        {
            prevPageButton.interactable = true;
            nextPageButton.interactable = false;
        }
        else
        {
            Debug.LogWarning(pageNumber + " is not a valid index for a page of the tutorial menu " + this.name + ".");
            return;
        }

        // Cambiar todos los campos para reflejar el contenido de la página indicada.
        if (tutorialMenuData.pages[pageNumber].title != "")
        {
            titleTMP.text = tutorialMenuData.pages[pageNumber].title;
        }
        if (tutorialMenuData.pages[pageNumber].previewImage != null)
        {
            previewImg.sprite = tutorialMenuData.pages[pageNumber].previewImage;
        }
        if (tutorialMenuData.pages[pageNumber].description != "")
        {
            descriptionTMP.text = tutorialMenuData.pages[pageNumber].description;
        }

        Debug.Log("Switched tutorial menu to page " + pageNumber);
    }
}
