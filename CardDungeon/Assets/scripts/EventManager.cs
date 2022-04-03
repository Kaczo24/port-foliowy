using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject optionPrefab;

    [Header("References")]
    public Transform optionPanel;
    public TextMeshProUGUI text;
    public RawImage image;
    public DeathPanelScript deathPanel;

    public void Start()
    {
        if (DataHolder.dataHolder.currentDialog == null)
        {
            End();
            return;
        }
        if (DataHolder.dataHolder.currentEvent.image != null)
            image.texture = DataHolder.dataHolder.currentEvent.image;
        if (!DataHolder.dataHolder.dialogFlag)
            DataHolder.dataHolder.currentDialog = DataHolder.dataHolder.currentEvent.dialog;
        RefreshDialog();
    }

    void RefreshDialog()
    {
        for (int n = 0; n < optionPanel.childCount; n++)
            Destroy(optionPanel.GetChild(n).gameObject);
        text.text = Helper.Pick(DataHolder.dataHolder.currentDialog.texts);
        for(int n = 0; n < DataHolder.dataHolder.currentDialog.options.Length; n++)
        {
            GameObject o = Instantiate(optionPrefab, optionPanel);
            o.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DataHolder.dataHolder.currentDialog.options[n].text;
            o.GetComponent<Button>().onClick.AddListener(delegate () { ChoseOption(o.transform.GetSiblingIndex()); });
        }
    }

    public void ChoseOption(int index)
    {
        if(DataHolder.dataHolder.currentDialog.options[index].outcomes.Length == 0)
        {
            End();
            return;
        }
        Dialog.Outcome outcome = Helper.Pick(DataHolder.dataHolder.currentDialog.options[index].outcomes);
        DataHolder.dataHolder.stats.hp += outcome.Dhp + (int)Random.Range(-outcome.Dhp * 0.2f, outcome.Dhp * 0.2f);
        DataHolder.dataHolder.money += outcome.Dmoney + (int)Random.Range(-outcome.Dmoney * 0.2f, outcome.Dmoney * 0.2f);
        if (DataHolder.dataHolder.stats.hp > 0)
        {
            DataHolder.dataHolder.currentDialog = outcome.dialog;
            if (outcome.stdFightLoot)
            {
                DataHolder.dataHolder.dialogFlag = true;
                DataHolder.dataHolder.currentEnemy = Helper.Pick(DataHolder.dataHolder.stdBattle.enemies);
                SceneManager.LoadScene(3);
            }
            else
            {
                if (outcome.SpecialnemyFigth != null)
                {
                    DataHolder.dataHolder.dialogFlag = true;
                    DataHolder.dataHolder.currentEnemy = outcome.SpecialnemyFigth;
                    SceneManager.LoadScene(3);
                }
                RefreshDialog();
            }
        }
        else
        {
            deathPanel.StartCoroutine(deathPanel.Die(outcome.deathMessage));
        }
    }


    void End()
    {
        DataHolder.dataHolder.dialogFlag = false;
        SceneManager.LoadScene(1);
    }
}
