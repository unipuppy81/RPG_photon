using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textHp;

    [SerializeField]
    private float textUpSpeed;
    [SerializeField]
    private float alphaSpeed;
    [SerializeField]
    private float destroyTime;

    private void Start()
    {
        Debug.Log("DamageText Start");

        textHp = GetComponent<TextMeshPro>();

        Invoke("DestroyDamageText", destroyTime);
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, textUpSpeed * Time.deltaTime, 0));
        textHp.color = Color.red;
    }

    public void SetDamage(float _damage)
    {
        float d = _damage;
        textHp.text = d.ToString();

        Debug.Log("Set Damage : " + d);

    }

    private void DestroyDamageText()
    {
        Destroy(gameObject);
    }
}
