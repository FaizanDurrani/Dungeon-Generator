namespace Enemies.Entities
{
    public interface IEnemy
    {
        int Level { get; }
        int Health { get; }
        int SpawnsOnLevel { get; }
        string Description { get; }

        void Damage(int dmg);
        void ExecuteAction();
    }
}