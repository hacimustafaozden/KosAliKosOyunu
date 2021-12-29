using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;

    public float limitX;
    public float runningSpeed;
    public float xSpeed; //Sað sola ne kadar hýzla gideceðini tutacak
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPreFab;
    public List<RidingCylinder> cylinders;

    private bool _spawningBridge;//Bu bool deðiþkenimiz true ise þuan köprü oluþtur false ise þuan köprü oluþturma.
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;

    private bool _finished;//Finishline a gelip gelmediðini tutucak.

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
        if (LevelController.Current ==null || !LevelController.Current.GameActive) //Herhangibir level conroller yok ise veya levelcontrollerýn oyunu aktif deðil ise update in devamýný çalýþtýrma.
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0) // Telefon mu yoksa fareyle mi oynanýyor kontrol ediliyor
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) // kullanýcý parmaðýný ilk defa dokunduruyorsa.
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved) //kullanýcý parmaðýný ilk defa dokundurmuyor ve haraket ettiriyorsa.
            {
                touchXDelta = 5 * (_lastTouchedX - Input.GetTouch(0).position.x) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }

        }
        else if(Input.GetMouseButton(0)) //Bilgisayardaysa 
        {
            touchXDelta = Input.GetAxis("Mouse X"); //Mouse'un x ekseninde ne kadar hareket ettiðini atýyoruz.
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX); //Platformun dýþýna çýkmamasý için sýnýrlandýrmamýza yarýyor.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;
        if (_spawningBridge) //köprü yaratýp yaratmadýðýný kontrol ediyoruz. ve karakterimizin altýndaki silindirleri azaltýyoruz.
        {
            _creatingBridgeTimer -= Time.deltaTime;
            if(_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);
                GameObject createBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReferece.transform.position; // iki referans arasýndaki yönü vektör tipinde tuttuk
                float distance = direction.magnitude;// Ýki nokta arasýndaki mesafe. yön vektörünün Aðýrlýðýna eþitledik.
                direction = direction.normalized;
                createBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReferece.transform.position.z; //karakterimizin baþlangýç referans durumundan ne kadar uzakta olduðunu buluyoruz.
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);//Bu  çýkardýðýmýz deperi 0 ve maksimum uzaklýkla sýnýrlandýrdýk
                Vector3 newPiecePosition = _bridgeSpawner.startReferece.transform.position + direction * characterDistance;//Yarattýðýmýz objenin yeni pozisyonunu tutmasý için vector 3 tipinde deðiþken oluþturduk.
                newPiecePosition.x = transform.position.x;
                createBridgePiece.transform.position = newPiecePosition;

                if (_finished) // finish çizgisine gelmiþ ise belli süreler içerisinde skor kazan.
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

    public void ChangeSpeed(float value) //LevelControllerýn player controllerýn hýzýný deðiþtirilmesi için hýzý  deðiþtir anlamýnda public bir deðiþken oluþturduk.
    {
        _currentRunningSpeed = value; //þuanki hýzýný girilen deðere eþitledik.
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AddCylinder") // Yüklere çarpýyor mu diye kontrol ettik
        {
            IncrementCylinderVolume(0.1f); 
            Destroy(other.gameObject);
        }else if (other.tag =="SpawnBridge") // yüke deðilde çarptýðý nesnenin etiketi körü yapmaya baslama etiketi ise.
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if (other.tag == "StopSpawnBridge") // Çarptýðý nesne köprü yapmayý durdur nesnesi ise.
        {
            StopSpawningBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }else if (other.tag == "Finish") //çarptýðý obje finish ise köprü yapmaya baþla.
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
    }

    public void OnTriggerStay(Collider other) //bu fonksiyon karakter objemizin colliderinde bir triger olduðu sürece çalýþýr yani tuzakla etkileþim uzun süreli devam eder.
    {
        if (other.tag == "Trap")
        {
            IncrementCylinderVolume(-Time.fixedDeltaTime);
        }
    }
    public void IncrementCylinderVolume(float value) // silindirlerimizi küçültüp büyütebileceðimiz fonksiyon.
    { 
        if(cylinders.Count ==0)
        {
            if(value >0)
            {
                CreateCylinder(value);
            }
            else
            {
               if(_finished) // eðer karakter bitiþ çizgisine ulaþmýþ ise game over olmicak level biticek
                {
                    LevelController.Current.FinishGame();
                }
                else // finishe ulaþmadan silindirleri bitti ve - ye düþtü ise gameover fonksiyonunu çaðýrdýk
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
    public void Die() // ölme fonksiyonu
    {
        animator.SetBool("dead", true);//karakterimizin animasyonunu ölme animasyonu yaptýk.
        gameObject.layer = 8;//karakterimizin layerýna ulaþtýk.
        Camera.main.transform.SetParent(null);//kameramýza ulaþtýk ve kamera objemizi parentsiz yaptýk.
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

    public void StartSpawningBridge(BridgeSpawner spawner) //köprü yaratmamýzý baþlamaya saðlayan fonksiyon ilgili bridge sapwner parametresi oluþturduk.
    {
        _bridgeSpawner = spawner;
        _spawningBridge = true;
    }
    public void StopSpawningBridge() // köprü yapmayý durdurma fonksiyonumuz.
    {
        _spawningBridge = false;
    }
}
