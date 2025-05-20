using Godot;
using System;

public partial class HealthBar : CanvasLayer
{
    private const int MinBarValue = 23;
    private const int MaxHealth = 100;

    private TextureProgressBar tProgress;
    private Tween tweener;

    public override void _Ready()
    {
        tProgress = GetNode<TextureProgressBar>("HealthBar");
        tweener = CreateTween();
    }

    // Llama a este método desde el jugador cuando la vida cambie
    public void UpdateHealthBar(int playerHealth)
    {
        // Convertimos la vida real (0-100) al rango visible (23-100)
        int visibleValue = Mathf.RoundToInt(Mathf.Lerp(MinBarValue, MaxHealth, playerHealth / 100.0f));

        // Detenemos cualquier tween anterior
        tweener.Kill();
        tweener = CreateTween();

        // Animamos suavemente la barra
        tweener.TweenProperty(tProgress, "value", playerHealth, 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }
}
