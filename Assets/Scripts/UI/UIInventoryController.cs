using UI;

public class UIInventoryController : UiBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        m_canvasGroup.alpha = 0.0f;
    }
}
