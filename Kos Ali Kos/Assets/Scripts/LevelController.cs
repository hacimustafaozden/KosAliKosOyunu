using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current; // di�er s�n�flar�n bu objeye eri�mesi i�in current ad�nda static olu�turduk.
    public bool GameActive = false;

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu; // levelcontroller�m�z�n aray�zlere eri�mesi i�in gerekli de�i�ken tan�mlamalar�n� yapt�k.
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText;//yaz� objelerimizide tuttuk.
    public Slider levelProgressBar; //slider�m�z� bu de�i�kende tutuyoruz.
    public float maxDistance;//karakterimizin biti� �izgisine olan uzakl��� tutuyoruz.
    public GameObject finishLine;//finish �izgimizi tutucak de�i�ken.

    int currentLevel;
    int score; // skorlar�m�z� tutan de�i�ken.
    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel"); // Oyuncunun en son hangi levelde kaldi��n� bir de�i�kene atad�k.
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel) //hangi levelde oldu�unu �ekiyoruz.
        {
            SceneManager.LoadScene("Level " + currentLevel); //Sahne y�kleme
        }
        else
        {
            currentLevelText.text = (currentLevel + 1).ToString(); //ilk leveldeysek bunun yaz�s�n� bire e�itledik 0. levelde ba�lad���m�z i�in 1 olmasa� i�in 1 artt�rd�k.
            nextLevelText.text = (currentLevel + 2).ToString(); // next level textimiz i�inde ayn�s�n� yap�yoruz fakat bu sefer bir sonrakini g�stermesi i�in 2 artt�r�yoruz.
        }
    }

    void Update()
    {
        if (GameActive) //Oyun aktif oldu�u s�rece slider � yani ilerleme bar�n� doldurma.
        {
            PlayerController player = PlayerController.Current; // Player kontrolumuzu tutmas� i�in bir de�i�ken olu�turdul ve bunu player kontrolumuzun �uanki durumuna e�itledik.
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z; // karakterimin �izgiye ne kadar uzak oldu�unu hesaplad�k.
            levelProgressBar.value = 1 - (distance / maxDistance);//sliderimizin ne kadar dolup dolmad���n� 0 la 1 aras� bir de�ere e�itledik. karakterimiz biti� �izgisine ne kadar yak�nsa max 1 vericek minumum 0 vericek.

        } 
    }

    public void StartLevel() // bu fonksiyon gerekli d�zenlemeleri yapt�ktan sonra en son game active i true yap�cak.
    {
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z; //maxDistance yi yani karakterimizin biti� �izgisine olan uzakl���n� hesaplamak i�in finish �izgisinin konumundan ba�lang�� noktas�n� ��kartt�k.
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed); //  playercontroller s�n�f�n�n �uanki objesine eri�tik ve h�z�n� yine ayn� objenin maximum h�z� kadar artt�r.
        startMenu.SetActive(false);// startmenuyu deaktifi hale getirdik.
        gameMenu.SetActive(true);// game menuyu aktif hale getirdik.
        PlayerController.Current.animator.SetBool("running", true);//�uanki player kontrol�m�ze eri� animat�r�ne gel ve running true oldu�unda oyunu ba�lat.
        GameActive = true;

    }
    public void RestartLevel()//karakterimiz yand���nda tekrardan ba�lamam�z� sa�layan fonksiyon.
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //�uanki sahneyi y�kleme.
    }
    public void LoadNextLevel() // sonraki leveli y�klemek i�in bu fonksiyonu kullan�yoruz.
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1)); //�uanki sahneden bir sonraki sahneye yani levele ge�isini sa�l�yoruz.
    }
    public void GameOver()// Bu fonksiyon oyunumuzun bitmesini sa�l�yor.
    {
        gameMenu.SetActive(false);//Oyun men�m�z�  kapatt�k.
        gameOverMenu.SetActive(true);//game over men�m�z� true yapt�k ��nk� yand�k ve bu men� a��lacak art�k.
        GameActive = false;//oyunda yand���m�z i�in art�k oyun aktif de�il bu y�zden gameactive i de false yapt�k.
    }
    public void FinishGame() // oyunu bitirdi�imizde �al�sacak fonksiyon.
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);//unutynin haf�za biriminde hangi levelde kald���m�z� kaydettik.
        finishScoreText.text = score.ToString();//biti� b�l�m�m�z�n skor tutan yaz�s�na eri�tik ve yaz�s�n�da scorun stringe d�n��t�r�lm�� haline e�itledik.
        gameMenu.SetActive(false); //oyun men�s�n� kapatt�k
        finishMenu.SetActive(true); //finish men�s�n� a�t�k
        GameActive = false;// oyun aktif olmad��� i�in game aktifi false yapt�k
    }

    public void ChangeScore(int increment)//oyuncu skor kazand��� zaman ekrandaki skorun de�i�mesi fonksiyonu.
    {
        score += increment;
        scoreText.text = score.ToString();
    }
}
