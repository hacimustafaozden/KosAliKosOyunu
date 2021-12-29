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
            int cylinderCount = PlayerController.Current.cylinders.Count; // Silindir sayisi diye bir de�i�ken olu�turduk ve yine karakterimizin playercontroler�na eri�iyoruz ve silindirlerin sayisini �ekiyoruz.
            
            //S�L�ND�R EN B�Y�K HAL�NE ULA�TI�INDA OLUSUCAK OLAN BOYUT DE����M�
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f, transform.localPosition.z);//silindirimiz x ve z ekseninde b�y�me ya�amayaca�� i�in sadece y ekseninde nas�l b�y�yece�inin hesab�n� yapt�k.
            transform.localScale = new Vector3(0.5f ,transform.localScale.y ,0.5f) ; //silindirimizin local pozisyonuna eri�iyoruz ve x ve z de�erleri zaten sabit oldu�undan de�erlerini yaz�yoruz y de�erini ise bi �st sat�rdaki ald���m�z form�ldeki de�eri �ekiyoruz.
            PlayerController.Current.CreateCylinder(leftValue);
        }
        else if(_value <0)
        {
            PlayerController.Current.DestroyCylinder(this); // Silindiri yok etmesi i�in this yani kendisini parametre olarak verdik.
        }
        else
        {
            int cylinderCount = PlayerController.Current.cylinders.Count; // Silindir sayisi diye bir de�i�ken olu�turduk ve yine karakterimizin playercontroler�na eri�iyoruz ve silindirlerin sayisini �ekiyoruz.

            //S�L�ND�R EN B�Y�K HAL�NE ULA�MADI�I OLUSUCAK OLAN BOYUT DE����MLER�
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f * _value, transform.localPosition.z); // silindirimizin sisme de�eri yani valuesiyle �arparak max boyutuna ula�mam�� ise o anki boyutunu ayarl�yoruz silindirimizin.
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);
        }
    }
}
