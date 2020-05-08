using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum PowerUps
{
    //, LongAimLine
    None, Health, Armor, DoubleHitPoint, DoubleShot, InstantKill
    // #6
}

public class GamePlayerScript : Player
{
    //public Image image;
    //public Text nameField;
    //public Text readyField;
    //public Text rollResultField;
    //public Text healthField;
    //public Text totalPointsField;

    private Button btnRematch;
    private Animator AimLineAnim, CharacterAnim;
    private RectTransform AimLine;
    private Collider2D AimLineCollider;
    private Image HealthBar;
    private Image PowerupImage;
    private Image ArmorBar;
    private Text txtCooldown, txtCooldownS;
    private GameObject CooldownParent, NoTimeParent;
    private AimLineRotator AimLineScr;
    private bool UpdateHealth = false, UpdatePowerup = false, startTimer = false, isAbleToShoot = true, ShowNoTime = true;
    private float TimeOut = 3.5f;
    bool isOpen = true;
    bool Waiting = true;
    float PassedTime = 0f;
    int Counter = 0, shotsCounter = 0, RoundsCounter = 1;
    float TimeoutTimer = 0f;
    GameSession gameSession;

    [SyncVar]
    public string spriteName;

    [SyncVar]
    public Color myColour;

    // Simple game states for a dice-rolling game

    [SyncVar]
    public int resetPlayer = 0;

    [SyncVar]
    public int totalPoints;

    [SyncVar]
    public int loadLobbyScene;

    [SyncVar]
    public bool shot;

    [SyncVar]
    public bool didShot = false;

    [SyncVar]
    public bool spawnBlood = false;

    [SyncVar]
    public bool spawnDust = false;

    [SyncVar]
    public float shotAngle;

    [SyncVar]
    public int health = 100;

    [SyncVar]
    public int earlyHealth = 100;

    [SyncVar]
    public int armor = 0;

    [SyncVar]
    public PowerUps powerUp;

    [SyncVar]
    public bool isAlive = true;

    [SyncVar]
    public int shotedPlayerIndex;


    [SyncVar]
    public float HitPositionX;
    [SyncVar]
    public float HitPositionY;

    private float AngleHolder = 0f;
    private int CooldownTimer = 0;
    private bool isTickinPlayin = false;


    void Start()
    {
        gameSession = GameSession.instance;

        NoTimeParent = GameObject.Find("OutTime");
        //NoTimeParent.SetActive(false);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        btnRematch = GameObject.Find("btnRematch").GetComponent<Button>();
        btnRematch.onClick.AddListener(() =>
        {
            RpcPlayAgain();
        });

        btnRematch.gameObject.SetActive(false);

        AimLineAnim = GameObject.Find("AimLineController").GetComponent<Animator>();
        AimLine = GameObject.Find("AimLineController").GetComponent<RectTransform>();
        AimLineCollider = GameObject.Find("AimLineController").GetComponent<Collider2D>();
        AimLineScr = GameObject.Find("AimLineController").GetComponent<AimLineRotator>();

        txtCooldown = GameObject.Find("CooldownTimer").GetComponent<Text>();
        txtCooldownS = GameObject.Find("CooldownTimerSecond").GetComponent<Text>();
        CooldownParent = GameObject.Find("NoShoot");

        CooldownParent.SetActive(false);

        GameObject.Find("btnShoot").GetComponent<Button>().interactable = true;
        Button btn = GameObject.Find("btnShoot").GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (isAbleToShoot == true && isAlive == true)
            {
                if (gameSession && gameSession.gameState == GameState.DuelRoundStart)
                {
                    CmdSetDidShot(true);
                    shotAngle = AimLineRotator.controller.eulerAngles.z;
                    AngleHolder = AimLineRotator.controller.eulerAngles.z;
                    AimLineRotator.freeze = true;
                    isAbleToShoot = false;
                    CmdShootAnimation();
                    HideAimLine();
                    Invoke("ShootAnimationContinue", 0.27f);
                    ShootRaycast();
                }
                else
                {
                    GameObject.Find("btnShoot").GetComponent<Button>().interactable = false;
                    CooldownTimer = Random.Range(1, 4);
                    StartCoroutine(Cooldown());
                    CooldownParent.SetActive(true);
                }
            }

        });


        //if (isServer)
        //{
        //    if (GameObject.Find("btnBack"))
        //    {
        //        Button btn = GameObject.Find("btnBack").GetComponent<Button>();
        //        btn.onClick.AddListener(() =>
        //        {
        //            CmdloadLobbyScene();
        //        });
        //    }
        //}
        //else
        //{
        //    if (GameObject.Find("btnBack"))
        //    {
        //        GameObject.Find("btnBack").SetActive(false);
        //    }
        //}

        // Send custom player info
        // This is an example of sending additional information to the server that might be needed in the lobby (eg. colour, player image, personal settings, etc.)
        //if (isLocalPlayer)
        //{
        //    image.GetComponent<Image>().sprite = Resources.Load<Sprite>(Globals.playerAppearance.CharacterSpritePath);
        //}
        //myColour = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);        
        spriteName = Globals.playerAppearance.CharacterSpritePath;
        myColour = new Color(1, 1, 1, 1);
        CmdSetCustomPlayerInfo(myColour, spriteName);
    }

    public void ShootRaycast()
    {
        shotsCounter++;
        if (AimLineCollider)
        {
            AimLineCollider.enabled = false;
        }
        RaycastHit2D hit = Physics2D.Raycast(AimLine.position, AimLine.right * -50);

        if (hit.collider != null && gameSession.roundType == "PowerUp")
        {
            if (hit.collider.name != "ArmorPowerup" && hit.collider.name != "ArmorPowerup2" && hit.collider.name != "RandomPowerup")
            {
                powerUp = new PowerUps();
                PowerupImage.color = new Color(0f, 0f, 0f, 0f);
                if (isServer)
                {
                    RpcResetPowerups();
                }
                else
                {
                    CmdResetPowerups();
                }
            }
        }
        else if (hit.collider == null && gameSession.roundType == "PowerUp")
        {
            powerUp = new PowerUps();
            PowerupImage.color = new Color(0f, 0f, 0f, 0f);
            if (isServer)
            {
                RpcResetPowerups();
            }
            else
            {
                CmdResetPowerups();
            }
        }

        if (hit.collider != null)
        {
            //Distance = hit.distance;
            if (hit.collider.name != "Ground")
            {
                int HitPoint = 0;
                string ShotType = "";
                if (hit.collider.name == "Head")
                {
                    HitPoint = 100;
                    ShotType = "HS";
                }
                else if (hit.collider.name == "Body")
                {
                    HitPoint = 45;
                    ShotType = "BS";
                }
                else if (hit.collider.name == "RightTopLeg")
                {
                    HitPoint = 20;
                    ShotType = "LS";
                }
                else if (hit.collider.name == "ArmorPowerup" || hit.collider.name == "ArmorPowerup2" || hit.collider.name == "RandomPowerup")
                {
                    HitPoint = 0;
                    ShotType = "PowerUp";
                }

                if (powerUp == PowerUps.DoubleHitPoint)
                {
                    HitPoint *= 2;
                }
                else if (powerUp == PowerUps.DoubleShot)
                {
                    HitPoint = Mathf.Abs(HitPoint - (HitPoint / 3));
                }
                else if (powerUp == PowerUps.InstantKill)
                {
                    HitPoint = 500;
                }

                if (hit.collider.name == "Hat")
                {
                    HitPoint = 0;
                    ShotType = "HatShot";
                }

                if (ShotType != "HatShot" && ShotType != "PowerUp")
                {
                    StartCoroutine(SpawnBlood((hit.distance / BulletScript.velocity), ShooterPlayerIndexV, hit.point.x, hit.point.y));
                }

                if (gameSession.roundType == "Kill")
                {
                    CmdUpdateHealth(HitPoint, int.Parse(hit.transform.name.Replace("Player", "")), ShotType, (hit.distance / BulletScript.velocity));
                    StartCoroutine(CheckHealth2(int.Parse(hit.transform.name.Replace("Player", ""))));
                }
                else if (gameSession.roundType == "Armor" || gameSession.roundType == "PowerUp")
                {
                    if (ShotType == "PowerUp")
                    {
                        ExploadScript ES = hit.collider.gameObject.GetComponent<ExploadScript>();
                        if (ES.SelfDestruction == false)
                        {
                            int x = 0;
                            if (RoundsCounter >= 10)
                            {
                                x = 6;
                            }
                            else
                            {
                                x = 5;
                            }

                            powerUp = (PowerUps)Random.Range(1, x);
                            //powerUp = (PowerUps)Random.Range(4, 5);
                            if (gameSession.roundType == "Armor")
                            {
                                powerUp = PowerUps.Armor;
                                CmdGetPowerUp(PowerUps.Armor);
                            }
                            else
                            {
                                CmdGetPowerUp(powerUp);
                            }

                            if (gameSession.roundType != "Armor")
                            {
                                ES.SpawnPowerup(powerUp.ToString());
                            }
                            ES.SelfDestructionDelay = (hit.distance / BulletScript.velocity);
                            ES.SelfDestruction = true;

                            if (isServer)
                            {
                                RpcDestoryPowerupBox(hit.collider.name, powerUp.ToString(), (hit.distance / BulletScript.velocity));
                            }
                            else
                            {
                                CmdDestoryPowerupBox(hit.collider.name, powerUp.ToString(), (hit.distance / BulletScript.velocity));
                            }
                        }
                    }
                }
            }
            else
            {
                //didHitGround = true;
                if (isServer)
                {
                    RpcSpawnGroundEffect(hit.point.x, hit.point.y);
                }
                else
                {
                    CmdSpawnGroundEffect(hit.point.x, hit.point.y);
                }
            }
        }
        else
        {
            //Missed
        }
        if (AimLineCollider)
        {
            AimLineCollider.enabled = true;
        }

        if (powerUp == PowerUps.DoubleShot && gameSession.roundType == "Kill")
        {
            if (shotsCounter < 2)
            {
                GameObject.Find("AimLineController").GetComponent<Animator>().SetBool("AimLine", true);
                AimLineRotator.freeze = false;
                isAbleToShoot = true;
            }
        }

        //NoTime Icon
        if (powerUp == PowerUps.DoubleShot && shotsCounter >= 2)
        {
            ShowNoTime = false;
        }
        else if (powerUp != PowerUps.DoubleShot)
        {
            ShowNoTime = false;
        }

    }

    [Command]
    public void CmdDestoryPowerupBox(string BoxObjectName, string PUName, float Delay)
    {
        RpcDestoryPowerupBox(BoxObjectName, PUName, Delay);
    }

    [ClientRpc]
    public void RpcDestoryPowerupBox(string BoxObjectName, string PUName, float Delay)
    {
        ExploadScript ES = GameObject.Find(BoxObjectName).GetComponent<ExploadScript>();
        if (gameSession.roundType != "Armor")
        {
            ES.SpawnPowerup(PUName);
        }
        ES.SelfDestructionDelay = Delay;
        ES.SelfDestruction = true;
    }

    IEnumerator SpawnBlood(float delay, int ShooterPlayerIndexV, float x, float y)
    {
        yield return new WaitForSeconds(delay);
        if (isServer)
        {
            RpcSpawnBloodEffect(ShooterPlayerIndexV, x, y);
        }
        else
        {
            CmdSpawnBlood(ShooterPlayerIndexV, x, y);
        }
    }

    [Command]
    public void CmdSetCustomPlayerInfo(Color aColour, string aSpriteName)
    {
        myColour = aColour;
        spriteName = aSpriteName;
    }

    GamePlayerScript GP;
    int HitedPlayerIndexV;
    int ShooterPlayerIndexV;
    string ShotTypeV;
    float EffectDelayV;
    int HitPointV, AHitPointV;

    [Command]
    public void CmdUpdateHealth(int HitPoint, int HitedPlayerIndex, string ShotType, float EffectDelay)
    {
        ShooterPlayerIndexV = playerIndex;
        HitedPlayerIndexV = HitedPlayerIndex;
        ShotTypeV = ShotType;
        EffectDelayV = EffectDelay;

        HitPointV = HitPoint;
        AHitPointV = 0;

        int ArmorBlockAmount = 1;
        int ArmorHit = 1;

        if (ShotTypeV == "HS")
        {
            ArmorBlockAmount = 45;
            ArmorHit = 55;
        }
        else if (ShotTypeV == "BS")
        {
            ArmorBlockAmount = 30;
            ArmorHit = 15;
        }
        else if (ShotTypeV == "LS")
        {
            ArmorBlockAmount = 15;
            ArmorHit = 5;
        }

        if (powerUp == PowerUps.DoubleHitPoint)
        {
            ArmorBlockAmount *= 2;
        }

        if (GameObject.Find("Canvas/PlayerContainer/Player" + HitedPlayerIndex + "Controller/Player" + HitedPlayerIndex))
        {
            GP = GameObject.Find("Canvas/PlayerContainer/Player" + HitedPlayerIndex + "Controller/Player" + HitedPlayerIndex).GetComponent<GamePlayerScript>();

            if (GP.armor > 0)
            {
                int d = Mathf.RoundToInt(ArmorBlockAmount / ArmorHit);

                if (GP.armor < AHitPointV)
                {
                    int PureHealthDecrease = ArmorHit - GP.armor;
                    ArmorBlockAmount = GP.armor * d;
                    ArmorHit = GP.armor;
                }
                else
                {
                    AHitPointV = ArmorHit;
                }
                HitPointV = HitPoint - ArmorBlockAmount;
            }

            GP.earlyHealth -= HitPointV;

            Invoke("CheckHealth", EffectDelay);
        }

    }

    [Command]
    public void CmdGetPowerUp(PowerUps powerup)
    {
        //Set Powerup Image
        Object[] SpriteSheet;
        SpriteSheet = Resources.LoadAll("Sprites/PowerUps");
        foreach (Object spt in SpriteSheet)
        {
            if (spt.name == powerup.ToString())
            {
                PowerupImage.sprite = (Sprite)spt;
                PowerupImage.color = new Color(1f, 1f, 1f, 0.9f);
            }
        }

        if (powerup == PowerUps.Health)
        {
            health += 50;
            if (health > 100)
            {
                health = 100;
            }
        }
        else if (powerup == PowerUps.Armor)
        {
            armor = 100;
        }
    
        RpcUpdatePowerUps();
    }

    [ClientRpc]
    public void RpcUpdatePowerUps()
    {
        UpdatePowerup = true;
    }

    [Command]
    public void CmdUpdatePowerUps()
    {
        RpcUpdatePowerUps();
    }

    IEnumerator CheckHealth2(int PlayerIndex)
    {
        yield return new WaitForSeconds(2f);
        //        UpdatePowerup = true;
        if (GameObject.Find("Canvas/PlayerContainer/Player" + PlayerIndex + "Controller/Player" + PlayerIndex))
        {
            GP = GameObject.Find("Canvas/PlayerContainer/Player" + PlayerIndex + "Controller/Player" + PlayerIndex).GetComponent<GamePlayerScript>();
            if (GP.health <= 0)
            {
                AimLineRotator.freeze = true;
                HideAimLine();
                CmdCelebrate();
            }
        }
    }

    [Command]
    public void CmdSetDidShot(bool val)
    {
        didShot = val;
    }

    [Command]
    public void CmdCelebrate()
    {
        GetComponent<Animator>().SetBool("Idle", false);
        GetComponent<Animator>().SetBool("Walking", false);
        GetComponent<Animator>().SetBool("Shoot", false);
        GetComponent<Animator>().SetInteger("CelebrateType", Random.Range(1, 4));
        GetComponent<Animator>().SetBool("Celebrate", true);
    }

    public void CheckHealth()
    {
        if (ShotTypeV == "HatShot")
        {
            if (isServer)
            {
                RpcHatEffect(HitedPlayerIndexV);
            }
            else
            {
                CmdHatEffect(HitedPlayerIndexV);
            }
        }

        GP.health -= HitPointV;
        GP.armor -= AHitPointV;
        if (GP.health <= 0)
        {
            GameObject.Find("Canvas/PlayerContainer/Player" + HitedPlayerIndexV + "Controller/Player" + HitedPlayerIndexV).GetComponent<Animator>().SetBool("Dead", true);
            isAbleToShoot = false;
            int DeathType = 0;
            if (ShotTypeV == "HS")
            {
                DeathType = 2;
            }
            else if (ShotTypeV == "LS")
            {
                DeathType = 1;
            }
            else if (ShotTypeV == "BS")
            {
                DeathType = 3;
            }
            isAlive = false;
            GameObject.Find("Canvas/PlayerContainer/Player" + HitedPlayerIndexV + "Controller/Player" + HitedPlayerIndexV).GetComponent<Animator>().SetInteger("DeathType", DeathType);
        }
        else if (GP.shot == true && (powerUp != PowerUps.DoubleShot || (powerUp == PowerUps.DoubleShot && shotsCounter >= 2)) && gameSession.gameState != GameState.GameOver)
        {
            RoundsCounter++;
            shotsCounter = 0;
            if (isServer && gameSession.roundType == "Kill")
            {
                AimLineRotator.freeze = false;
                GameObject.Find("AimLineController").GetComponent<Animator>().SetBool("AimLine", true);
                isAbleToShoot = true;
                RpcAbleToShoot();
                gameSession.RpcStartNewRound();
            }
            else
            {
                CmdAbleToShoot();
                CmdStartNewRound();
            }
        }
    }

    [Command]
    public void CmdAbleToShoot()
    {
        RpcAbleToShoot();
    }
    [Command]
    public void CmdStartNewRound()
    {
        gameSession.RpcStartNewRound();
        shotsCounter = 0;
    }

    [ClientRpc]
    public void RpcAbleToShoot()
    {
        AimLineRotator.freeze = false;
        GameObject.Find("AimLineController").GetComponent<Animator>().SetBool("AimLine", true);
        isAbleToShoot = true;
    }

    [Command]
    public void CmdHatEffect(int index)
    {
        RpcHatEffect(index);
    }

    [ClientRpc]
    public void RpcHatEffect(int PlayerIndex)
    {
        if (GameObject.Find("Canvas/PlayerContainer/Player" + PlayerIndex + "Controller/Player" + PlayerIndex + "/Body/Head/Hat").GetComponent<SpriteRenderer>().enabled == true)
        {
            GameObject.Find("Canvas/PlayerContainer/Player" + PlayerIndex + "Controller/Player" + PlayerIndex + "/Body/Head/Hat").GetComponent<SpriteRenderer>().enabled = false;
            GameObject GoneHatObj = Instantiate(Resources.Load("Hat"), GameObject.Find("Canvas/PlayerContainer/Player" + PlayerIndex + "Controller/Player" + PlayerIndex + "/Body/Head/Hat").transform.position, Quaternion.identity) as GameObject;
            if (isLocalPlayer)
            {
                GoneHatObj.GetComponent<Rigidbody2D>().velocity = new Vector2(-15f, 7f);
            }
            else
            {
                GoneHatObj.GetComponent<Rigidbody2D>().velocity = new Vector2(15f, 7f);
            }
            Destroy(GoneHatObj, 0.5f);
        }
    }

    [Command]
    public void CmdloadLobbyScene()
    {
        loadLobbyScene = Random.Range(1, 3);
    }

    [Command]
    public void CmdPlayAgain()
    {
        gameSession.PlayAgain();
    }

    [ClientRpc]
    public void RpcPlayAgain()
    {
        SceneManager.LoadScene("LocalMultiplayerArena");
    }

    [Command]
    public void CmdNoShot()
    {
        shot = false;
    }

    [Command]
    public void CmdSpawnBullet(float angle)
    {
        shot = true;
        shotAngle = angle;
    }

    [Command]
    public void CmdSpawnBlood(int ShooterIndex, float x, float y)
    {
        HitPositionX = x;
        HitPositionY = y;
        RpcSpawnBloodEffect(ShooterPlayerIndexV, x, y);
        //spawnBlood = true;
    }

    [Command]
    public void CmdSpawnGroundEffect(float x, float y)
    {
        HitPositionX = x;
        HitPositionY = y;
        RpcSpawnGroundEffect(x, y);
    }

    public void HideAimLine()
    {
        GameObject.Find("AimLineController").GetComponent<Animator>().SetBool("AimLine", false);
    }

    [Command]
    public void CmdShootAnimation()
    {
        GetComponent<Animator>().SetBool("Idle", false);
        GetComponent<Animator>().SetBool("Walking", false);
        GetComponent<Animator>().SetBool("Shoot", true);
    }

    [Command]
    public void CmdResetPowerups()
    {
        RpcResetPowerups();
    }

    [ClientRpc]
    public void RpcResetPowerups()
    {
        powerUp = new PowerUps();
        PowerupImage.color = new Color(0f, 0f, 0f, 0f);
        CmdUpdatePowerUps();
    }

    public void ShootAnimationContinue()
    {
        Transform PlayerGun = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/Body/GunHolster/SideGun").GetComponent<Transform>();
        GetComponent<Animator>().enabled = false;
        if (GetComponent<Animator>().GetInteger("Angle") >= -30 && GetComponent<Animator>().GetInteger("Angle") <= 30)
        {
            PlayerGun.rotation = Quaternion.Euler(0, 0, (GameObject.Find("AimLineController").GetComponent<RectTransform>().eulerAngles.z));
        }
        else if (GetComponent<Animator>().GetInteger("Angle") >= 30 && GetComponent<Animator>().GetInteger("Angle") <= 130)
        {
            PlayerGun.rotation = Quaternion.Euler(0, 0, (GameObject.Find("AimLineController").GetComponent<RectTransform>().eulerAngles.z));
        }
        else if (GetComponent<Animator>().GetInteger("Angle") >= -130 && GetComponent<Animator>().GetInteger("Angle") <= -30)
        {
            PlayerGun.rotation = Quaternion.Euler(0, 0, (GameObject.Find("AimLineController").GetComponent<RectTransform>().eulerAngles.z + 12.155f));
        }

        if (isServer)
        {
            RpcSpawnBullet(shotAngle);
        }
        else
        {
            CmdSpawnBullet(AngleHolder);
        }

        //CmdIdle();
        startTimer = true;
        LocalIdle();
    }


    [Command]
    public void CmdIdle()
    {
        GetComponent<Animator>().enabled = true;

        GetComponent<Animator>().SetBool("Idle", true);
        GetComponent<Animator>().SetBool("Shoot", false);
        GetComponent<Animator>().SetBool("Walking", false);
    }

    public void LocalIdle()
    {
        //GameObject.Find("AimLineController").GetComponent<Animator>().SetBool("AimLine", true);
        //AimLineRotator.freeze = false;
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().SetBool("Idle", true);
        GetComponent<Animator>().SetBool("Shoot", false);
        GetComponent<Animator>().SetBool("Walking", false);
    }

    [Command]
    public void CmdUpdateAngle(int angle)
    {
        GetComponent<Animator>().SetInteger("Angle", angle);
    }

    [ClientRpc]
    public void RpcSpawnBullet(float ZAngle)
    {
        GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/Body/GunHolster/SideGun").GetComponent<AudioSource>().Play();
        Transform gun = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/Body/GunHolster/SideGun").GetComponent<Transform>();
        GameObject obj;
        if (isLocalPlayer)
        {
            obj = Instantiate((GameObject)Resources.Load("Bullet"), new Vector3(gun.position.x, gun.position.y + 0.050f, gun.position.z), Quaternion.Euler(0, 0, ZAngle));
        }
        else
        {
            obj = Instantiate((GameObject)Resources.Load("Bullet"), new Vector3(gun.position.x, gun.position.y + 0.050f, gun.position.z), Quaternion.Euler(0, 0, (-180 - ZAngle)));
        }
        obj.transform.parent = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex).transform;
        obj.name = "Bullet";
        Destroy(obj, 1.5f);
    }

    [ClientRpc]
    public void RpcSpawnBloodEffect(int ShooterPlayerIndex, float x, float y)
    {
        GameObject.Find("Grunt").GetComponent<AudioSource>().Play();
        spawnBlood = false;
        GameObject obj;
        GameObject obj2;
        if (isLocalPlayer)
        {
            obj = (GameObject)Instantiate(Resources.Load("BloodParticleFront"), new Vector3(x - 0.1f, y, 147f), Quaternion.Euler(9.696f, 89.36401f, -90.00401f));
            obj2 = (GameObject)Instantiate(Resources.Load("BloodParticleBack"), new Vector3(x - 0.1f, y, 147f), Quaternion.Euler(0.076f, -75.802f, -89.981f));
        }
        else
        {
            obj = (GameObject)Instantiate(Resources.Load("BloodParticleFront"), new Vector3(-x + 0.1f, y, 147f), Quaternion.Euler(9.696f, -89.36401f, -90.00401f));
            obj2 = (GameObject)Instantiate(Resources.Load("BloodParticleBack"), new Vector3(-x + 0.1f, y, 147f), Quaternion.Euler(0.076f, 75.802f, -89.981f));
        }

        //int index = 0;
        //if (ShooterPlayerIndex == 0)
        //{
        //    index = 1;
        //}        
        //obj.transform.parent = GameObject.Find("Canvas/PlayerContainer/Player" + index + "Controller/Player" + index + "/Body").transform;
        //obj2.transform.parent = GameObject.Find("Canvas/PlayerContainer/Player" + index + "Controller/Player" + index + "/Body").transform;

        Destroy(obj, 3f);
        Destroy(obj2, 3f);
    }

    [ClientRpc]
    public void RpcSpawnGroundEffect(float x, float y)
    {
        spawnDust = false;
        GameObject obj;

        if (isLocalPlayer)
        {
            obj = (GameObject)Instantiate(Resources.Load("BulletImpactOnGround"), new Vector3(x, y, 147f), Quaternion.identity);
        }
        else
        {
            obj = (GameObject)Instantiate(Resources.Load("BulletImpactOnGround"), new Vector3(-x, y, 147f), Quaternion.identity);
        }
        Destroy(obj, 3f);
    }


    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        // Brief delay to let SyncVars propagate
        Invoke("ShowPlayer", 0.3f);
    }

    public override void OnClientReady(bool readyState)
    {
        if (readyState)
        {
            //readyField.text = "READY!";
            //readyField.color = Color.green;
        }
        else
        {
            //readyField.text = "not ready";
            //readyField.color = Color.red;
        }
    }

    void ShowPlayer()
    {
        if (!isServer)
        {
            GameObject cplayer = Instantiate(Resources.Load("PlayerController"), Vector3.zero, Quaternion.identity) as GameObject;
            cplayer.name = "Player" + playerIndex + "Controller";
            cplayer.transform.SetParent(GameObject.Find("Canvas/PlayerContainer").transform, false);

            GameObject cprofile = (GameObject)Instantiate(Resources.Load("ProfileInfo"), Vector3.zero, Quaternion.identity);
            cprofile.name = "Player" + playerIndex + "Profile";
            cprofile.transform.SetParent(GameObject.Find("Canvas").transform, false);

            gameObject.name = "Player" + playerIndex;
        }

        HealthBar = GameObject.Find("Player" + playerIndex + "Profile/HealthBar").GetComponent<Image>();
        PowerupImage = GameObject.Find("Player" + playerIndex + "Profile/PowerupImage").GetComponent<Image>();
        ArmorBar = GameObject.Find("Player" + playerIndex + "Profile/ArmorBar").GetComponent<Image>();
        // yo
        ArmorBar.fillAmount = 1;

        transform.SetParent(GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller").transform, false);

        GetComponent<Animator>().SetBool("Walking", true);

        AimLineRotator.freeze = false;

        if (isLocalPlayer)
        {
            //AimLine.position = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/PlayerAimLinePosition").GetComponent<RectTransform>().position;
            transform.parent.position = GameObject.Find("LocalPlayerPosition").transform.position;
            GameObject.Find("Player" + playerIndex + "Profile").transform.position = GameObject.Find("LocalPlayerProfilePosition").transform.position;
            GameObject.Find("Player" + playerIndex + "Profile").transform.rotation = Quaternion.Euler(0, 180, 0);
            AimLineRotator.CharacterAnim = GetComponent<Animator>();
            AimLineRotator.txtAngle = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/" + "Player" + playerIndex + "/Body/Head/AngleWood/txtAngle").GetComponent<Text>();
            //GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller").GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 0f);
        }
        else
        {
            transform.parent.position = GameObject.Find("OpponentPosition").transform.position;
            transform.parent.rotation = Quaternion.Euler(0, 180, 0);
            GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/" + "Player" + playerIndex + "/Body/Head/AngleWood").SetActive(false);
            GameObject.Find("Player" + playerIndex + "Profile").transform.position = GameObject.Find("OpponentProfilePosition").transform.position;
            //GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller").GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 0f);
        }

        //AimLineAnim.SetBool("AimLine", false);
        //Invoke("CmdStopWalking", 3.3f);

        //AimLineRotator.character_controller = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/Body/AimLineHelper").GetComponent<Transform>();
        //AimLineRotator.SyncAimLinePostion = true;
        UpdateHealth = true;

        //image.color = myColour;
        //image.GetComponent<Image>().sprite = Resources.Load<Sprite>(spriteName);
        //nameField.text = deviceName;
        //readyField.gameObject.SetActive(true);

        //rollResultField.gameObject.SetActive(false);
        //totalPointsField.gameObject.SetActive(false);

        OnClientReady(IsReady());
    }

    [Command]
    public void CmdStopWalking()
    {
        Waiting = false;
        //2.5s
        GetComponent<Animator>().SetBool("Walking", false);
        //0.5s
        GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller").GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        GetComponent<Animator>().SetBool("Idle", true);
        //0.5s
        GameObject.Find("AimLineController").GetComponent<Animator>().SetBool("AimLine", true);
        //ableToShoot = true;
        //btnShoot.interactable = true;

        gameSession.gameState = GameState.Countdown;
    }

    bool var = true;
    bool var2 = true;
    public void Update()
    {
        if (Globals.networkData.isHost == 0 && Globals.networkData.ConnectionState == 1 && !networkManager.IsConnected())
        {
            networkManager.StartJoining();
        }
        if (gameSession.gameState == GameState.DuelRoundStart)
        {
            if (TimeoutTimer == 0)
            {
                GameObject.Find("StartSound").GetComponent<AudioSource>().Play();
            }
            TimeoutTimer += Time.deltaTime;
            GameObject.Find("Fire").GetComponent<SpriteRenderer>().sortingOrder = 1;
            GameObject.Find("Hold").GetComponent<SpriteRenderer>().sortingOrder = 0;
            //print(TimeoutTimer + " > " + TimeOut + "&&" + isServer + " : True && " + gameSession.gameState);            
            if (TimeoutTimer > TimeOut && isServer && gameSession.gameState != GameState.GameOver)
            {
                gameSession.RpcStartNewRound();

                TimeoutTimer = 0f;
                shotsCounter = 0;

                AimLineRotator.freeze = false;
                GameObject.Find("AimLineController").GetComponent<Animator>().SetBool("AimLine", true);
                isAbleToShoot = true;
                RpcAbleToShoot();
                RoundsCounter++;
            }
        }
        else if (gameSession.gameState == GameState.DuelRoundWaiting)
        {
            GameObject.Find("Fire").GetComponent<SpriteRenderer>().sortingOrder = 0;
            GameObject.Find("txtCountdown").GetComponent<Text>().text = "";
            GameObject.Find("Hold").GetComponent<SpriteRenderer>().sortingOrder = 1;
            CmdSetDidShot(false);
        }
        else if (gameSession.gameState == GameState.Countdown)
        {
            //if (didShot == false && gameSession.roundType == "PowerUp" && isLocalPlayer)
            if (isLocalPlayer && gameSession.roundType == "PowerUp" && isServer)
            {
                powerUp = new PowerUps();
                PowerupImage.color = new Color(0f, 0f, 0f, 0f);
                if (isServer)
                {
                    RpcResetPowerups();
                }
                else
                {
                    CmdResetPowerups();
                }

                //if (powerUp == PowerUps.LongAimLine)
                //{
                //    GameObject.Find("AimLine").transform.localScale = new Vector3(1003.1f, 10.3f, 63.7f);
                //    GameObject.Find("AimLine").transform.localPosition = new Vector3(501.55f, 0.3f, 0f);
                //    GameObject.Find("AimLine").GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(7, 1));
                //}
                //else
                //{
                //    GameObject.Find("AimLine").transform.localScale = new Vector3(304.5f, 10.3f, 63.7f);
                //    GameObject.Find("AimLine").transform.localPosition = new Vector3(152.2f, 0.3f, 0f);
                //    GameObject.Find("AimLine").GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(2.5f, 1f));
                //}
            }
            else if (didShot == false && gameSession.roundType == "PowerUp" && isLocalPlayer)
            {
                powerUp = new PowerUps();
                PowerupImage.color = new Color(0f, 0f, 0f, 0f);
                if (isServer)
                {
                    RpcResetPowerups();
                }
                else
                {
                    CmdResetPowerups();
                }

                //if (powerUp == PowerUps.LongAimLine)
                //{
                //    GameObject.Find("AimLine").transform.localScale = new Vector3(1003.1f, 10.3f, 63.7f);
                //    GameObject.Find("AimLine").transform.localPosition = new Vector3(501.55f, 0.3f, 0f);
                //    GameObject.Find("AimLine").GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(7, 1));
                //}
                //else
                //{
                //    GameObject.Find("AimLine").transform.localScale = new Vector3(304.5f, 10.3f, 63.7f);
                //    GameObject.Find("AimLine").transform.localPosition = new Vector3(152.2f, 0.3f, 0f);
                //    GameObject.Find("AimLine").GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(2.5f, 1f));
                //}
            }

            TimeoutTimer = 0f;
            shotsCounter = 0;
            ShowNoTime = true;
        }

        if (earlyHealth <= 0)
        {
            gameSession.gameState = GameState.GameOver;
            isAlive = false;
        }
        if (isAlive == false && Counter == 0)
        {
            GameObject.Find("btnShoot").GetComponent<Button>().interactable = false;
            HideAimLine();
            Counter++;
        }
        if (networkManager.NumPlayers() >= 2 && Waiting == true && GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller"))
        {
            if (isLocalPlayer)
            {
                GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller").GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 0f);
            }
            else
            {
                GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller").GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 0f);
            }
            Invoke("CmdStopWalking", 3.6f);
        }

        if (startTimer == true)
        {
            PassedTime += Time.deltaTime;
            if (PassedTime > 0.5f)
            {
                CmdIdle();
                startTimer = false;
                PassedTime = 0f;
            }
        }
        if (isLocalPlayer)
        {
            if (gameSession.gameState == GameState.GameOver)
            {
                NoTimeParent.GetComponent<AudioSource>().Stop();
                isTickinPlayin = false;
                //NoTimeParent.SetActive(false);
                NoTimeParent.GetComponent<SpriteRenderer>().sortingOrder = -12;
                GameObject.Find("Hourglass").GetComponent<SpriteRenderer>().sortingOrder = -12;

                GameObject.Find("Fire").GetComponent<SpriteRenderer>().sortingOrder = -20;
                GameObject.Find("Hold").GetComponent<SpriteRenderer>().sortingOrder = -20;
                GameObject.Find("Warning").GetComponent<SpriteRenderer>().sortingOrder = -20;

                if (health <= 0)
                {
                    if (GP)
                    {
                        if (GP.health <= 0)
                        {
                            GameObject.Find("txtResult").GetComponent<Text>().text = "Draw!";
                            GameObject.Find("Canvas/PlayerContainer/Player" + HitedPlayerIndexV + "Controller/Player" + HitedPlayerIndexV).GetComponent<Animator>().SetBool("Dead", true);
                            GameObject.Find("Canvas/PlayerContainer/Player" + HitedPlayerIndexV + "Controller/Player" + HitedPlayerIndexV).GetComponent<Animator>().SetBool("Celebrate", false);
                        }
                        else
                        {
                            GameObject.Find("txtResult").GetComponent<Text>().text = "You Lost!";
                        }
                    }
                    else
                    {
                        GameObject.Find("txtResult").GetComponent<Text>().text = "You Lost!";
                    }
                }
                else
                {
                    GameObject.Find("txtResult").GetComponent<Text>().text = "You Won!";
                }
                if (isServer)
                {
                    btnRematch.gameObject.SetActive(true);
                }
            }
            if (GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/Body/AimLineHelper"))
            {
                GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/PlayerAimLinePosition").transform.position = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/Body/AimLineHelper").GetComponent<Transform>().position;
            }
            if (GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/PlayerAimLinePosition") && AimLine.position != GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/PlayerAimLinePosition").transform.position)
            {
                AimLine.position = GameObject.Find("Canvas/PlayerContainer/Player" + playerIndex + "Controller/Player" + playerIndex + "/PlayerAimLinePosition").GetComponent<RectTransform>().position;
            }

            if (AimLineRotator.txtAngle)
            {
                CmdUpdateAngle(int.Parse(AimLineRotator.txtAngle.text.Replace("°", string.Empty)));
            }
        }
        if (UpdateHealth)
        {
            //AimLine.position = AimLineHelper.position;
            HealthBar.fillAmount = health / 100f;
            ArmorBar.fillAmount = armor / 100f;
        }
        if (UpdatePowerup)
        {
            //Set Powerup Image
            Object[] SpriteSheet;
            SpriteSheet = Resources.LoadAll("Sprites/PowerUps");
            if (powerUp != PowerUps.None)
            {
                foreach (Object spt in SpriteSheet)
                {
                    if (spt.name == powerUp.ToString())
                    {
                        PowerupImage.sprite = (Sprite)spt;
                        PowerupImage.color = new Color(1f, 1f, 1f, 0.9f);
                    }
                }
                if (isLocalPlayer)
                {
                    PowerupImage.gameObject.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else
            {
                PowerupImage.color = new Color(0f, 0f, 0f, 0f);
            }
            //UpdatePowerup = false;
        }
        if (shot == true)
        {
            if (!isServer)
            {
                CmdSpawnBullet(AngleHolder);
            }
            else
            {
                RpcSpawnBullet(shotAngle);
            }
            CmdNoShot();
        }
        //totalPointsField.text = "Points: " + totalPoints.ToString();

        if (loadLobbyScene > 0)
        {
            if (isOpen == true)
            {
                //networkManager.CloseServer();
                if (networkManager.discoveryServer.running)
                {
                    networkManager.discoveryServer.StopBroadcast();
                }
                isOpen = false;
            }
            if (!isServer)
            {
                ConfigNetworkScript.SaveNData(0, 1);
                SceneManager.LoadScene("Lobby");
            }
            else if (networkManager.NumPlayers() == 1)
            {
                ConfigNetworkScript.SaveNData(1, 1);
                SceneManager.LoadScene("Lobby");
            }
        }
        if (TimeoutTimer > 1 && isAbleToShoot == true && isAlive == true && ShowNoTime == true)
        {
            if (isTickinPlayin == false)
            {
                NoTimeParent.GetComponent<AudioSource>().Play();
                isTickinPlayin = true;
            }
            //NoTimeParent.SetActive(true);
            NoTimeParent.GetComponent<SpriteRenderer>().sortingOrder = 5;
            GameObject.Find("Hourglass").GetComponent<SpriteRenderer>().sortingOrder = 6;
        }
        else
        {
            NoTimeParent.GetComponent<AudioSource>().Stop();
            isTickinPlayin = false;
            //NoTimeParent.SetActive(false);
            NoTimeParent.GetComponent<SpriteRenderer>().sortingOrder = -12;
            GameObject.Find("Hourglass").GetComponent<SpriteRenderer>().sortingOrder = -12;
        }
    }

    [ClientRpc]
    public void RpcOnStartedGame()
    {
        //readyField.gameObject.SetActive(false);

        //rollResultField.gameObject.SetActive(true);
        //totalPointsField.gameObject.SetActive(true);
    }

    IEnumerator Cooldown()
    {
        txtCooldownS.text = "s";
        txtCooldown.text = CooldownTimer.ToString();
        CooldownTimer--;
        yield return new WaitForSeconds(1.0f);
        if (CooldownTimer > 0)
        {
            StartCoroutine(Cooldown());
        }
        else
        {
            CooldownParent.SetActive(false);
            txtCooldown.text = "";
            txtCooldownS.text = "";
            GameObject.Find("btnShoot").GetComponent<Button>().interactable = true;
        }
    }


    //Power UPS Functions



}