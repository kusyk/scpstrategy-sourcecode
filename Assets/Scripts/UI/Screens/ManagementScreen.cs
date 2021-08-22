using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManagementScreen : MonoBehaviour
{
    [Header("Income")]
    public GameObject incomeAlert;
    public Slider incomeSlider;
    public TextMeshProUGUI incomeMaxTMP;
    public TextMeshProUGUI incomeSliderText;
    public RectTransform incomeCountriesContent;
    public GameObject countryPrefab;

    [Header("Outcome")]
    public GameObject outcomeAlert;
    public Slider unitsSlider;
    public TextMeshProUGUI unitsSliderText;
    public Slider scientistsSlider;
    public TextMeshProUGUI scientistsSliderText;

    [Header("Progress")]
    public Image progressBar;

    [Header("Statistics")]
    public TextMeshProUGUI statsTMP;

    [Header("Previous Sliders' Values")]
    public RectTransform incomeSlidingArea;
    public RectTransform incomePreviousPointer;
    public RectTransform unitsSlidingArea;
    public RectTransform unitsPreviousPointer;
    public RectTransform scientistsSlidingArea;
    public RectTransform scientistsPreviousPointer;

    public void SetupManagementScreen()
    {
        for (int i = incomeCountriesContent.childCount - 1; i >= 0; i--)
        {
            Destroy(incomeCountriesContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < GDPReader.GDPList.Count; i++)
        {
            Instantiate(countryPrefab, incomeCountriesContent.transform).GetComponent<CountryInfo>().SetupCountryPanel(GDPReader.GDPList[i]);
        }
    }

    public void LoadManagementScreenValues()
    {
        UpdateCountries();
        LoadSlidersValues();
    }

    public void SaveNewSliderValues()
    {
        if (gameObject.activeSelf == false)
            return;

        PlayerController.PlayerInfo.contribution = (int)incomeSlider.value;
        PlayerController.PlayerInfo.outcomeUnits = FormatChanger.Round(unitsSlider.value, 2);
        PlayerController.PlayerInfo.outcomeScientists = FormatChanger.Round(scientistsSlider.value, 2);
    }

    void LateUpdate()
    {
        UpdateSlidersAndAlerts();
        UpdateStats();
    }

    private void UpdateSlidersAndAlerts()
    {
        string tempString = incomeSlider.value.ToString();

        incomeSliderText.text = tempString;

        tempString = FormatChanger.Round(unitsSlider.value, 2).ToString();
        unitsSliderText.text = tempString;

        tempString = FormatChanger.Round(scientistsSlider.value, 2).ToString();
        scientistsSliderText.text = tempString;

        if (incomeSlider.value >= 0.8f * PlayerController.GetMaxContribution())
        {
            if (incomeAlert.activeSelf == false)
            {
                incomeAlert.SetActive(true);
            }
        }
        else if (incomeAlert.activeSelf == true)
        {
            incomeAlert.SetActive(false);
        }

        int payouts = (int)(FormatChanger.Round(unitsSlider.value, 2) * PlayerController.GetUnitsEmplCount()
            + FormatChanger.Round(scientistsSlider.value, 2) * PlayerController.GetBrnahcesEmplCount());

        if (payouts > PlayerController.NextMonthIncome)
        {
            if (outcomeAlert.activeSelf == false)
            {
                outcomeAlert.SetActive(true);
            }
        }
        else if (outcomeAlert.activeSelf == true)
        {
            outcomeAlert.SetActive(false);
        }
    }

    private void UpdateCountries()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        float tempSlider = (incomeSlider.value - 1) / 19f;
        float tempIncome = 700f * (1 - PlayerController.PlayerInfo.satisfactionLevel) + 15000;

        for (int i = 0; i < incomeCountriesContent.childCount; i++)
        {
            incomeCountriesContent.GetChild(i).gameObject.GetComponent<CountryInfo>().UpdateColors(tempIncome);
        }

        stopwatch.Stop();
        Debug.Log(string.Format("{0} elements repainted in {1} ms", incomeCountriesContent.childCount, stopwatch.Elapsed.Milliseconds));
    }

    private void UpdateStats()
    {
        float playTime = PlayerController.PlayerInfo.playTime;
        int hours = (int)(playTime / 60);
        int minutes = ((int)playTime) % 60;
        int seconds = (int)((playTime % 1) * 60);

        int income = PlayerController.NextMonthIncome;
        int payouts = (int)(FormatChanger.Round(unitsSlider.value, 2) * PlayerController.GetUnitsEmplCount()
            + FormatChanger.Round(scientistsSlider.value, 2) * PlayerController.GetBrnahcesEmplCount());

        statsTMP.text = "<b>Organization</b>\n";
        statsTMP.text += string.Format("<b>{0}</b> - owner\n", PlayerController.PlayerInfo.name);
        statsTMP.text += string.Format("<b>{0}</b> - branches\n", PlayerController.PlayerInfo.branches.Count);
        statsTMP.text += string.Format("<b>{0}</b> - units\n", PlayerController.PlayerInfo.units.Count);
        statsTMP.text += string.Format("<b>{0}%</b> - governments' satisfaction\n", (int)(PlayerController.PlayerInfo.satisfactionLevel * 100));
        statsTMP.text += string.Format("<b>{0}</b> - months played\n", PlayerController.PlayerInfo.monthsPlayed);

        statsTMP.text += "\n<b>Money</b>\n";
        statsTMP.text += string.Format("<b>{0}</b> - next month income\n", FormatChanger.HugeNumerToString(income));
        statsTMP.text += string.Format("<b>{0}</b> - next month expenses\n", FormatChanger.HugeNumerToString(payouts));
        statsTMP.text += string.Format("<b>{0}</b> - money earned\n", FormatChanger.HugeNumerToString(PlayerController.PlayerInfo.earnedMoney));
        statsTMP.text += string.Format("<b>{0}</b> - money spent\n", FormatChanger.HugeNumerToString(PlayerController.PlayerInfo.spentMoney));

        statsTMP.text += "\n<b>Objects</b>\n";
        statsTMP.text += string.Format("<b>{0}</b> - checked cases\n", PlayerController.PlayerInfo.checkedCases);
        statsTMP.text += string.Format("<b>{0}</b> - captured objects\n", PlayerController.PlayerInfo.capturedObjects);
        statsTMP.text += string.Format("<b>{0}</b> - conducted research\n", PlayerController.PlayerInfo.conductedResearch);

        statsTMP.text += "\n<b>Other</b>\n";
        statsTMP.text += string.Format("<b>{0}:{1}:{2}</b> - game time\n", 
            FormatChanger.TimeToString(hours), FormatChanger.TimeToString(minutes), FormatChanger.TimeToString(seconds));
        statsTMP.text += string.Format("<size=18>#{0}</size>\n", PlayerController.PlayerInfo.gamesaveid);

        //progress bar
        progressBar.fillAmount = Mathf.Clamp01(PlayerController.PlayerInfo.score / 10000f);
    }

    private void LoadSlidersValues()
    {
        incomeSlider.maxValue = PlayerController.GetMaxContribution();
        incomeMaxTMP.text = incomeSlider.maxValue.ToString();

        incomeSlider.value = PlayerController.PlayerInfo.contribution;
        unitsSlider.value = PlayerController.PlayerInfo.outcomeUnits;
        scientistsSlider.value = PlayerController.PlayerInfo.outcomeScientists;

        //calculating old pointers positions
        incomePreviousPointer.anchoredPosition = new Vector2(incomeSlidingArea.offsetMin.x + incomeSlidingArea.rect.width * ((PlayerController.PlayerInfo.oldContibution - 1f) / (PlayerController.GetMaxContribution() -1f)), 0);
        unitsPreviousPointer.anchoredPosition = new Vector2(unitsSlidingArea.offsetMin.x + unitsSlidingArea.rect.width * ((PlayerController.PlayerInfo.oldOutcomeUnits - 1f) / 1f), 0);
        scientistsPreviousPointer.anchoredPosition = new Vector2(scientistsSlidingArea.offsetMin.x + scientistsSlidingArea.rect.width * ((PlayerController.PlayerInfo.oldOutcomeScientists - 1f) / 1f), 0);
    }
}
