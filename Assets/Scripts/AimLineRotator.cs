using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class AimLineRotator : MonoBehaviour
{
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;
    public static bool freeze = false;
    Transform character;
    public static RectTransform controller;
    public static Transform character_controller;
    public static Text txtAngle;
    public static Animator CharacterAnim;
    public bool ableToRotate = false;
    float va1 = 0f;
    float va2 = 0f;

    private float r;
    private bool start_rotation = true;
    private bool find_new_values = true;
    private bool StartUpdate = false;
    private bool rotate_to_right, rotate_to_left;

    void Start()
    {
        _sensitivity = 0.4f;
        if (Application.platform != RuntimePlatform.WindowsPlayer)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/optionsdata.sav", FileMode.Open);
            Globals.optionsdata = (Optionsdata)bf.Deserialize(file);
            if (Globals.optionsdata.Sensitivity == 0)
            {
                _sensitivity = 0.4f;
            }
            else
            {
                _sensitivity = Globals.optionsdata.Sensitivity;
            }
        }

        _rotation = Vector3.zero;
        rotate_to_right = true;
        rotate_to_left = true;
        if (SceneManager.GetActiveScene().name == "TrainingArena")
        {
            character = GameObject.Find("Character").GetComponent<Transform>();
            CharacterAnim = GameObject.Find("Character").GetComponent<Animator>();
            character_controller = GameObject.Find("AimLineHelper").GetComponent<Transform>();
        }
        controller = GameObject.Find("AimLineController").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (character_controller)
        {
            controller.position = character_controller.position;
        }
        if (txtAngle && controller)
        {
            if (Mathf.Round(controller.eulerAngles.z) > 180 && txtAngle)
            {
                txtAngle.text = (-1 * Mathf.Round(controller.eulerAngles.z - 360)) + "°";
            }
            else
            {
                if (txtAngle)
                {
                    txtAngle.text = (-1 * Mathf.Round(controller.eulerAngles.z)) + "°";
                }
            }

            if (txtAngle)
            {
                CharacterAnim.SetInteger("Angle", int.Parse(txtAngle.text.Replace("°", string.Empty)));
            }

            if (start_rotation == true)
            {
                start_rotation = false;
                StartCoroutine(RandomRotate());
            }

            if (Mathf.Abs(controller.eulerAngles.z) > 75 && Mathf.Abs(controller.eulerAngles.z) <= 150)
            {
                controller.rotation = Quaternion.Euler(0, 0, 75);
            }
            else if (Mathf.Abs(controller.eulerAngles.z) <= 285 && Mathf.Abs(controller.eulerAngles.z) >= 160)
            {
                controller.rotation = Quaternion.Euler(0, 0, -75);
            }

            if (Mathf.Abs(controller.eulerAngles.z) >= 75 && Mathf.Abs(controller.eulerAngles.z) <= 150)
            {
                rotate_to_left = false;
            }
            else
            {
                rotate_to_left = true;
            }

            if (Mathf.Abs(controller.eulerAngles.z) <= 285 && Mathf.Abs(controller.eulerAngles.z) >= 160)
            {
                rotate_to_right = false;
            }
            else
            {
                rotate_to_right = true;
            }

            //Debug.Log("Rotate to Left : " + rotate_to_left + "  Rotate to Right : " + rotate_to_right);


            if (_isRotating == true && ableToRotate == true && Input.touchCount < 2 && freeze == false)
            {
                // offset
                _mouseOffset = (Input.mousePosition - _mouseReference);

                // apply rotation
                _rotation.z = -(_mouseOffset.y) * _sensitivity;

                // rotate
                if (rotate_to_right == false || rotate_to_left == false)
                {
                    if (rotate_to_right == false && _mouseOffset.y < 0)
                    {
                        transform.Rotate(_rotation);
                    }
                    else if (rotate_to_left == false && _mouseOffset.y > 0)
                    {
                        transform.Rotate(_rotation);
                    }
                }
                else
                {
                    transform.Rotate(_rotation);
                }

                // store mouse
                _mouseReference = Input.mousePosition;
            }

        }
    }

    void OnMouseDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.name == "AimLineController")
            {
                ableToRotate = true;
            }
            else
            {
                ableToRotate = false;
            }
        }
        else
        {
            ableToRotate = false;
        }
    }

    IEnumerator RandomRotate()
    {
        float speed = 0.5f;
        yield return new WaitForSeconds(0.025f);
        //Debug.Log("R : " + r + " | Rotation : " + _rotation.z);
        if (find_new_values == true)
        {
            va1 = Random.Range(8f, 12f);
            va2 = Random.Range(-8f, -12f);
            int va3 = Random.Range(0, 2);
            if (va3 == 0)
            {
                r = va1;
            }
            else
            {
                r = va2;
            }
            find_new_values = false;
        }

        if (r < 0)
        {
            r += 0.1f;
            _rotation.z = speed;
            if (rotate_to_left == true && freeze == false)
            {
                transform.Rotate(_rotation);
            }
        }
        else
        {
            r -= 0.1f;
            _rotation.z = -speed;
            if (rotate_to_right == true && freeze == false)
            {
                transform.Rotate(_rotation);
            }
        }
        if (r < 0.1f && r > -0.1f)
        {
            r = 0;
        }
        else
        {
            StartCoroutine(RandomRotate());
        }

        if (r == 0)
        {
            _rotation.z = 0;
            start_rotation = true;
            find_new_values = true;
        }
    }

    void OnMouseDown()
    {
        // rotating flag
        _isRotating = true;

        // store mouse
        _mouseReference = Input.mousePosition;
    }

    void OnMouseUp()
    {
        //StartCoroutine(RotatingTransition());
        // rotating flag
        _isRotating = false;
    }

    //int temp = 10;

    //IEnumerator RotatingTransition()
    //{
    //    _rotation.z += 0.1f;
    //    transform.Rotate(_rotation);

    //    yield return new WaitForSeconds(0.01f);
    //    if (temp != 0)
    //    {
    //        StartCoroutine(RotatingTransition());
    //        temp--;
    //    }
    //    else
    //    {
    //        temp = 10;
    //    }
    //}

}
