using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject startReferece, endReference; //baþlangýç ve bitiþ referanslarýný tutmasý için.
    public BoxCollider hiddenPlatform;// box colliderimizi  konumlandýrmak ve boyutladýrmak için bir deðiþken tanýmladýk.
    
    void Start()
    { //coolider ý iki referans arasýna getirdik be ikisi arasýnda boyutlandýrdýk.
        Vector3 direction = endReference.transform.position - startReferece.transform.position; // iki referans arasýndaki yönü vektör tipinde tuttuk
        float distance = direction.magnitude;// Ýki nokta arasýndaki mesafe. yön vektörünün Aðýrlýðýna eþitledik.
        direction = direction.normalized;
        hiddenPlatform.transform.forward = direction;
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);
        hiddenPlatform.transform.position = startReferece.transform.position + (direction * distance / 2) + ( new Vector3(0 ,-direction.z , direction.y) * hiddenPlatform.size.y / 2);
    }
}
