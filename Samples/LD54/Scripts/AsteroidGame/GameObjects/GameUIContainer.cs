namespace LD54.AsteroidGame.GameObjects
{
    using LD54.AsteroidGame.GameObjects;
    using LD54.AsteroidGame.Scenes;
    using LD54.Scripts.Engine.Components;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework.Audio;

    public class GameUIContainer : GameObject
    {
        UITextComponent scoreText;
        UITextComponent hitCounter;
        UITextComponent gameOverText;
        UITextComponent finalScore;
        UITextComponent highscoreText;
        List<SpriteFont> fonts;
        private enum UIState { PLAYER_ALIVE, PLAYER_DEAD};
        private UIState state = UIState.PLAYER_ALIVE;
        private int hitsTaken = 0;

        GameScene gameScene;
        private float score = 0;

        private SoundEffect gameOver;

        public GameUIContainer(List<SpriteFont> gameUI, SoundEffect gameOver, string name, Game appCtx) : base(name, appCtx)
        {
            this.gameOver = gameOver;

            fonts = gameUI;
            scoreText = new UITextComponent("ui", app);
            scoreText.LoadTextElementData(
                app,
                this.GetGlobalTransform(),
                new Vector2(1, 1),
                "SCORE: 0",
                fonts[0],
                new Color(255, 255, 255));
            this.AddComponent(scoreText);
            scoreText.PositionXAtRightEdge(new Vector2(-20, 10));

            fonts = gameUI;
            hitCounter = new UITextComponent("ui", app);
            hitCounter.LoadTextElementData(
                app,
                this.GetGlobalTransform(),
                new Vector2(1, 1),
                "HITS TAKEN: 0",
                fonts[0],
                Color.Crimson);
            this.AddComponent(hitCounter);
            hitCounter.PositionBottomRight(new Vector2(0,0));
        }

        public override void OnLoad(GameObject? parentObject)
        {
            this.SetLocalPosition(new Vector2(0,0));

            gameScene = this.app.Services.GetService<ISceneControllerService>().GetCurrentScene() as GameScene;
            if(gameScene != null)
            {
                gameScene.player.AsteroidHitEvent += IncreaseHitCounter;
            }

            state = UIState.PLAYER_ALIVE;
        }

        private void IncreaseHitCounter()
        {
            hitsTaken++;
            hitCounter.SetText("HITS TAKEN: " + hitsTaken.ToString());
            hitCounter.PositionBottomRight(new Vector2(0, 0));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(state == UIState.PLAYER_ALIVE)
            {
                score += ((float)gameTime.ElapsedGameTime.Milliseconds / 1000f) *
                    MathF.Pow(1 - gameScene.player.BlackHoleOrbitRadius / Spaceship.MaxRadius, 2) * 12f;

                UpdateScore(gameTime);
            }
        }

        public void OnGameOver()
        {
            if(state == UIState.PLAYER_DEAD)
            {
                return;
            }
            this.gameOver.Play();
            PrintLn("Game Over");
            if(score > gameScene.highscore.highScore)
            {
                gameScene.highscore.highScore = (int)score;
            }

            state = UIState.PLAYER_DEAD;

            gameOverText = new UITextComponent("ui", app);
            gameOverText.LoadTextElementData(
                app,
                this.GetGlobalTransform(),
                new Vector2(1, 1),
                "GAME OVER",
                fonts[1],
                Color.Red,
                true);
            this.AddComponent(gameOverText);
            gameOverText.PositionXAtScreenCentre();
            gameOverText.PositionYAtScreenCentre(new Vector2(0,0));

            //========================================

            highscoreText = new UITextComponent("ui", app);
            highscoreText.LoadTextElementData(
                app,
                this.GetGlobalTransform(),
                new Vector2(1, 1),
                "HIGHSCORE:" + " " + gameScene.highscore.highScore.ToString(),
                fonts[0],
                Color.Crimson,
                true);
            this.AddComponent(highscoreText);
            highscoreText.PositionXAtScreenCentre();
            highscoreText.PositionYAtScreenCentre(new Vector2(0, 100));

            finalScore = new UITextComponent("ui", app);
            finalScore.LoadTextElementData(
                app,
                this.GetGlobalTransform(),
                new Vector2(1, 1),
                "FINAL SCORE",
                fonts[0],
                Color.Crimson,
                true);
            this.AddComponent(finalScore);

            finalScore.SetText("SCORE: " + ((int)score).ToString());

            finalScore.PositionXAtScreenCentre();
            finalScore.PositionYAtScreenCentre(new Vector2(0,150));

            //============================
            UITextComponent restartText = new UITextComponent("ui", app);
            restartText.LoadTextElementData(
                app,
                this.GetGlobalTransform(),
                new Vector2(1, 1),
                "PRESS [R] TO RESTART",
                fonts[0],
                Color.White,
                true);
            this.AddComponent(restartText);
            restartText.PositionXAtScreenCentre();
            restartText.PositionYAtScreenCentre(new Vector2(0, 200));
        }

        private void UpdateScore(GameTime gameTime)
        {
            scoreText.SetText("SCORE: " + ((int)score).ToString());
            scoreText.PositionXAtRightEdge(new Vector2(-20, 10));
        }
    }
}
