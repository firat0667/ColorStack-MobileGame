using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuControl : MonoBehaviour
{
    public Color[] textcolor;
    public int scoreTextcolor = 0;
    public Image image;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreTextcolor == 14)
        {
            scoreTextcolor = 0;
        }
        image.color = textcolor[scoreTextcolor];
    }
    public  void Playerbutton()
    {
        SceneManager.LoadScene(1);
    }
    public void ColorChange()
    {
        scoreTextcolor++;
    }
}
