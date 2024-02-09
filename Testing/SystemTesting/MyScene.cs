namespace Testing.SystemTesting;

using GREngine.Core.System;

public class MyScene : Scene
{
    public MyScene() : base("MyScene")
    {
    }

    protected override void OnLoad(Node rootNode, Node persistantNode)
    {
        MyNode myNode = new MyNode();
        MyBehaviour myBehaviour = new MyBehaviour();

        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(myNode, myBehaviour);

        this.Game.Services.GetService<ISceneControllerService>().AddNodeAtRoot(myNode);
    }
}
