using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{

    [Header("References")]
    public GameObject buttonPanel;
    public GameObject credits;
    public GameObject settings;
    public AudioMixer mixer;
    public TMP_Dropdown resDD;
    public PoolsHolder poolHolder;

    Resolution[] resolutions;
    public Character character;
    void Start()
    {
        resDD.ClearOptions();
        resDD.AddOptions(new List<Resolution>(resolutions = Screen.resolutions).ConvertAll(x => x.width + "x" + x.height));
        resDD.value = new List<Resolution>(resolutions).FindIndex(x => x.Equals(Screen.currentResolution));
    }

    public void OpenSettings()
    {
        buttonPanel.SetActive(false);
        settings.SetActive(true);
    }

    public void OpenCredits()
    {
        buttonPanel.SetActive(false);
        credits.SetActive(true);
    }

    void Update()
    {
        if(credits.activeSelf)
            if(Input.GetMouseButtonDown(1))
            {
                buttonPanel.SetActive(true);
                credits.SetActive(false);
            }
    }

    public void Focus()
    {
        DataHolder.dataHolder.stats.HPmax = DataHolder.dataHolder.stats.hp = character.maxHp;
        character.slots.ForEach(x => DataHolder.dataHolder.slots.Add(x));
        character.artifacts.ForEach(x => DataHolder.dataHolder.artifacts.Add(x.Clone()));
        character.cards.ForEach(x => DataHolder.dataHolder.stats.cards.Add(x.Clone()));
        DataHolder.dataHolder.stats.cardDraw = character.cardDraw;
        DataHolder.dataHolder.characterManaMax = character.manaMax;

        DataHolder.dataHolder.locations[0] = (Location)System.Enum.GetValues(typeof(Location)).GetValue(Random.Range(0, 6));
        if (DataHolder.dataHolder.locations[0] == Location.AshPlains || DataHolder.dataHolder.locations[0] == Location.Hell)
            DataHolder.dataHolder.locations[1] = (Location)System.Enum.GetValues(typeof(Location)).GetValue(Random.Range(2, 6));
        if (DataHolder.dataHolder.locations[0] == Location.Taiga || DataHolder.dataHolder.locations[0] == Location.IceBergs)
            DataHolder.dataHolder.locations[1] = (Location)System.Enum.GetValues(typeof(Location)).GetValue(Random.Range(0, 2) * 4 + Random.Range(0, 2));
        if (DataHolder.dataHolder.locations[0] == Location.Swamp || DataHolder.dataHolder.locations[0] == Location.DarkForest)
            DataHolder.dataHolder.locations[1] = (Location)System.Enum.GetValues(typeof(Location)).GetValue(Random.Range(0, 4));


        DataHolder.dataHolder.rooms = Room.GenerateRooms(30, poolHolder.getPool[DataHolder.dataHolder.locations[0]].events.ToArray());
        DataHolder.dataHolder.currentRoom = DataHolder.dataHolder.rooms[0];
        DataHolder.dataHolder.bossRoom = DataHolder.dataHolder.rooms[DataHolder.dataHolder.rooms.Length - 1];

        SceneManager.LoadScene(1);
    }

    public void CloseSettings()
    {
        buttonPanel.SetActive(true);
        settings.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }


    #region Settings

    public void SetResolution()
    {
        Screen.SetResolution(resolutions[resDD.value].width, resolutions[resDD.value].height, Screen.fullScreen);
    }


    public void SetVolumeMaster(Slider volume)
    {
        mixer.SetFloat("masterVolume", volume.value);
    }
    public void SetVolumeMusic(Slider volume)
    {
        mixer.SetFloat("musicVolume", volume.value);
    }
    public void SetVolumeEffects(Slider volume)
    {
        mixer.SetFloat("effectsVolume", volume.value);
    }

    public void SetFullscreen(bool fs)
    {
        Screen.fullScreen = fs;
    }
    #endregion
}
