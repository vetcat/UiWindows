using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopRightView : WindowComponent
    {
        public RectTransform Body;
        public Image ImageIconCoins;
        public Text TextCoinsAmount;

        public RectTransform CoinIconRectTransform =>
            ImageIconCoins != null ? (RectTransform)ImageIconCoins.transform : null;

        public void EnsureLayout()
        {
            if (Body != null && ImageIconCoins != null && TextCoinsAmount != null)
            {
                return;
            }

            throw UiPrefabReferenceGuard.Missing(
                this,
                "Body, ImageIconCoins, TextCoinsAmount");
        }

        public void SetCoins(int coins)
        {
            EnsureLayout();
            if (TextCoinsAmount != null)
            {
                TextCoinsAmount.text = coins.ToString();
            }
        }
    }
}
