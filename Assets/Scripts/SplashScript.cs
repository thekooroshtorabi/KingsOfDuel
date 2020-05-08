using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour {


   public Animator anim;
    public Slider slider;
    public Text txtPercentage;
    public void LoadScene()
    {


        StartCoroutine(LoadAnimFormainenu("MainMenu"));

    }


    IEnumerator LoadAnimFormainenu(string sceneName)
    {


        
        yield return new WaitForSeconds(2.0f);
        anim.SetBool("RunAnim", true);
        StartCoroutine(LoadLevelAsynchronous(sceneName));
    }


    IEnumerator LoadLevelAsynchronous(string sceneName = "MainMenu")
    {


        AsyncOperation op = SceneManager.LoadSceneAsync("MainMenu");
        while (!op.isDone)
        {
            double progress = System.Math.Round(op.progress, 2);
            slider.value = float.Parse(progress.ToString());
            txtPercentage.text = "Loading " + slider.value * 100 + " % ";
            yield return null;
        }
    }







    // Use this for initialization
    IEnumerator Start()
    {
        anim = GameObject.Find("Slider").GetComponent<Animator>();
        yield return new WaitForSeconds(1.95f);
        anim.SetBool("RunAnim", true);
        LoadScene();
        //SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
