using ProjectContext.UiRequests;

namespace ProjectContext.Player
{
    public interface IPlayerCommands
    {
        void SetHealth(int value);
        void AddHealth(int amount);
        void SetXp(int value);
        void AddXp(int amount);
        void SetCoins(int value);
        void AddCoins(int amount);
        void RemoveCoins(int amount);
        void AddCoinsWithFx(int amount);
        void AddCoinsWithFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins);
        void RemoveCoinsWithFx(int amount);
        void RemoveCoinsWithFxFrom(int amount, UiFxTarget source, UiFxTarget target = UiFxTarget.Coins);
        void SetLevel(int value);
        void SetName(string value);
    }
}
