namespace CS2External
{
    public class GameSettings : IGameSettings
    {
        public bool IsBunnyHopEnabled
        {
            get; set;
        } = false;

        public bool IsAntiFlashEnabled
        {
            get; set;
        } = false;

        public int Fov
        {
            get; set;
        } = 60;

        public int RecoilControlAmount
        {
            get; set;
        } = 0; 
    }

    public interface IGameSettings
    {
        bool IsBunnyHopEnabled
        {
            get; set;
        }

        bool IsAntiFlashEnabled
        {
            get; set;
        }

        int Fov
        {
            get; set;
        }

        int RecoilControlAmount
        {
            get; set;
        }
    }
}