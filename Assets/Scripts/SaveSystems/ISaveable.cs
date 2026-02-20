public interface ISaveable
{
    string SaveID { get; }
    string CaptureState();          // returns JSON
    void RestoreState(string json); // receives JSON
}