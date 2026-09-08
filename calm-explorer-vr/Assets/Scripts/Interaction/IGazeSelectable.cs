namespace CalmExplorer
{
    public interface IGazeSelectable
    {
        void OnGazeEnter();
        void OnGazeExit();
        void OnGazeSelect();
    }
}
