using FMODUnity;
using UnityEngine;

public class CPUVoiceLineManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject speechbubbleCPUWon;
    public GameObject speechbubbleCPULost;
    public GameObject speechbubbleCPUComment;

    [Header("Audio")]
    public EventReference cpuWonVoiceLine;
    public EventReference cpuLostvoiceLine;
    public EventReference cpuCommentsVoiceLine;

    public int voiceLinesPreset = -1;

    private void Awake()
    {
        if (voiceLinesPreset == 1) //Was Thorvald
        {
            voiceLinesPreset = 0; //Set to Vincent
        }
        else if (voiceLinesPreset == 0) //Was Vincent
        {
            voiceLinesPreset = 1; //Set to Thorvald
        }
        else if (voiceLinesPreset == -1) //First time
        {
            voiceLinesPreset = 1; //Set to Thorvald
        }
    }

    private void PlayVoiceLine(EventReference fmodEvent)
    {
        var instance = RuntimeManager.CreateInstance(fmodEvent.Guid);
        instance.setParameterByName("parameter:/VoiceOverPreset", voiceLinesPreset);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }

    public void PlayCPUWonVoiceLine()
    {
        speechbubbleCPUWon.SetActive(true);
        //Twean pop in speechbubble
        PlayVoiceLine(cpuWonVoiceLine);
        //Wait for 4.5seconds
        //Twean pop out speechbubble
        //speechbubbleCPUWon.SetActive(false);
    }

    public void PlayCPULostVoiceLine()
    {
        speechbubbleCPULost.SetActive(true);
        //Twean pop in speechbubble
        PlayVoiceLine(cpuLostvoiceLine);
        //Wait for 4seconds
        //Twean pop out speechbubble
        //speechbubbleCPULost.SetActive(false);
    }

    public void PlayCPUCommentVoiceLine()
    {
        speechbubbleCPUComment.SetActive(true);
        //Twean pop in speechbubble
        PlayVoiceLine(cpuCommentsVoiceLine);
        //Wait for 4 seconds
        //Twean pop out speechbubble
        //speechbubbleCPUComment.SetActive(false);
    }
}
