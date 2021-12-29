using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject startReferece, endReference; //ba�lang�� ve biti� referanslar�n� tutmas� i�in.
    public BoxCollider hiddenPlatform;// box colliderimizi  konumland�rmak ve boyutlad�rmak i�in bir de�i�ken tan�mlad�k.
    
    void Start()
    { //coolider � iki referans aras�na getirdik be ikisi aras�nda boyutland�rd�k.
        Vector3 direction = endReference.transform.position - startReferece.transform.position; // iki referans aras�ndaki y�n� vekt�r tipinde tuttuk
        float distance = direction.magnitude;// �ki nokta aras�ndaki mesafe. y�n vekt�r�n�n A��rl���na e�itledik.
        direction = direction.normalized;
        hiddenPlatform.transform.forward = direction;
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);
        hiddenPlatform.transform.position = startReferece.transform.position + (direction * distance / 2) + ( new Vector3(0 ,-direction.z , direction.y) * hiddenPlatform.size.y / 2);
    }
}
