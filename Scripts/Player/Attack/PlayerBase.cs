using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviourPunCallbacks, IWeaponLevel, IRestart
{
    #region Variables
    public RPCManager rpcManager;
    public PlayerWeapons playerWeapons;
    public PlayerMovement playerMovement;
    public LoadingManager loadingManager;
    public GameManager gameManager;
    public Animator animator;

    public LevelScriptableObject levelValues;

    public Transform[] weaponUI;

    public Transform[] skillSpawnPoints;

    public GameObject[] skillGameObjects = new GameObject[4];

    public float[] coolDowns;

    public RaycastHit hit;
    public Ray ray;

    public Vector3 mouseInput;
    public Vector3 areaIndicatorPosition;

    public bool skill1Available = true;
    public bool skill2Available = true;
    public bool skill3Available = true;
    public bool skill4Available = true;

    public bool[] skillUse = new bool[4];

    public int level;

    private bool directionIndicator = false;
    private bool areaIndicator = false;

    private float rotationY;

    #endregion

    #region Properities
    public bool Skill1Available { get { return skill1Available; } set { skill1Available = value; } }
    public bool Skill2Available { get { return skill2Available; } set { skill2Available = value; } }
    public bool Skill3Available { get { return skill3Available; } set { skill3Available = value; } }
    public bool Skill4Available { get { return skill4Available; } set { skill4Available = value; } }
    public bool DirectionIndicator { get { return directionIndicator; } set { directionIndicator = value; } }
    public bool AreaIndicator { get { return areaIndicator; } set { areaIndicator = value; } }

    public Vector3 MouseInput { get { return mouseInput; } set { mouseInput = value; } }

    public float RotationY { get { return rotationY; } set { rotationY = value; } }

    #endregion

    public virtual void Awake()
    {
        rpcManager = RPCManager.instance;
        playerWeapons = PlayerWeapons.instance;
        loadingManager = LoadingManager.instance;
        gameManager = GameManager.instance;

        playerMovement = GetComponent<PlayerMovement>();

        SaveInsideOfManager();
        if (!photonView.IsMine)
        {
            this.enabled = false;
            return;
        }
    }

    public virtual void Start()
    {
        CameraFollow.instance.ChangeCameraTarget(this.transform);
        //RestartGameManager.instance.AddRestart(this);
    }
    public virtual void Attack()
    {

    }
    public virtual void Skill1(InputAction.CallbackContext context)
    {

    }
    public virtual void Skill2(InputAction.CallbackContext context)
    {

    }
    public virtual void Skill3(InputAction.CallbackContext context)
    {

    }
    public virtual void Skill4(InputAction.CallbackContext context)
    {

    }

    public virtual void SetTrigger(string key)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(key))
            animator.SetTrigger(key);
    }
    public virtual void SetBool(string key, bool m_value)
    {
        animator.SetBool(key, m_value);
    }
    public virtual IEnumerator OpenDirectionIndicator()
    {
        DirectionIndicator = true;

        weaponUI[0].gameObject.SetActive(true);

        weaponUI[0].rotation = Quaternion.Euler(Vector3.zero);

        while (true)
        {
            if (!DirectionIndicator)
            {
                weaponUI[0].gameObject.SetActive(false);
                yield break;
            }

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                mouseInput = new Vector3(hit.point.x, 0, hit.point.z);
            }

            Vector3 tempDirecton = new Vector3(weaponUI[0].position.x, 0, weaponUI[0].position.z);

            Quaternion tempRotation = Quaternion.LookRotation(mouseInput - tempDirecton);

            weaponUI[0].rotation = tempRotation;

            rotationY = weaponUI[0].eulerAngles.y;

            yield return null;
        }
    }

    public virtual IEnumerator OpenAreaIndicator()
    {
        AreaIndicator = true;

        Transform movingCircle = weaponUI[1].GetChild(1).transform;

        float yHeight = movingCircle.transform.position.y;

        weaponUI[1].gameObject.SetActive(true);

        weaponUI[1].rotation = Quaternion.Euler(Vector3.zero);

        while (true)
        {
            if (!AreaIndicator)
            {
                weaponUI[1].gameObject.SetActive(false); yield break;
            }

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.CompareTag("AreaIndicator"))
                {
                    mouseInput = new Vector3(hit.point.x, yHeight, hit.point.z);

                    areaIndicatorPosition = mouseInput;

                    movingCircle.position = areaIndicatorPosition;

                    Vector3 tempDirecton = new Vector3(transform.GetChild(0).position.x, yHeight, transform.GetChild(0).position.z);

                    Quaternion tempRotation = Quaternion.LookRotation(mouseInput - tempDirecton);

                    transform.GetChild(0).rotation = tempRotation;
                }
            }

            yield return null;
        }
    }

    public virtual void OpenDirectionIndicatorGamepad(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        print(context.control.device);
        if (!DirectionIndicator)
        {
            weaponUI[0].gameObject.SetActive(false);
            return;
        }

        DirectionIndicator = true;

        weaponUI[0].gameObject.SetActive(true);

        weaponUI[0].rotation = Quaternion.Euler(Vector3.zero);

        Vector2 rotationVector = context.ReadValue<Vector2>();

        float rotationAngle = Mathf.Atan2(rotationVector.x, rotationVector.y) * Mathf.Rad2Deg;

        weaponUI[0].eulerAngles += new Vector3(0, rotationAngle, 0);

        rotationY = weaponUI[0].eulerAngles.y;

    }

    public virtual IEnumerator SkillCooldown(byte m_skillType)
    {
        float cooldown = coolDowns[m_skillType - 1];

        if (m_skillType == 1)
        {
            Skill1Available = false;
        }
        else if (m_skillType == 2)
        {
            Skill2Available = false;
        }
        else if (m_skillType == 3)
        {
            Skill3Available = false;
        }
        else if (m_skillType == 4)
        {
            Skill4Available = false;
        }
        else
        {
            Debug.LogError("Hatali Yetenek girildi");
        }

        yield return new WaitForSeconds(cooldown);

        if (m_skillType == 1)
        {
            Skill1Available = true;
            yield break;
        }
        else if (m_skillType == 2)
        {
            Skill2Available = true;
            yield break;
        }
        else if (m_skillType == 3)
        {
            Skill3Available = true;
            yield break;
        }
        else if (m_skillType == 4)
        {
            Skill4Available = true;
            yield break;
        }
        else
        {
            Debug.LogError("Hatali Yetenek girildi");
        }
    }

    public virtual IEnumerator StopMoving(float m_time)
    {
        playerMovement.CanMove = false;
        yield return new WaitForSeconds(m_time);
        playerMovement.CanMove = true;
    } // YAP : buradaki stop move animator event cek

    public virtual void IncreaseLevel()
    {

    }
    public virtual void SaveInsideOfManager()
    {

    }

    public void ChangeCursor(bool m_boolen)
    {
        if (!m_boolen)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;
    }

    public void SaveAbility(int index, GameObject obj)
    {
        skillGameObjects[index] = obj;
    }

    public virtual void Restart()
    {
        transform.position = PlayerSpawner.instance.spawnPoints[(int)GetComponent<PlayerCurrentType>().currentType].position;
    }
}
