using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class OptionsScript : MonoBehaviour
{
    public Slider  SoundSlider, MusicSlider;
    public Text  SoundPercentage, MusicPercentage;
    public AudioSource Music, Sounds;
    public Toggle SoundToggle, MusicToggle;
    public Image SoundIsNotCheckedSprite , SoundIsCheckedSprite , MusicIsNotCheckedSprite , MusicIsCheckedSprite;
    float SoundVolume, MusicVolume;


    public void ChangeSoundVolume()
    {
        Sounds.volume = SoundSlider.value;
        SoundPercentage.text = Mathf.Round(SoundSlider.value * 100) + " %";
        //SoundVolume = Sounds.volume;

        if (SoundSlider.value == 0 )
        {
            SoundIsNotCheckedSprite.enabled = true;
            SoundIsCheckedSprite.enabled = false;
            SoundToggle.isOn = false;
            
        }
        else
        {
            SoundIsCheckedSprite.enabled = true;
            SoundIsNotCheckedSprite.enabled = false;
            SoundToggle.isOn = true;
        }
    }

    public void ChangeMusicVolume()
    {


        Music.volume = MusicSlider.value;
        MusicPercentage.text = Mathf.Round(MusicSlider.value * 100) + " %";
        //MusicVolume2 = Music.volume;
        if (MusicSlider.value == 0)
        {
            MusicIsNotCheckedSprite.enabled = true;
            MusicIsCheckedSprite.enabled = false;
            MusicToggle.isOn = false;

        }
        else
        {
            MusicIsCheckedSprite.enabled = true;
            MusicIsNotCheckedSprite.enabled = false;
            MusicToggle.isOn = true;
        }
    }


    public void SoundToggleImageChanger()
    {
        //if (SoundSlider.value == 0)
        //{
        //    SoundToggle.isOn = false;
        //}
        //else
        //{
        //    SoundToggle.isOn = true;
        //}

        if (SoundToggle.isOn == false)
        {
            SoundVolume = Sounds.volume;
            Sounds.volume = 0;
            SoundSlider.value = 0;
            SoundPercentage.text = "0%";
            SoundToggle.image = SoundIsNotCheckedSprite;
        }
        else
        {
            Sounds.volume = SoundVolume;
            if(SoundVolume == 0)
            {
                Sounds.volume = 0.25f;
            }
            SoundToggle.image = SoundIsCheckedSprite;
            SoundSlider.value =  Sounds.volume;
            SoundPercentage.text = Mathf.Round(SoundSlider.value * 100) + " %";
        }
    }



    public void MusicToggleImageChanger()
    {
        
        //if(MusicSlider.value == 0)
        //{
        //    MusicToggle.isOn = false;
        //}
        //else
        //{
        //    MusicToggle.isOn = true;
        //}






        if (MusicToggle.isOn == false)
        {
            MusicVolume = Music.volume; 
            Music.volume = 0; 
            MusicSlider.value = 0;
            MusicPercentage.text = "0%";
            MusicToggle.image = MusicIsNotCheckedSprite;
            
        }
        else 
        {

            Music.volume = MusicVolume;
            if(MusicVolume == 0)
            {
                Music.volume = 0.25f;
            }
            MusicSlider.value = Music.volume  ;
            MusicToggle.image = MusicIsCheckedSprite;
            MusicPercentage.text = Mathf.Round(MusicSlider.value * 100) + " %";
        }
    }



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Globals.optionsdata.SoundVolume = Sounds.volume;
        Globals.optionsdata.MusicVolume = Music.volume;
        //----Sensitivity
        // in codo edit kon va ba SensitivitySlider okayesh kon

        ////Globals.optionsdata.Sensitivity = SensitivitySlider.value;

        //---END  

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/optionsdata.sav");
        bf.Serialize(file, Globals.optionsdata);
        //Debug.Log(SoundVolume + " " + MusicVolume);
        file.Close();
    }
    private void Awake()
    {
        
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

            //----Sensitivity
            // in codo edit kon va ba SensitivitySlider okayesh kon
            Globals.optionsdata.Sensitivity = 0.5f;
            //---END  


            file.Close();

            if(Sounds.volume == 0)
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




}
