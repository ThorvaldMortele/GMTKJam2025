using FMODUnity;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public EventReference hoverSFX;
    public EventReference pressedSFX;
    public EventReference uiPopUpSFX;
    public EventReference explodeLoop;

    public void UIHoverSFX()
    {
        var instance = RuntimeManager.CreateInstance(hoverSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }

    public void UIPressedSFX()
    {
        var instance = RuntimeManager.CreateInstance(pressedSFX.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }


    public void UILoopExplodeSFX()
    {
        var instance = RuntimeManager.CreateInstance(explodeLoop.Guid);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        instance.start();
        instance.release();
    }
}
