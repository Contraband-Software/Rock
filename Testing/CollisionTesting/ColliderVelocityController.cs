namespace Testing.CollisionTesting
{
    using GREngine.Core.Physics2D;
    using GREngine.Core.System;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ColliderVelocityController : Behaviour
    {
        Collider col1;
        public ColliderVelocityController(
            Collider col1
            )
        {
            this.col1 = col1;
        }

        protected override void OnStart()
        {
            col1.SetVelocity(new Vector2(0.2f, 0));
        }
    }
}
