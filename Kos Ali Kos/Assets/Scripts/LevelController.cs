using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current; // diðer sýnýflarýn bu objeye eriþmesi için current adýnda static oluþturduk.
    public bool GameActive = false;

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu; // levelcontrollerýmýzýn arayüzlere eriþmesi için gerekli deðiþken tanýmlamalarýný yaptýk.
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText;//yazý objelerimizide tuttuk.
    public Slider levelProgressBar; //sliderýmýzý bu deðiþkende tutuyoruz.
    public float maxDistance;//karakterimizin bitiþ çizgisine olan uzaklýðý tutuyoruz.
    public GameObject finishLine;//finish çizgimizi tutucak deðiþken.

    int currentLevel;
    int score; // skorlarýmýzý tutan deðiþken.
    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel"); // Oyuncunun en son hangi levelde kaldiðýný bir deðiþkene atadýk.
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel) //hangi levelde olduðunu çekiyoruz.
        {
            SceneManager.LoadScene("Level " + currentLevel); //Sahne yükleme
        }
        else
        {
            currentLevelText.text = (currentLevel + 1).ToString(); //ilk leveldeysek bunun yazýsýný bire eþitledik 0. levelde baþladýðýmýz için 1 olmasaý için 1 arttýrdýk.
            nextLevelText.text = (currentLevel + 2).ToString(); // next level textimiz içinde aynýsýný yapýyoruz fakat bu sefer bir sonrakini göstermesi için 2 arttýrýyoruz.
        }
    }

    void Update()
    {
        if (GameActive) //Oyun aktif olduðu sürece slider ý yani ilerleme barýný doldurma.
        {
            PlayerController player = PlayerController.Current; // Player kontrolumuzu tutmasý için bir deðiþken oluþturdul ve bunu player kontrolumuzun þuanki durumuna eþitledik.
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z; // karakterimin çizgiye ne kadar uzak olduðunu hesapladýk.
            levelProgressBar.value = 1 - (distance / maxDistance);//sliderimizin ne kadar dolup dolmadýðýný 0 la 1 arasý bir deðere eþitledik. karakterimiz bitiþ çizgisine ne kadar yakýnsa max 1 vericek minumum 0 vericek.

        } 
    }

    public void StartLevel() // bu fonksiyon gerekli düzenlemeleri yaptýktan sonra en son game active i true yapýcak.
    {
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z; //maxDistance yi yani karakterimizin bitiþ çizgisine olan uzaklýðýný hesaplamak için finish çizgisinin konumundan baþlangýç noktasýný çýkarttýk.
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed); //  playercontroller sýnýfýnýn þuanki objesine eriþtik ve hýzýný yine ayný objenin maximum hýzý kadar arttýr.
        startMenu.SetActive(false);// startmenuyu deaktifi hale getirdik.
        gameMenu.SetActive(true);// game menuyu aktif hale getirdik.
        PlayerController.Current.animator.SetBool("running", true);//þuanki player kontrolümüze eriþ animatörüne gel ve running true olduðunda oyunu baþlat.
        GameActive = true;

    }
    public void RestartLevel()//karakterimiz yandýðýnda tekrardan baþlamamýzý saðlayan fonksiyon.
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //þuanki sahneyi yükleme.
    }
    public void LoadNextLevel() // sonraki leveli yüklemek için bu fonksiyonu kullanýyoruz.
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1)); //þuanki sahneden bir sonraki sahneye yani levele geçisini saðlýyoruz.
    }
    public void GameOver()// Bu fonksiyon oyunumuzun bitmesini saðlýyor.
    {
        gameMenu.SetActive(false);//Oyun menümüzü  kapattýk.
        gameOverMenu.SetActive(true);//game over menümüzü true yaptýk çünkü yandýk ve bu menü açýlacak artýk.
        GameActive = false;//oyunda yandýðýmýz için artýk oyun aktif deðil bu yüzden gameactive i de false yaptýk.
    }
    public void FinishGame() // oyunu bitirdiðimizde çalýsacak fonksiyon.
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);//unutynin hafýza biriminde hangi levelde kaldýðýmýzý kaydettik.
        finishScoreText.text = score.ToString();//bitiþ bölümümüzün skor tutan yazýsýna eriþtik ve yazýsýnýda scorun stringe dönüþtürülmüþ haline eþitledik.
        gameMenu.SetActive(false); //oyun menüsünü kapattýk
        finishMenu.SetActive(true); //finish menüsünü açtýk
        GameActive = false;// oyun aktif olmadýðý için game aktifi false yaptýk
    }

    public void ChangeScore(int increment)//oyuncu skor kazandýðý zaman ekrandaki skorun deðiþmesi fonksiyonu.
    {
        score += increment;
        scoreText.text = score.ToString();
    }
}
