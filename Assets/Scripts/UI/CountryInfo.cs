using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountryInfo : MonoBehaviour
{
    public int gdp = -1;
    public Image background;
    public Image flagImage;
    public TextMeshProUGUI nameText;

    [Space]
    public Color positiveColor;
    public Color negativeColor;
    

    public void SetupCountryPanel(GDPReader.GDPInfo tempGDPInfo)
    {

        if (tempGDPInfo == null)
            return;

        gdp = tempGDPInfo.gdp;

        UpdateColors();

        Sprite tempImage = Resources.Load<Sprite>("Flags/" + tempGDPInfo.name);

        if(tempImage == null)
        {
            gameObject.SetActive(false);
            return;
        }

        flagImage.sprite = tempImage;
        nameText.text = tempGDPInfo.name;

    }

    public void UpdateColors(float income = 0)
    {
        //if (gdp > income)
        //    background.color = positiveColor;
        //else
        //    background.color = negativeColor;
        float tempValue = (gdp * 0.005f + 15300f) / income;
        tempValue = Mathf.Clamp01(tempValue) * 100;
        background.color = Color.Lerp(positiveColor, negativeColor, 100f-tempValue);
    }
}
