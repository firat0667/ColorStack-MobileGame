using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameData gameData;

    [HideInInspector]
    public int scoreCount;
    private string data_Path = "GameData.dat";

    private void Awake()
    {
        MakeSingleton();

        InitializeGameData();
    }

    void Start()
    {

        print(Application.persistentDataPath + data_Path);
        if (gameData != null)
        {
            print("data loaded");
        }




    }

    void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void InitializeGameData()
    {
        LoadGameData();

        if (gameData == null)
        {
            // we are running our game for the first time
            // set up initial values
            //			starScore = 0;

            // FOR TESTING ONLY REMOVE FOR PRODUCTION

            scoreCount = 0;

            

          
            gameData = new GameData();


            gameData.ScoreCount = scoreCount;
           





            SaveGameData();

        }

    }
    public void SaveGameData()
    {
        FileStream stream = null;

        try
        {

            BinaryFormatter bf = new BinaryFormatter();

            stream = File.Create(Application.persistentDataPath + data_Path);


            if (gameData != null)
            {

              
                gameData.ScoreCount = scoreCount;
                

                bf.Serialize(stream, gameData);


                print("kaydetti");
            }

        }
        catch (Exception e)
        {

        }
        finally
        {
            if (stream != null)
            {
                stream.Close();
            }
        }

    }
    public void LoadGameData()
    {

        FileStream file = null;


        try
        {

            BinaryFormatter bf = new BinaryFormatter();

            file = File.Open(Application.persistentDataPath + data_Path, FileMode.Open);

            gameData = (GameData)bf.Deserialize(file);


            if (gameData != null)
            {

               
                scoreCount= gameData.ScoreCount;
                
               

                print("geri yükle");

            }

        }
        catch (Exception e)
        {

        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }






}
