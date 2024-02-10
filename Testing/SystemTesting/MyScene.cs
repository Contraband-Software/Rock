namespace Testing.SystemTesting;

using GREngine.Core.System;
using GREngine.Debug;
using GREngine.GameBehaviour.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MyScene : Scene
{
    public MyScene() : base("MyScene")
    {
    }

    protected override void OnLoad(Node rootNode, Node persistantNode)
    {
        MyNode myNode = new MyNode();
        MyBehaviour myBehaviour = new MyBehaviour();

        ISceneControllerService sc = this.Game.Services.GetService<ISceneControllerService>();

        sc.AddBehaviour(myNode, myBehaviour);
        sc.AddBehaviour(myNode, new UpdateNotifier());
        sc.AddNodeAtRoot(myNode);

        GenericNode node = new GenericNode();
        NodeNetwork network = new NodeNetwork();
        sc.AddBehaviour(node, network);
        sc.AddNode(myNode, node);

        sc.QueueSceneAction(() =>
        {
            node.SetLocalPosition(new Vector2(100, 100));
            network.BuildNetwork(600, 600, "", "", 10);

            network.Navigate(new Point(10, 10), new Point(150, 260));
        });
    }
}

class UpdateNotifier : Behaviour
{
    protected override void OnUpdate(GameTime gameTime)
    {
        this.Game.Services.GetService<ISceneControllerService>().DebugPrintGraph();
    }
}
