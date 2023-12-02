using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    public GameObject[] pages;
    public Sprite[] sprites;

    public int currentPage = 0;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }
    public void nextPage(bool next)
    {
        if (next)
        {
            if (currentPage != pages.Length - 1)
            {
                pages[currentPage].SetActive(false);
                currentPage++;
                pages[currentPage].SetActive(true);
                CheckPage();
            }
        }
        else if (currentPage != 0)
        {
            pages[currentPage].SetActive(false);
            currentPage--;
            pages[currentPage].SetActive(true);
            CheckPage();
        }
    }
    public void SetPage(int page)
    {
        pages[currentPage].SetActive(false);
        currentPage = page;
        pages[currentPage].SetActive(true);
        CheckPage();
    }
    private void CheckPage()
    {
        if(currentPage == 0) image.sprite = sprites[0];
        else image.sprite = sprites[1];
    }
}
