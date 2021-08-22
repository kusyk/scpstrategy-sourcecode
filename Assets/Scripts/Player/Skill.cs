using UnityEngine;

[CreateAssetMenu()]
public class Skill : ScriptableObject
{
    [Space(10)]
    public string fullName;
    [TextArea(3, 10)]
    public string description = "";

    //requirements
    public string rSkill = "";
    public int rScore = 0;
    public int rMoney = 0;
    public int rResearch = 0;
    public int rAction = 0;

    public bool CanBeActivated
    {
        get
        {
            if (SkillsManager.Instance.GetSkillProgress(rSkill) != 1f && rSkill != "")
            {
                Debug.Log("brak skillsa essa");
                return false;
            }

            if (PlayerController.PlayerInfo.score < rScore)
            {
                Debug.Log("brak scora");
                return false;
            }

            if (PlayerController.PlayerInfo.money < rMoney)
            {
                Debug.Log("brak hajsu kurwa");
                return false;
            }

            if (PlayerController.PlayerInfo.researchPoints < rResearch)
            {
                Debug.Log("brak punktów");
                return false;
            }

            if (PlayerController.GetAvailableActionPoints() < rAction)
            {
                Debug.Log("brak akcji");
                return false;
            }

            return true;
        }
    }
}
