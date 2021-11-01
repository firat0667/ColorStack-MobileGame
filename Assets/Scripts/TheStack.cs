using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using GoogleMobileAds.Api;
using System;


public class TheStack : MonoBehaviour
{
    public Color32[] gameColors = new Color32[14];
    public Color[] textcolor;
    
    
    
    
    public Material stackMat;
    private const float Bounds_Size = 3.5f;
    private const float Stack_Moving_Speed = 3.5f;
    private const float Error_Margin = 0.1f;
    private const float Stack_Bounds_Gain= 0.25f;
    private const float Combo_Start_Gain = 3f;


    private GameObject[] theStack;
    private Vector2 stackBounds = new Vector2(Bounds_Size,Bounds_Size);
    private int stackIndex;
    
    private int scoreCount = 0;
    public int scoreColor = 0;
    public int scoreTextcolor = 0;
    public int coloreksi1;
    private int combo = 0;
    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;
    private bool isMovingX = true;
    public bool gameOver = false;
    

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;
    
    public Text scoretext,exitText;
    private Button PauseButton;
    
    
   
    public Image image,image2,image3;
    private ParticleSystem ps;
    public AudioSource audioSource;
    public Text bestScore;
  
    











    void Start()
    {
        
       
        
        
        bestScore.text = GameManager.instance.scoreCount.ToString();
        audioSource = GetComponent<AudioSource>();


        PauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
     
      
         



        scoretext.text = scoreCount.ToString();

        theStack = new GameObject[transform.childCount];
        
        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
        }
        stackIndex = transform.childCount - 1;
        
    }
    private void FixedUpdate()
    {
        
        ps = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
        var col = ps.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(textcolor[coloreksi1], 0.0f), new GradientColorKey(textcolor[scoreTextcolor], 0.8f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.0f) });

        col.color = grad;
        
    }

    // Update is called once per frame
    void Update()
    {
        

        coloreksi1 = scoreTextcolor - 1;
        if (scoreTextcolor == 0)
        {
            coloreksi1 = 13;
        }

        if (Time.timeScale == 1)
        {
            if (gameOver)

                return;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began|| Input.GetMouseButtonDown(0))
            {
                print("dokundu");
                if (PlaceTile())
                {
                    SpawnTile();
                    scoreCount++;

                    if (scoreTextcolor == 14)
                    {
                        scoreTextcolor = 0;
                    }

                    scoretext.text = scoreCount.ToString();
                    scoretext.color = textcolor[scoreTextcolor];
                    bestScore.color = textcolor[scoreTextcolor];
                    image.color = textcolor[scoreTextcolor];
                    image2.color = textcolor[scoreTextcolor];
                    image3.color = textcolor[scoreTextcolor];

                    exitText.color = textcolor[scoreTextcolor];
                    
                    ps.Play();
                    audioSource.Play();
                    if (GameManager.instance.scoreCount < scoreCount)
                    {
                        GameManager.instance.scoreCount = scoreCount;
                    }











                    // scoreColor=scoreCount/2;




                }
                else
                {

                    SceneManager.LoadScene(0);
                    
                    


                    //  UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
            }


        }



        MoveTile();
        //move the stuck
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Stack_Moving_Speed * Time.deltaTime);

  
         

        
    }
    void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] color = new Color32[vertices.Length];

        float f = Mathf.Sin(scoreCount * 0.25f);
        for(int i = 0; i < vertices.Length; i++)
        {
            if (scoreColor ==14)
                {
                scoreColor= 0;
                }
            color[i] = Lerp14(gameColors[scoreColor], gameColors[scoreColor], gameColors[scoreColor], gameColors[scoreColor],f);
                
        }
        mesh.colors32 = color;
    }
    Color32 Lerp14(Color32 a, Color32 b, Color32 c, Color32 d, float t)
        
      
    {

      
      

        if (t < 0.33f)
        {

            return Color.Lerp(a, b, t / 0.33f);

        }
        else if (t < 0.66f)
        {

            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);

        }
        else
        {

            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);

        }



    }
   
    void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;

        if (isMovingX)
        {
            theStack[stackIndex].transform.localPosition =
                new Vector3(Mathf.Sin(tileTransition) * Bounds_Size,scoreCount,secondaryPosition);
        }
        else
        {
            theStack[stackIndex].transform.localPosition =
                new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * Bounds_Size);
        }
    }
    bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;
        if (isMovingX)
        {
            float deltaX = lastTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > Error_Margin)
            {
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                {
                    return false;
                }
                float middle = lastTilePosition.x + t.localPosition.x / 2f;
                t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);
                //create the part  that is going to fall down
                CreateRubble(new Vector3((t.position.x > 0) ?
                    t.position.x + (t.localScale.x / 2f) : t.position.x - (t.localScale.x / 2f), t.position.y, t.position.z),
                    new Vector3(Mathf.Abs(deltaX),1f,t.localScale.z)

                    );
                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2f),scoreCount,lastTilePosition.z);


            }
            else
            {
                if (combo > Combo_Start_Gain)
                {
                    stackBounds.x += Stack_Bounds_Gain;
                    if (stackBounds.x > Bounds_Size)
                        stackBounds.x = Bounds_Size;
                    float middle = lastTilePosition.x + t.localPosition.x / 2f;
                    t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);
                    t.localPosition = new Vector3(middle - (lastTilePosition.x / 2f), scoreCount, lastTilePosition.z);

                    
                }
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x,scoreCount,lastTilePosition.z);

            }
           
           
            
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > Error_Margin)
            {
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTilePosition.z + t.localPosition.z / 2f;
                t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);

                CreateRubble(new Vector3(t.position.x, t.position.y, (t.position.z > 0) ? t.position.z + (t.localScale.z / 2f) :
                    t.position.z - (t.localScale.z / 2f)),
                    new Vector3(t.localScale.x, 1f, Mathf.Abs(deltaZ)));
                t.localPosition = new Vector3(lastTilePosition.x / 2f, scoreCount, middle - (lastTilePosition.z / 2f));
                scoreColor++;
                scoreTextcolor++;



            }
            else
            {
                if (combo > Combo_Start_Gain)
                {
                    stackBounds.y += Combo_Start_Gain;
                    if (stackBounds.y > Bounds_Size)
                        stackBounds.y = Bounds_Size;
                    float middle = lastTilePosition.z + t.localPosition.z / 2f;
                    t.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);
                    t.localPosition=new Vector3(lastTilePosition.x/2f,scoreCount,middle-( lastTilePosition.z/2f));
                }
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x,scoreCount,lastTilePosition.z);
            }
                
        }
        secondaryPosition = (isMovingX) ? t.localPosition.x : t.localPosition.z;
        isMovingX = !isMovingX;

        return true;

    }
    void CreateRubble(Vector3 pos,Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();

        go.GetComponent<MeshRenderer>().material = stackMat;
        ColorMesh(go.GetComponent<MeshFilter>().mesh);
    }// create rubble
    void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
        
            stackIndex = transform.childCount - 1;
         desiredPosition = Vector3.down * scoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(0f, scoreCount, 0f);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1f, stackBounds.y);
        ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);

        
        
    }
   public void pauseButton()
    {
        Time.timeScale = 0;
        
       
    }
    public void ReturnButton()
    {
        Time.timeScale = 1;
    }
    public void QuitGame()
    {
        print("application quit");
        Application.Quit();
    }
    public void replayGame()
    {
        SceneManager.LoadScene(0);
    }
 
   

  
   
   



}
