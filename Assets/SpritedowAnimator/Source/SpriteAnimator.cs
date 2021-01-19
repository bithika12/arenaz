// Spritedow Animation Plugin by Elendow
// http://elendow.com

using UnityEngine;
using UnityEngine.UI;

namespace Elendow.SpritedowAnimator
{
    /// <summary>
    /// Animator for Sprite Renderers.
    /// </summary>
    [AddComponentMenu("Spritedow/Sprite Animator")]
    //[RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : BaseAnimator
    {
        private bool isSprite = false;
        private SpriteRenderer spriteRenderer;
        private Image image;

        protected override void Awake()
        {
            isSprite = TryGetComponent<SpriteRenderer>(out spriteRenderer);
            if (!isSprite)
            {
                image = GetComponent<Image>();
            }           
            //spriteRenderer = GetComponent<SpriteRenderer>();
            base.Awake();
        }

        /// <summary>
        /// Changes the sprite to the given sprite
        /// </summary>
        protected override void ChangeFrame(Sprite frame)
        {
            if (isSprite)
            {
                spriteRenderer.sprite = frame;
            }
            else
            {
                image.sprite = frame;
            }
        }

        /// <summary>
        /// Enable or disable the renderer
        /// </summary>
        public override void SetActiveRenderer(bool active)
        {
            if (isSprite)
            {
                if (spriteRenderer == null)
                    spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.enabled = active;
            }
            else
            {
                if (image == null)
                    image = GetComponent<Image>();
                image.enabled = active;
            }
        }

        /// <summary>
        /// Flip the sprite on the X axis
        /// </summary>
        public override void FlipSpriteX(bool flip)
        {
            spriteRenderer.flipX = flip;
        }

        /// <summary>
        /// Flip the sprite on the Y axis
        /// </summary>
        public override void FlipSpriteY(bool flip)
        {
            spriteRenderer.flipY = flip;
        }

        /// <summary>
        /// The bounds of the Sprite Renderer
        /// </summary>
        public Bounds SpriteBounds
        {
            get { return spriteRenderer.bounds; }
        }

        /// <summary>
        /// The Sprite Renderer used to render
        /// </summary>
        public SpriteRenderer SpriteRenderer
        {
            get { return spriteRenderer; }
        }
    }
}