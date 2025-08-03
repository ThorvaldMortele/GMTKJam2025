using System.Collections;
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
        //Set all speechbubble inactive
        speechbubbleCPUWon.SetActive(false);
        speechbubbleCPULost.SetActive(false);
        speechbubbleCPUComment.SetActive(false);

        voiceLinesPreset = Random.Range(0, 2);
    }

    private void PlayVoiceLine(EventReference fmodEvent)
    {
        var instance = RuntimeManager.CreateInstance(fmodEvent.Guid);
        instance.setParameterByName("parameter:/VoiceOverPreset", voiceLinesPreset);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }

    public IEnumerator PlayCPUWonVoiceLine()
    {
        speechbubbleCPUWon.SetActive(true);
        //Twean pop in speechbubble
        PlayVoiceLine(cpuWonVoiceLine);

        yield return new WaitForSeconds(4.5f);
        //Twean pop out speechbubble
        speechbubbleCPUWon.SetActive(false);
    }

    public IEnumerator PlayCPULostVoiceLine()
    {
        speechbubbleCPULost.SetActive(true);
        //Twean pop in speechbubble
        PlayVoiceLine(cpuLostvoiceLine);

        yield return new WaitForSeconds(4);
        //Twean pop out speechbubble
        speechbubbleCPULost.SetActive(false);
    }

    public IEnumerator PlayCPUCommentVoiceLine()
    {
        speechbubbleCPUComment.SetActive(true);
        //Twean pop in speechbubble
        PlayVoiceLine(cpuCommentsVoiceLine);

        yield return new WaitForSeconds(4);

        //Twean pop out speechbubble
        speechbubbleCPUComment.SetActive(false);
    }
}
