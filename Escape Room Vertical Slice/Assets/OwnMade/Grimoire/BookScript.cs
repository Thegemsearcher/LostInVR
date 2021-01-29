using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookScript : MonoBehaviour
{
    public GameObject Page1;
    public GameObject Page2;
    public List<Material> SpellImages;
    public Material Empty;

    PageScript page1;
    PageScript page2;
    MeshRenderer spellImage1;
    MeshRenderer spellImage2;
    List<Tuple<Material, Material>> Pages;

    int currentPage;

    void Start()
    {
        Pages = new List<Tuple<Material, Material>>();
        for (int i = 0; i < SpellImages.Count; i+=2)
        {
            if ((i + 1) < SpellImages.Count)
            {
                Pages.Add(new Tuple<Material, Material>(SpellImages[i], SpellImages[i + 1]));
            }
            else
            {
                Pages.Add(new Tuple<Material, Material>(SpellImages[i], Empty));
            }
        }

        page1 = Page1.GetComponent<PageScript>();
        page2 = Page2.GetComponent<PageScript>();

        spellImage1 = page1.SpellImage.GetComponent<MeshRenderer>();
        spellImage2 = page2.SpellImage.GetComponent<MeshRenderer>();

        if (Pages.Count > 0)
        {
            spellImage1.material = Pages[0].Item1;
            spellImage2.material = Pages[0].Item2;
        }

        currentPage = 0;
    }
    
    public void FlipForward()
    {
        if((currentPage + 1) < Pages.Count)
        {
            currentPage++;
            spellImage1.material = Pages[currentPage].Item1;
            spellImage2.material = Pages[currentPage].Item2;
        }
    }

    public void FlipBackward()
    {
        if(currentPage > 0)
        {
            currentPage--;
            spellImage1.material = Pages[currentPage].Item1;
            spellImage2.material = Pages[currentPage].Item2;
        }
    }
}
