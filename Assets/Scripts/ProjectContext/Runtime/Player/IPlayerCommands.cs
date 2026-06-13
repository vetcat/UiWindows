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
        void SetLevel(int value);
        void SetName(string value);
    }
}