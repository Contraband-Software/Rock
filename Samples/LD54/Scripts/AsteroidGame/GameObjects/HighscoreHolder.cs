namespace LD54.AsteroidGame.GameObjects
{
    using Microsoft.Xna.Framework;

    public class HighscoreHolder : GameObject
    {
        public int highScore;
        public HighscoreHolder(Game appCtx) : base("HighScoreContainer", appCtx)
        {
        }

        public override void OnLoad(GameObject? parentObject)
        {
            
        }
    }
}
