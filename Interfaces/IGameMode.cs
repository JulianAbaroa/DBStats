
namespace DBStats.Interfaces;

public interface IGameMode
{
    void AddStats(IGameMode other);
    double GetScore();
}