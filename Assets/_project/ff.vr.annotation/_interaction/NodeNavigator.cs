class NodeNavigator {
    public Node ContextNode;
    public Node HoveredNode; //  Must be a child of ContxtNode
    public Node SelectedNode;

    public NodeHitTester _hitTester
}

interface IHitTester {

}

interface IInteractiveGizmo 
{

}

interface IRayHitTester {
    bool CheckForIntersection(Ray ray);
}

public NodeHitTester:IInteractiveGizmo {

}

// Note-To-Self:
class InteractiveController {
    public IInteractiveGizmo hoveredGizma;

}

// Note-To-Self:
// - maybe we can reuse hitResult for NodeHitTester
class LaserPointer 
{

    public IHitTester[] _additionalHitTesters;

    void Update() 
    {
        Ray ray;

        IInteractiveGizmo hitByColliderResult;

        
        IInteractiveGizmo
    }
}