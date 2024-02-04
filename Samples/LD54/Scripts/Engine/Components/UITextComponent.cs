namespace LD54.Scripts.Engine.Components
{
    using LD54.Engine.Leviathan;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UITextComponent : Component
    {
        private LeviathanUIElement? textSprite;

        private ILeviathanEngineService re;
        public string text;
        private float textWidth;
        private float textHeight;
        SpriteFont font;
        private Vector2 scale;

        public UITextComponent(string name, Game appCtx) : base(name, appCtx)
        {

        }
        ~UITextComponent()
        {
            PrintLn("UITextComponent destroyed");
        }

        public void LoadTextElementData(
            Game appCtx,
            Matrix transform,
            Vector2 size,
            string text,
            SpriteFont font,
            Color color,
            bool isEnabled = true)
        {
            this.font = font;
            this.scale = size;
            this.text = text;
            re = this.app.Services.GetService<ILeviathanEngineService>();
            textSprite = new(appCtx, transform, size, text, font, color, isEnabled);
            re.addUISprite(textSprite);
        }

        public override void OnLoad(GameObject? parentObject)
        {
            gameObject = parentObject;
        }

        public override void OnUnload()
        {
            re.removeUISprite(textSprite);
        }

        private void CalculateTextDimensions()
        {
            textWidth = font.MeasureString(text).X;
            textHeight = font.MeasureString(text).Y;
        }

        public void PositionXAtScreenCentre()
        {
            CalculateTextDimensions();

            float screenWidth = re.getWindowSize().X;
            float posX = (screenWidth / 2f) - (textWidth / 2f);
            float currY = textSprite.GetPositionXY().Y;
            textSprite.SetPosition(new Vector3(posX, currY,0));
        }

        public void PositionYAtScreenCentre(Vector2 offset)
        {
            CalculateTextDimensions();

            float screenHeight = re.getWindowSize().Y;
            float posY = (screenHeight / 2f) - (textHeight / 2f) + offset.Y;
            float currX = textSprite.GetPositionXY().X + offset.X;
            textSprite.SetPosition(new Vector3(currX, posY, 0));
        }

        public void PositionXAtRightEdge(Vector2 offset)
        {
            CalculateTextDimensions();

            float screenWidth = re.getWindowSize().X;
            float posX = screenWidth - textWidth + offset.X;
            float currY = offset.Y;
            textSprite.SetPosition(new Vector3(posX, currY, 0));
        }

        public void PositionBottomRight(Vector2 offset)
        {
            CalculateTextDimensions();

            float screenWidth = re.getWindowSize().X;
            float screenHeight = re.getWindowSize().Y;
            float posX = screenWidth - textWidth + offset.X;
            float posY = screenHeight - textHeight + offset.Y;
            textSprite.SetPosition(new Vector3(posX, posY, 0));
        }

        public void SetText(string s)
        {
            this.text = s;
            textSprite.text = s;
        }
    }
}
