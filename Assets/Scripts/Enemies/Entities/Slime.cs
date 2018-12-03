namespace Enemies.Entities
{
    public class Slime : IEnemy
    {
        private int _level;
        private int _health;
        private int _spawnsOnLevel;
        private string _description;

        public int Level => _level;
        public int Health => _health;
        public int SpawnsOnLevel => _spawnsOnLevel;
        public string Description => _description;

        public void Damage(int dmg)
        {
            _health -= dmg;
        }

        public void ExecuteAction() 
        {
            
        }
    }
}