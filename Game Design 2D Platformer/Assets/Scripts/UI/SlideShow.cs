using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlideShow : MonoBehaviour
{
    Image image;
    GameObject slidelist;
    int currentSlide = 0;
    [SerializeField] private string whichScene;
    void Start()
    {
        image = GameObject.Find("Image").GetComponent<Image>();
        slidelist = GameObject.Find("SlideList");
        image.sprite = slidelist.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextSlide(){
        if(currentSlide == slidelist.transform.childCount-1){
            SceneManager.LoadScene(whichScene);
        }
        currentSlide++;
        image.sprite = slidelist.transform.GetChild(currentSlide).GetComponent<SpriteRenderer>().sprite;
    }
}
