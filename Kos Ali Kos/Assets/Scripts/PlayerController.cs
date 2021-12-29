using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;

    public float limitX;
    public float runningSpeed;
    public float xSpeed; //Sa� sola ne kadar h�zla gidece�ini tutacak
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPreFab;
    public List<RidingCylinder> cylinders;

    private bool _spawningBridge;//Bu bool de�i�kenimiz true ise �uan k�pr� olu�tur false ise �uan k�pr� olu�turma.
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;

    private bool _finished;//Finishline a gelip gelmedi�ini tutucak.

    private float _scoreTimer = 0;

    public Animator animator;

    private float _lastTouchedX;

    void Start()
    {
        Current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelController.Current ==null || !LevelController.Current.GameActive) //Herhangibir level conroller yok ise veya levelcontroller�n oyunu aktif de�il ise update in devam�n� �al��t�rma.
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0) // Telefon mu yoksa fareyle mi oynan�yor kontrol ediliyor
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) // kullan�c� parma��n� ilk defa dokunduruyorsa.
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved) //kullan�c� parma��n� ilk defa dokundurmuyor ve haraket ettiriyorsa.
            {
                touchXDelta = 5 * (_lastTouchedX - Input.GetTouch(0).position.x) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }

        }
        else if(Input.GetMouseButton(0)) //Bilgisayardaysa 
        {
            touchXDelta = Input.GetAxis("Mouse X"); //Mouse'un x ekseninde ne kadar hareket etti�ini at�yoruz.
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX); //Platformun d���na ��kmamas� i�in s�n�rland�rmam�za yar�yor.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;
        if (_spawningBridge) //k�pr� yarat�p yaratmad���n� kontrol ediyoruz. ve karakterimizin alt�ndaki silindirleri azalt�yoruz.
        {
            _creatingBridgeTimer -= Time.deltaTime;
            if(_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);
                GameObject createBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReferece.transform.position; // iki referans aras�ndaki y�n� vekt�r tipinde tuttuk
                float distance = direction.magnitude;// �ki nokta aras�ndaki mesafe. y�n vekt�r�n�n A��rl���na e�itledik.
                direction = direction.normalized;
                createBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReferece.transform.position.z; //karakterimizin ba�lang�� referans durumundan ne kadar uzakta oldu�unu buluyoruz.
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);//Bu  ��kard���m�z deperi 0 ve maksimum uzakl�kla s�n�rland�rd�k
                Vector3 newPiecePosition = _bridgeSpawner.startReferece.transform.position + direction * characterDistance;//Yaratt���m�z objenin yeni pozisyonunu tutmas� i�in vector 3 tipinde de�i�ken olu�turduk.
                newPiecePosition.x = transform.position.x;
                createBridgePiece.transform.position = newPiecePosition;

                if (_finished) // finish �izgisine gelmi� ise belli s�reler i�erisinde skor kazan.
                {
                    _scoreTimer -= Time.deltaTime;
                    if(_scoreTimer < 0)
                    {
                        _scoreTimer = 0.0001f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }
    }

    public void ChangeSpeed(float value) //LevelController�n player controller�n h�z�n� de�i�tirilmesi i�in h�z�  de�i�tir anlam�nda public bir de�i�ken olu�turduk.
    {
        _currentRunningSpeed = value; //�uanki h�z�n� girilen de�ere e�itledik.
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AddCylinder") // Y�klere �arp�yor mu diye kontrol ettik
        {
            IncrementCylinderVolume(0.1f); 
            Destroy(other.gameObject);
        }else if (other.tag =="SpawnBridge") // y�ke de�ilde �arpt��� nesnenin etiketi k�r� yapmaya baslama etiketi ise.
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if (other.tag == "StopSpawnBridge") // �arpt��� nesne k�pr� yapmay� durdur nesnesi ise.
        {
            StopSpawningBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }else if (other.tag == "Finish") //�arpt��� obje finish ise k�pr� yapmaya ba�la.
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
    }

    public void OnTriggerStay(Collider other) //bu fonksiyon karakter objemizin colliderinde bir triger oldu�u s�rece �al���r yani tuzakla etkile�im uzun s�reli devam eder.
    {
        if (other.tag == "Trap")
        {
            IncrementCylinderVolume(-Time.fixedDeltaTime);
        }
    }
    public void IncrementCylinderVolume(float value) // silindirlerimizi k���lt�p b�y�tebilece�imiz fonksiyon.
    { 
        if(cylinders.Count ==0)
        {
            if(value >0)
            {
                CreateCylinder(value);
            }
            else
            {
               if(_finished) // e�er karakter biti� �izgisine ula�m�� ise game over olmicak level biticek
                {
                    LevelController.Current.FinishGame();
                }
                else // finishe ula�madan silindirleri bitti ve - ye d��t� ise gameover fonksiyonunu �a��rd�k
                {
                    Die();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value);
        }
    }
    public void Die() // �lme fonksiyonu
    {
        animator.SetBool("dead", true);//karakterimizin animasyonunu �lme animasyonu yapt�k.
        gameObject.layer = 8;//karakterimizin layer�na ula�t�k.
        Camera.main.transform.SetParent(null);//kameram�za ula�t�k ve kamera objemizi parentsiz yapt�k.
        LevelController.Current.GameOver();
    } 
    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPreFab,transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);
    }
    public void DestroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }

    public void StartSpawningBridge(BridgeSpawner spawner) //k�pr� yaratmam�z� ba�lamaya sa�layan fonksiyon ilgili bridge sapwner parametresi olu�turduk.
    {
        _bridgeSpawner = spawner;
        _spawningBridge = true;
    }
    public void StopSpawningBridge() // k�pr� yapmay� durdurma fonksiyonumuz.
    {
        _spawningBridge = false;
    }
}
