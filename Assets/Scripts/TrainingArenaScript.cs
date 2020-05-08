using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainingArenaScript : MonoBehaviour
{

    private float time = 5f, shot_time = 0f, ShotAngle = 0f, BulletRotation = 0f, Distance, BulletArrivalTime, PassedTimeAfterShoot, ZoomTime = 0;
    private int Headshots = 0, PunchCounter = 1, t = 0;
    private bool ableToShoot = false, didHit, didHitGround , didPlayerHeadshot = false;
    private string TrainingMode = "AimOnly";
    private Animator CanvasAnimator, AimLineAnim, CharacterAnim;
    private Rigidbody2D CharacterPhysics, BulletFollowCamPhysics;
    private SpriteRenderer fire_sign, hold_sign;
    private Renderer[] listOfChildren;
    private Renderer parent;
    private Text txtCooldown, txtMode, total, txtCooldownS;
    private Button btnShoot;
    private Transform PlayerGun, BulletFollowCamTransform, PlayerBulletTransform, MainCameraTransform;
    private RectTransform AimLine;
    private Vector3 HitPosition;
    private Collider2D AimLineCollider;
    private Camera MainCamera, BulletFollowCam;


    void Awake()
    {
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        BulletFollowCam = GameObject.Find("BulletFollowCamera").GetComponent<Camera>();
        BulletFollowCamTransform = GameObject.Find("BulletFollowCamera").GetComponent<Transform>();
        MainCameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        total = GameObject.Find("Total").GetComponent<Text>();
        txtMode = GameObject.Find("TrainingMode").GetComponent<Text>();
        txtCooldown = GameObject.Find("CooldownTimer").GetComponent<Text>();
        txtCooldownS = GameObject.Find("CooldownTimerSecond").GetComponent<Text>();
        time = Random.Range(4f, 6f);
        btnShoot = GameObject.Find("btnShoot").GetComponent<Button>();
        CharacterAnim = GameObject.Find("Character").GetComponent<Animator>();
        AimLineAnim = GameObject.Find("AimLineController").GetComponent<Animator>();
        AimLine = GameObject.Find("AimLineController").GetComponent<RectTransform>();
        AimLineCollider = GameObject.Find("AimLineController").GetComponent<Collider2D>();
        CanvasAnimator = GameObject.Find("Canvas").GetComponent<Animator>();
        CharacterPhysics = GameObject.Find("CharacterController").GetComponent<Rigidbody2D>();
        BulletFollowCamPhysics = GameObject.Find("BulletFollowCamera").GetComponent<Rigidbody2D>();
        PlayerGun = GameObject.Find("SideGun").GetComponent<Transform>();
        CharacterPhysics.velocity = new Vector2(-1f, 0f);
        hold_sign = GameObject.Find("Hold").GetComponent<SpriteRenderer>();
        fire_sign = GameObject.Find("Fire").GetComponent<SpriteRenderer>();
        AimLineRotator.txtAngle = GameObject.Find("txtAngle").GetComponent<Text>();

        //Training
        ShowFireSign();

        parent = GameObject.Find("NoShoot").GetComponent<Renderer>();
        listOfChildren = GameObject.Find("NoShoot").GetComponentsInChildren<Renderer>();
        parent.enabled = false;
        foreach (Renderer child in listOfChildren)
        {
            child.enabled = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        BulletFollowCamTransform.position = MainCameraTransform.position;
        CharacterAnim.SetBool("Walking",true);
        StartCoroutine(StopWalking());
    }

    // Update is called once per frame
    void Update()
    {
        if (TrainingMode == "AimAndReaction")
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                if (hold_sign.sortingOrder == 1)
                {
                    GameObject.Find("StartSound").GetComponent<AudioSource>().Play();
                }
                ShowFireSign();
                shot_time += Time.deltaTime;
            }
        }
    }

    IEnumerator ZoomOnTarget()
    {
        if (ZoomTime == 0)
        {
            // Zoom Delay
            yield return new WaitForSeconds(BulletArrivalTime / 2);

            // Change Cam            
            Time.timeScale = 0.05f;
            MainCamera.enabled = !MainCamera.enabled;
            BulletFollowCam.enabled = !BulletFollowCam.enabled;

        }
        if (ZoomTime < 60)
        {
            BulletFollowCamTransform.position += transform.forward * 1.4f;
            BulletFollowCamTransform.position += transform.right * -0.13f;
            BulletFollowCamTransform.position += transform.up * -0.02f;
            ZoomTime++;
            yield return new WaitForSeconds(0.001f);
            StartCoroutine(ZoomOnTarget());
        }
        else
        {
            ZoomTime = 0;
            StartCoroutine(CalculateResetTime());
        }

    }

    IEnumerator StartTimePassed()
    {
        PassedTimeAfterShoot += 0.001f;
        yield return new WaitForSeconds(0.001f);
        StartCoroutine(StartTimePassed());
    }

    IEnumerator CalculateResetTime()
    {
        if (PassedTimeAfterShoot < BulletArrivalTime - 0.06f)
        {
            yield return new WaitForSeconds(0.001f);
            StartCoroutine(CalculateResetTime());
        }
        else
        {
            StartCoroutine(ResetCamerasToDefault());
        }
    }

    IEnumerator ResetCamerasToDefault()
    {
        yield return new WaitForSeconds(0f);
        PassedTimeAfterShoot = 0;
        Time.timeScale = 1;
        MainCamera.enabled = !MainCamera.enabled;
        BulletFollowCam.enabled = !BulletFollowCam.enabled;
        BulletFollowCamTransform.position = MainCameraTransform.position;
        didPlayerHeadshot = false;
    }

    public void SwitchTrainingMode()
    {
        GameObject.Find("BtnSign").GetComponent<AudioSource>().Play();
        if (TrainingMode == "AimOnly")
        {
            TrainingMode = "AimAndReaction";
            txtMode.text = "Aim And Reaction";
            if (GameObject.Find("BestReaction").GetComponent<Text>().text.Trim() == "")
            {
                GameObject.Find("BestReaction").GetComponent<Text>().text = "Best Reaction : ";
                GameObject.Find("txtBestReaction").GetComponent<Text>().text = "0s";
                GameObject.Find("txtTip").GetComponent<Text>().text = "";
            }
            ResetGame();
        }
        else
        {
            TrainingMode = "AimOnly";
            txtMode.text = "Aim Only";
            ShowFireSign();
        }
    }

    public void ShowFireSign()
    {
        fire_sign.sortingOrder = 1;
        hold_sign.sortingOrder = 0;
    }

    public void ShowHoldSign()
    {
        fire_sign.sortingOrder = 0;
        hold_sign.sortingOrder = 1;
    }

    IEnumerator StopWalking()
    {
        yield return new WaitForSeconds(2.5f);
        CharacterAnim.SetBool("Walking", false);
        AimLineAnim.SetBool("AimLine", true);
        yield return new WaitForSeconds(0.5f);
        CharacterPhysics.velocity = new Vector2(0f, 0f);
        CharacterAnim.SetBool("Idle", true);
        yield return new WaitForSeconds(0.5f);
        ableToShoot = true;
        btnShoot.interactable = true;
    }

    IEnumerator ShootAnimation()
    {
        CharacterAnim.SetBool("Idle", false);
        if (CharacterAnim.GetInteger("Angle") >= -30 && CharacterAnim.GetInteger("Angle") <= 30)
        {
            CharacterAnim.Play("CharacterShoot");
        }
        else if (CharacterAnim.GetInteger("Angle") >= 30 && CharacterAnim.GetInteger("Angle") <= 130)
        {
            CharacterAnim.Play("CharacterShooting45_90");
        }
        else if (CharacterAnim.GetInteger("Angle") >= -130 && CharacterAnim.GetInteger("Angle") <= -30)
        {
            CharacterAnim.Play("CharacterShootingN45_90");
        }
        else
        {
            CharacterAnim.Play("CharacterShooting45_90");
        }
        yield return new WaitForSeconds(0.27f);
        GameObject.Find("SideGun").GetComponent<AudioSource>().Play();
        CharacterAnim.enabled = false;
        if (CharacterAnim.GetInteger("Angle") >= -30 && CharacterAnim.GetInteger("Angle") <= 30)
        {
            PlayerGun.rotation = Quaternion.Euler(0, 0, (AimLine.eulerAngles.z));
        }
        else if (CharacterAnim.GetInteger("Angle") >= 30 && CharacterAnim.GetInteger("Angle") <= 130)
        {
            PlayerGun.rotation = Quaternion.Euler(0, 0, (AimLine.eulerAngles.z));
        }
        else if (CharacterAnim.GetInteger("Angle") >= -130 && CharacterAnim.GetInteger("Angle") <= -30)
        {
            PlayerGun.rotation = Quaternion.Euler(0, 0, (AimLine.eulerAngles.z + 12.155f));
        }
        yield return new WaitForSeconds(0.07f);
        if (didHit == true)
        {
            BulletArrivalTime = Distance / BulletScript.velocity;
            StartCoroutine(HitEffects(BulletArrivalTime, HitPosition));
        }
        if (didHitGround == true)
        {
            BulletArrivalTime = Distance / BulletScript.velocity;
            StartCoroutine(HitEffectsOnGround(BulletArrivalTime, HitPosition));
        }
        GenerateBullet();
        GameObject.Find("GunShotFire").GetComponent<ParticleSystem>().Play();
        //GameObject.Find("GunShotSmoke").GetComponent<ParticleSystem>().Play();

        StartCoroutine(GunPunch());
    }

    IEnumerator GunPunch()
    {
        int interval = 5;
        int speed = 10;
        if (PunchCounter < interval && PunchCounter > 0)
        {
            PunchCounter++;
            PlayerGun.Rotate(Vector3.forward * speed);
        }
        if (PunchCounter == interval)
        {
            PunchCounter = (-interval) + 1;
        }
        if (PunchCounter > -interval && PunchCounter < 0)
        {
            PunchCounter++;
            PlayerGun.Rotate(Vector3.forward * -speed);
        }
        if (PunchCounter != 0)
        {
            yield return new WaitForSeconds(0.001f);
            StartCoroutine(GunPunch());
        }
        else
        {
            PunchCounter = 1;
            yield return new WaitForSeconds(1f);
            StartCoroutine(RotateGunToDefault());
        }
    }

    IEnumerator RotateGunToDefault()
    {

        if ((PlayerGun.eulerAngles.z >= 332 && PlayerGun.eulerAngles.z <= 335) || (PlayerGun.eulerAngles.z >= 25 && PlayerGun.eulerAngles.z <= 28) || (PlayerGun.eulerAngles.z >= 358 && PlayerGun.eulerAngles.z <= 361))
        {
            StartCoroutine(Idle());
        }
        else
        {
            int speed = 1;
            if (ShotAngle >= -30 && ShotAngle <= 30)
            {
                if (PlayerGun.eulerAngles.z < 333 && PlayerGun.eulerAngles.z > 330)
                {
                    PlayerGun.Rotate(Vector3.forward * -speed);
                }
                else if (PlayerGun.eulerAngles.z < 359)
                {
                    PlayerGun.Rotate(Vector3.forward * speed);
                }
                else if (PlayerGun.eulerAngles.z > 360)
                {
                    PlayerGun.Rotate(Vector3.forward * speed);
                }
            }
            else if (ShotAngle >= 30 && ShotAngle <= 130)
            {

                if (PlayerGun.eulerAngles.z < 333)
                {
                    PlayerGun.Rotate(Vector3.forward * -speed);
                }
                else if (PlayerGun.eulerAngles.z > 334)
                {
                    PlayerGun.Rotate(Vector3.forward * -speed);
                }
            }
            else if (ShotAngle >= -130 && ShotAngle <= -30)
            {
                if (PlayerGun.eulerAngles.z < 26)
                {
                    PlayerGun.Rotate(Vector3.forward * -speed);
                }
                else if (PlayerGun.eulerAngles.z > 27)
                {
                    PlayerGun.Rotate(Vector3.forward * speed);
                }
            }

            yield return new WaitForSeconds(0.001f);
            StartCoroutine(RotateGunToDefault());
        }
    }

    IEnumerator Idle()
    {
        if (TrainingMode == "AimAndReaction")
        {
            ResetGame();
        }
        CharacterAnim.enabled = true;
        CharacterAnim.SetBool("Idle", true);
        AimLineAnim.SetBool("AimLine", true);
        AimLineRotator.freeze = false;
        yield return new WaitForSeconds(1f);
        ableToShoot = true;
        btnShoot.interactable = true;
    }

    public void Restart()
    {
        AimLineRotator.freeze = false;
        SceneManager.LoadScene("TrainingArena");
    }

    public void Pause()
    {        
        SceneManager.LoadScene("MainMenu");
    }

    public void Shoot()
    {
        if ((TrainingMode == "AimOnly") || (time <= 0 && TrainingMode == "AimAndReaction"))
        {
            int shots = int.Parse(total.text);
            shots++;
            total.text = shots + "";
            AimLineRotator.freeze = true;
            btnShoot.interactable = false;
            ableToShoot = false;
            AimLineAnim.SetBool("AimLine", false);
            AimLineCollider.enabled = false;
            RaycastHit2D hit = Physics2D.Raycast(AimLine.position, AimLine.right * -50);
            didHit = false;
            didHitGround = false;
            if (hit.collider != null)
            {
                HitPosition = hit.point;
                Distance = hit.distance;
                if (hit.collider.name != "Ground")
                {
                    didHit = true;                    
                    if (hit.collider.name == "Head")
                    {
                        // DISABLE CAMERA ZOOM CODES START
                        //StartCoroutine(StartTimePassed());
                        //didPlayerHeadshot = true;
                        // END
                        Headshots++;
                        GameObject.Find("Headshots").GetComponent<Text>().text = Headshots + " Headshots";
                    }
                    GameObject.Find("txtShotStatus").GetComponent<Text>().text = hit.collider.name;
                } else
                {
                    didHitGround = true;
                }
            }
            else
            {
                GameObject.Find("txtShotStatus").GetComponent<Text>().text = "Missed";
            }

            AimLineCollider.enabled = true;
            BulletRotation = AimLine.eulerAngles.z;
            ShotAngle = AimLine.eulerAngles.z;
            if (ShotAngle > 180)
            {
                ShotAngle -= 360;
            }
            ShotAngle = -ShotAngle;
            StartCoroutine(ShootAnimation());

            if (time <= 0 && TrainingMode == "AimAndReaction")
            {
                GameObject.Find("txtReaction").GetComponent<Text>().text = System.Math.Round(shot_time, 2) + "s";
                float LastBestReaction = float.Parse(GameObject.Find("txtBestReaction").GetComponent<Text>().text.Replace("s", ""));
                if (LastBestReaction > shot_time || LastBestReaction == 0)
                {
                    GameObject.Find("txtBestReaction").GetComponent<Text>().text = System.Math.Round(shot_time, 2) + "s";
                }
            }

        }
        else
        {
            t = Random.Range(1, 4);
            btnShoot.interactable = false;
            parent.enabled = true;
            foreach (Renderer child in listOfChildren)
            {
                child.enabled = true;
            }
            txtCooldown.text = t.ToString();
            txtCooldownS.text = "s";
            t--;
            StartCoroutine(Cooldown());
            GameObject.Find("GunBlow").GetComponent<AudioSource>().Play();
        }

    }

    IEnumerator HitEffects(float BulletArrivalTime, Vector3 HitPoisition)
    {
        yield return new WaitForSeconds(BulletArrivalTime);
        Instantiate(Resources.Load("BulletImpactWoodEffect"), new Vector3(HitPoisition.x, HitPoisition.y, 147), Quaternion.Euler(0f, 90f, 0f));
        GameObject.Find("BulletImpactSound").GetComponent<AudioSource>().Play();
        StartCoroutine(RemoveWoodImpacts());
    }

    IEnumerator HitEffectsOnGround(float BulletArrivalTime, Vector3 HitPoisition)
    {
        yield return new WaitForSeconds(BulletArrivalTime);
        Instantiate(Resources.Load("BulletImpactOnGround"), new Vector3(HitPoisition.x, HitPoisition.y, 147), Quaternion.Euler(0f, 90f, 0f));
        Destroy(GameObject.Find("BulletImpactOnGround") , 2f);
    }

    IEnumerator RemoveWoodImpacts()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(GameObject.Find("BulletImpactWoodEffect(Clone)"));
        Time.timeScale = 1f;
    }

    public void ResetGame()
    {
        BulletFollowCamTransform.position = MainCameraTransform.position;
        shot_time = 0f;
        time = Random.Range(3, 9);
        ShowHoldSign();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1.0f);
        txtCooldown.text = t.ToString();
        t--;
        if (t >= 0)
        {
            StartCoroutine(Cooldown());
        }
        else
        {
            parent.enabled = false;
            foreach (Renderer child in listOfChildren)
            {
                child.enabled = false;
            }
            txtCooldown.text = "";
            txtCooldownS.text = "";
            btnShoot.interactable = true;
        }
    }

    public void GenerateBullet()
    {
        if (didPlayerHeadshot == true)
        {
            StartCoroutine(ZoomOnTarget());
        }
        Transform gun = PlayerGun;
        GameObject obj = Instantiate((GameObject)Resources.Load("Bullet"), new Vector3(gun.position.x, gun.position.y + 0.050f, gun.position.z), Quaternion.Euler(0, 0, BulletRotation));
        obj.name = "pbullet";
        PlayerBulletTransform = GameObject.Find("pbullet").GetComponent<Transform>();
        Destroy(obj, 1.5f);
    }
}
