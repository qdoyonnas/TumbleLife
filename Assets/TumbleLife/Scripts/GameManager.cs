using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager instance {
         get {
            return _instance;
        }
    }

    public GameObject blockPrefab;

    public Camera mainCamera;

    ConfigurableJoint joint;
    public TumbleBlock grabbedBlock = null;

    public float boundsHeight = -10f;

    public float spawnMin = 0.5f;
    public float spawnMax = 1.5f;
    float spawnTime = 1f;

    float startTime = 0;

    public List<TumbleBlock> blocks = new List<TumbleBlock>();

    public Canvas menuCanvas;
    public Transitional introTitle;
    public Transitional scoreCounter;
    public Transitional replayScreen;
    public Text replayScoreCounter;

    public float pitchVariation = 0.3f;
    public AudioClip grabSound;
    AudioSource audioSource;

    public bool doDebug = false;

    public enum GameState {
        menu,
        game,
        replay
    }
    public GameState _state;
    public GameState state {
        get {
            return _state;
        }
        set {
            switch( state ) {
                case GameState.menu:
                    introTitle.TransitionOut(2f);
                    break;
                case GameState.game:
                    for( int i = blocks.Count - 1; i >= 0; i-- ) {
                        TumbleBlock block = blocks[i];
                        blocks.Remove(block);
                        Destroy(block.gameObject);
                    }
                    replayScoreCounter.text = string.Format("{0}s", (Time.time - startTime).ToString("0.00"));
                    break;
                case GameState.replay:
                    replayScreen.TransitionOut(2f);
                    break; 
            }
            _state = value;
            switch( state ) {
                case GameState.menu:
                    introTitle.TransitionIn(2f);
                    break;
                case GameState.game:
                    spawnTime = Time.time + spawnMin;
                    startTime = Time.time;
                    scoreCounter.TransitionIn(1f);
                    break;
                case GameState.replay:
                    scoreCounter.TransitionOut(1f);
                    replayScreen.TransitionIn(2f);
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if( _instance != null ) { Destroy(this); }
        _instance = this;

        joint = GetComponent<ConfigurableJoint>();
        audioSource = GetComponent<AudioSource>();
        state = GameState.menu;
    }

    // Update is called once per frame
    void HandleMouse()
    {
        Plane zPlane = new Plane(-Vector3.forward, 0);
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = mainCamera.ScreenPointToRay(mousePos);

        float distance;
        zPlane.Raycast(mouseRay, out distance);
        Vector3 mouseWorldPos = mouseRay.GetPoint(distance);
        transform.position = mouseWorldPos;

        if (Input.GetMouseButtonDown(0) && grabbedBlock == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                TumbleBlock blockScript = hit.collider.gameObject.GetComponent<TumbleBlock>();
                if (blockScript != null && blockScript.isGrabbable)
                {
                    audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
                    audioSource.PlayOneShot(grabSound);
                    grabbedBlock = blockScript;
                    joint.connectedBody = grabbedBlock.rigidBody;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && grabbedBlock != null)
        {
            joint.connectedBody = null;
            grabbedBlock = null;
        }
    }

    private void Update()
    {
        switch( state ) {
            case GameState.menu:
                if( Input.anyKeyDown ) {
                    state = GameState.game;
                }
                break;
            case GameState.game:
                HandleBlockSpawning();
                HandleMouse();
                scoreCounter.GetComponentInChildren<Text>().text = string.Format("{0}s", (Time.time - startTime).ToString("0.00"));
                break;
            case GameState.replay:
                if( Input.anyKeyDown ) {
                    state = GameState.game;
                }
                break;
        }
    }
    void HandleBlockSpawning()
    {
        if (Time.time > spawnTime)
        {
            if (Random.value < 0.5f) {
                Vector3 velocity = Vector3.up * Random.Range(16f,21f);
                SpawnBlock(new Vector3(-13f, -5f), velocity, new Vector3(Random.Range(1f, 5f), Random.Range(0.5f, 2f), 1));
            } else {
                Vector3 velocity = Vector3.up * Random.Range(16f,21f);
                SpawnBlock(new Vector3(13f, -5f), velocity, new Vector3(Random.Range(1f, 5f), Random.Range(0.5f, 2f), 1));
            }

            spawnTime = Time.time + Random.Range(spawnMin, spawnMax);
        }
    }
    public void SpawnBlock( Vector3 position, Vector3 velocity, Vector3 scale )
    {
        GameObject blockObj = Instantiate<GameObject>( blockPrefab, position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 360f))) );
        TumbleBlock blockScript = blockObj.GetComponent<TumbleBlock>();
        blockScript.Init(scale);
        blockScript.rigidBody.velocity = velocity;
    }
    
}
