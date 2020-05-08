using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenuScript : MonoBehaviour
{
    public Toggle SoundToggle, MusicToggle;
    public Image SoundIsNotCheckedSprite, SoundIsCheckedSprite, MusicIsNotCheckedSprite, MusicIsCheckedSprite;

    float SoundVolume, MusicVolume;
    public Slider slider, SoundSlider, MusicSlider , SensitivitySlider;
    public Text txtPercentage, SoundPercentage, MusicPercentage , SensitivityPercentage;
    public AudioSource Music, Sounds;
    string btnName;
    public Animator Mainanim, ExitDialogCanvas, Canvas, Camera, Store;
    public Canvas MainCanvas;

    public Camera camera1;

    public void LoadLevel(string sceneName)
    {


        StartCoroutine(LoadAnimForSinglePlayer(sceneName));

    }


    IEnumerator LoadAnimForSinglePlayer(string sceneName)
    {


        Mainanim.SetInteger("ChangeAnim", 5);
        Store.SetInteger("ChangeAnim", 2);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(LoadLevelAsynchronous(sceneName));
    }


    IEnumerator LoadLevelAsynchronous(string sceneName)
    {


        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
        {
            double progress = System.Math.Round(op.progress, 2);            
            slider.value = float.Parse(progress.ToString());
            txtPercentage.text = "Loading " + slider.value * 100 + " % ";
            yield return null;
        }
    }


    // Use this for initialization
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        camera1.enabled = false;

        if (GameObject.Find("LocalNetworkManager"))
        {
            Destroy(GameObject.Find("LocalNetworkManager"));
        }
    }

    private void Awake()
    {

        //Mainanim = GameObject.Find("MainCanvas").GetComponent<Animator>();
        //ExitDialogCanvas = GameObject.Find("ExitDialogCanvas").GetComponent<Animator>();
        //Mainanim.SetInteger("ChangeAnim", 1);

        //Canvas = GameObject.Find("Canvas").GetComponent<Animator>();
        //Camera = GameObject.Find("Camera").GetComponent<Animator>();
        //Store= GameObject.Find("Store").GetComponent<Animator>();
        //MainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
    }











    public void CheckForExit()
    {



        Mainanim.SetInteger("ChangeAnim", 3);
        ExitDialogCanvas.SetInteger("ChangeAnim", 3);

    }


    // this happens when u press No and cancel the Exit Process
    public void CancelQuit()
    {
        StartCoroutine(WasteOfTime());

    }

    // this is when u hit Yes  :D
    public void Quit()
    {
        Application.Quit();
    }


    public void btnStore()
    {

        Camera.SetInteger("ChangeAnim", 1);
        MainCanvas.enabled = false;
        camera1.enabled = true;


        Store.SetInteger("ChangeAnim", 1);

    }


    int a = 0;


    public void onPointerDownButton()
    {

        btnName = EventSystem.current.currentSelectedGameObject.name;




        if (btnName == "btnQuit")
        {
            Mainanim.SetInteger("ChangeAnim", 7);


        }
        else if (btnName == "btnPlay")
        {
            Mainanim.SetInteger("ChangeAnim", 9);

        }
        else if (btnName == "btnStore")
        {
            Mainanim.SetInteger("ChangeAnim", 11);

        }
        else if (btnName == "btnAbout")
        {
            Mainanim.SetInteger("ChangeAnim", 13);

        }
        else if (btnName == "btnSettings")
        {
            Mainanim.SetInteger("ChangeAnim", 15);

        }
        else if (btnName == "btnCancel")
        {
            Canvas.SetInteger("ChangeAnim", 1);
        }
        else if (btnName == "btnNo")
        {
            Canvas.SetInteger("ChangeAnim", 3);

        }
        else if (btnName == "btnYes")
        {
            Canvas.SetInteger("ChangeAnim", 5);

        }
        else if (btnName == "btnSinglePlayer")
        {
            Canvas.SetInteger("ChangeAnim", 7);

        }
        else if (btnName == "btnOnline")
        {
            Canvas.SetInteger("ChangeAnim", 9);

        }
        else if (btnName == "btnLocal")
        {
            Canvas.SetInteger("ChangeAnim", 11);
        }
        Debug.Log(btnName);
    }
    public void onPointerUpButton()
    {


        Sounds.Play();
        if (btnName == "btnQuit")
        {
            Mainanim.SetInteger("ChangeAnim", 8);

        }
        else if (btnName == "btnPlay")
        {
            Mainanim.SetInteger("ChangeAnim", 10);
        }
        else if (btnName == "btnStore")
        {

            Mainanim.SetInteger("ChangeAnim", 12);
        }
        else if (btnName == "btnAbout")
        {

            Mainanim.SetInteger("ChangeAnim", 14);
        }
        else if (btnName == "btnSettings")
        {

            Mainanim.SetInteger("ChangeAnim", 16);
        }
        else if (btnName == "btnCancel")
        {
            //ExitDialogCanvas.SetInteger("ChangeAnim", 18);

            Canvas.SetInteger("ChangeAnim", 2);




        }
        else if (btnName == "btnNo")
        {
            Canvas.SetInteger("ChangeAnim", 4);

        }
        else if (btnName == "btnYes")
        {
            Canvas.SetInteger("ChangeAnim", 6);

        }
        else if (btnName == "btnSinglePlayer")
        {
            Canvas.SetInteger("ChangeAnim", 8);

        }
        else if (btnName == "btnOnline")
        {
            Canvas.SetInteger("ChangeAnim", 10);

        }
        else if (btnName == "btnLocal")
        {
            Canvas.SetInteger("ChangeAnim", 12);

        }

        btnName = "Null2";
    }




    IEnumerator WasteOfTime()
    {
        yield return new WaitForSeconds(0.1f);
        ExitDialogCanvas.SetInteger("ChangeAnim", 0);
        Mainanim.SetInteger("ChangeAnim", 4);



    }









    // this happens when u press the facin play btn man Runs the facin "RunAnim()" function this name is fucked up cuz u fucked it up manam hal ndrm avaz konm xD
    public void AimTest()

    {

        ExitDialogCanvas.SetInteger("ChangeAnim", 0);
        Mainanim.SetInteger("ChangeAnim", 3);
        ExitDialogCanvas.SetInteger("ChangeAnim", 1);








        // Mainanim.SetInteger("ChangeAnim", 6);


    }

    public void RunSinglePlayerMode()
    {
        StartCoroutine(RunAnim());
    }


    IEnumerator RunAnim()
    {

        Mainanim.SetInteger("ChangeAnim", 2);
        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadScene("TrainingArena");


    }


    //this opens options dialog


    public void OpenOptionsDialog()
    {
        ExitDialogCanvas.SetInteger("ChangeAnim", 0);

        Mainanim.SetInteger("ChangeAnim", 3);
        ExitDialogCanvas.SetInteger("ChangeAnim", 2);

        if (File.Exists(Application.persistentDataPath + "/optionsdata.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/optionsdata.sav", FileMode.Open);
            Globals.optionsdata = (Optionsdata)bf.Deserialize(file);
            Music.volume = Globals.optionsdata.MusicVolume;
            MusicPercentage.text = Mathf.Round(MusicSlider.value * 100) + " %";
            MusicVolume = Music.volume;
            MusicSlider.value = Music.volume;

            Sounds.volume = Globals.optionsdata.SoundVolume;
            SoundVolume = Sounds.volume;
            SoundPercentage.text = Mathf.Round(SoundSlider.value * 100) + " %";
            SoundSlider.value = Sounds.volume;

            SensitivitySlider.value = Globals.optionsdata.Sensitivity;
            SensitivityPercentage.text = Globals.optionsdata.Sensitivity + "";

            file.Close();

            if (Sounds.volume == 0)
            {
                SoundIsNotCheckedSprite.enabled = true;
                SoundIsCheckedSprite.enabled = false;

            }
            else
            {
                SoundIsCheckedSprite.enabled = true;
                SoundIsNotCheckedSprite.enabled = false;
            }

            if (Music.volume == 0)
            {
                MusicIsNotCheckedSprite.enabled = true;
                MusicIsCheckedSprite.enabled = false;

            }
            else
            {
                MusicIsCheckedSprite.enabled = true;
                MusicIsNotCheckedSprite.enabled = false;
            }
        }


        

    }

    //this Opens About Us Dialog

    public void OpenAboutDialog()
    {
        ExitDialogCanvas.SetInteger("ChangeAnim", 0);
        Mainanim.SetInteger("ChangeAnim", 3);
        ExitDialogCanvas.SetInteger("ChangeAnim", 2);
    }



    // Update is called once per frame

    void Update()
    {

        //Debug.Log(btnName);
        //if (isRacePressed)
        //{

        //        Mainanim.SetInteger("ChangeAnim", 7);

        //}

        //else if (!isRacePressed)
        //{
        //    Mainanim.SetInteger("ChangeAnim", 0);

        //}
    }



    public void BlackCanvas()
    {
        ExitDialogCanvas.SetInteger("ChangeAnim", 0);
        Mainanim.SetInteger("ChangeAnim", 4);

    }







}
