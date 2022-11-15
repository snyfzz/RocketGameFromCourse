using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RocketControl : MonoBehaviour
{
    enum State {Playing, Dead, NextLevel };

    private Rigidbody rb;
    private AudioSource audioSource;
    [SerializeField] Text fuelUi;
    private Scene thisLevel;
    private State state;
    private bool god = false;
    private int fuel = 300;

    [SerializeField] float strong = 1200f;
    [SerializeField] float rotationStrong = 110f;
    [SerializeField] int minusFuel = 5;

    [SerializeField] AudioClip flySound;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;
    [SerializeField] ParticleSystem flyParticle;
    [SerializeField] ParticleSystem loseParticle;

    public Camera mainCamera;
    public Camera secondCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        state = State.Playing;

        rb.constraints = RigidbodyConstraints.FreezePositionZ;

        mainCamera.enabled = false;
        secondCamera.enabled = true;
        Invoke("ShowLevel", 3f);

        fuelUi.text = fuel.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(state == State.Playing)
        {
            RocketRotate();
            RocketLaunch();
            if (Debug.isDebugBuild)
                DebugKeys();
        }
        thisLevel = SceneManager.GetActiveScene();

        CameraControl();
    }

    void FuelSystem()
    {
        fuel -= Mathf.RoundToInt(minusFuel * Time.deltaTime);
        fuelUi.text = fuel.ToString();
    }

    void RocketRotate()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, 0, rotationStrong * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 0, -rotationStrong * Time.deltaTime));
        }
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
    }

    void RocketLaunch()
    {
        if (Input.GetKey(KeyCode.Space) && fuel > 0)
        {
            FuelSystem();
            rb.AddRelativeForce(Vector3.up * strong * Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(flySound);
                flyParticle.Play();
            }
        }
        else if (!Input.GetKey(KeyCode.Space) || fuel <= 0)
        {
            audioSource.Pause();
            flyParticle.Stop();
        }
    }

    void LoadNextLevel()
    {
        if (SceneManager.sceneCountInBuildSettings == thisLevel.buildIndex + 1)
            SceneManager.LoadScene(0);
        SceneManager.LoadScene(thisLevel.buildIndex + 1);
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(thisLevel.buildIndex);
    }

    void PlaySound()
    {
        if (state == State.Dead)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(loseSound);
        }
        else if (state == State.NextLevel)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(winSound);
        }
            }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.Dead || state == State.NextLevel || god)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.NextLevel;
                PlaySound();
                Invoke("LoadNextLevel", 2f);
                break;
            default:
                state = State.Dead;
                rb.constraints = RigidbodyConstraints.None;
                PlaySound();
                flyParticle.Stop();
                loseParticle.Play();
                Invoke("ReloadLevel", 2f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Battery")
        {
            AddFuel(other.gameObject);
        }
    }

    void AddFuel(GameObject collision)
    {
        Destroy(collision.gameObject);
        fuel += 150;
        fuelUi.text = fuel.ToString();
    }

    void ShowLevel()
    {
        mainCamera.enabled = true;
        secondCamera.enabled = false;
    }

    void CameraControl()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            mainCamera.enabled = false;
            secondCamera.enabled = true;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            mainCamera.enabled = true;
            secondCamera.enabled = false;
        }
    }

    void DebugKeys()
    {
        if (Input.GetKey(KeyCode.C))
        {
            god = !god;
        }
        else if (Input.GetKey(KeyCode.V))
        {
            LoadNextLevel();
        }
    }
}
