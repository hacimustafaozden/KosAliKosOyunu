using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled;
    private float _value;

    public void IncrementCylinderVolume(float value)
    {
        _value += value;
        if(_value > 1) 
        {
            float leftValue = _value - 1;
            int cylinderCount = PlayerController.Current.cylinders.Count; // Silindir sayisi diye bir deðiþken oluþturduk ve yine karakterimizin playercontrolerýna eriþiyoruz ve silindirlerin sayisini çekiyoruz.
            
            //SÝLÝNDÝR EN BÜYÜK HALÝNE ULAÞTIÐINDA OLUSUCAK OLAN BOYUT DEÐÝÞÝMÝ
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f, transform.localPosition.z);//silindirimiz x ve z ekseninde büyüme yaþamayacaðý için sadece y ekseninde nasýl büyüyeceðinin hesabýný yaptýk.
            transform.localScale = new Vector3(0.5f ,transform.localScale.y ,0.5f) ; //silindirimizin local pozisyonuna eriþiyoruz ve x ve z deðerleri zaten sabit olduðundan deðerlerini yazýyoruz y deðerini ise bi üst satýrdaki aldýðýmýz formüldeki deðeri çekiyoruz.
            PlayerController.Current.CreateCylinder(leftValue);
        }
        else if(_value <0)
        {
            PlayerController.Current.DestroyCylinder(this); // Silindiri yok etmesi için this yani kendisini parametre olarak verdik.
        }
        else
        {
            int cylinderCount = PlayerController.Current.cylinders.Count; // Silindir sayisi diye bir deðiþken oluþturduk ve yine karakterimizin playercontrolerýna eriþiyoruz ve silindirlerin sayisini çekiyoruz.

            //SÝLÝNDÝR EN BÜYÜK HALÝNE ULAÞMADIÐI OLUSUCAK OLAN BOYUT DEÐÝÞÝMLERÝ
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f * _value, transform.localPosition.z); // silindirimizin sisme deðeri yani valuesiyle çarparak max boyutuna ulaþmamýþ ise o anki boyutunu ayarlýyoruz silindirimizin.
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);
        }
    }
}
