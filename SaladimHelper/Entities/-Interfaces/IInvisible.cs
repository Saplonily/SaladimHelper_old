namespace Celeste.Mod.SaladimHelper.Entities;

public interface IInvisible
{
    public float CurrentAlpha { get; set; }

    public void SetCollideAble(bool value);

    public bool LightCheck();
}
