using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] Image healthBarImg;

    public bool isUpdating { get; private set; }

    private void Awake()
    {

    }
    private void Update()
    {
        float val = health.transform.localScale.x;

        if(val <= .25f)
            healthBarImg.color = Color.red;
        else if (val <= .5f)
            healthBarImg.color = Color.yellow;
        else
            healthBarImg.color = Color.green;
    }
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        isUpdating = true;

        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f);

        isUpdating = false;
    }
}
