namespace Creatures
{
    public static class AlignmentSystem 
    {
        private static readonly bool[,] _enemyMatrix = new bool[8, 8];

        static AlignmentSystem()
        {
            int ally = (int)Alignment.Ally;
            int evilAlly = (int)Alignment.EvilAlly;
            int neutral = (int)Alignment.Neutral;
            int evil = (int)Alignment.Evil;
            int enemy = (int)Alignment.Enemy;
            int evilEnemy = (int)Alignment.EvilEnemy;
            int fullyFriendly = (int)Alignment.FullyFriendly;

            _enemyMatrix[ally, enemy] = true;
            _enemyMatrix[ally, evilEnemy] = true;
            _enemyMatrix[ally, evil] = true;

            _enemyMatrix[evilAlly, neutral] = true;
            _enemyMatrix[evilAlly, evil] = true;
            _enemyMatrix[evilAlly, enemy] = true;
            _enemyMatrix[evilAlly, evilEnemy] = true;
            _enemyMatrix[evilAlly, fullyFriendly] = true;

            _enemyMatrix[neutral, evilEnemy] = true;
            _enemyMatrix[neutral, evilAlly] = true;
            _enemyMatrix[neutral, evil] = true;

            for (int i = 0; i < 7; i++) 
                _enemyMatrix[evil, i] = true;

            _enemyMatrix[enemy, ally] = true;
            _enemyMatrix[enemy, evilAlly] = true;
            _enemyMatrix[enemy, evil] = true;

            _enemyMatrix[evilEnemy, ally] = true;
            _enemyMatrix[evilEnemy, evilAlly] = true;
            _enemyMatrix[evilEnemy, neutral] = true;
            _enemyMatrix[evilEnemy, evil] = true;
            _enemyMatrix[evilEnemy, fullyFriendly] = true;
        }
        
        public static bool IsEnemy(Alignment source, Alignment other) {
            return _enemyMatrix[(int)source, (int)other];
        }
    }

    public enum Alignment : int { Ally, EvilAlly, Neutral, Evil, Enemy, EvilEnemy, FullyFriendly }
}