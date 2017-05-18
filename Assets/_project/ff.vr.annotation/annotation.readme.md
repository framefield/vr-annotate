# Annotation



## Interaction

Since the laser-pointer needs to work with different kind of InteractiveGizmos, we have to also incorporate the Node-Bounds-HitDetection:

- LaserPointer casts ray into scene to test for IInteractiveGizmo
- LaserPointer must also check the AnnotationHitTest for a hit
- note to self:
  - AnnationHitTest should be refactored into `NodeHitTester`
  - The `NodeHitTest` has to use the `.ContextNode` for testing
  - The `NodeNavigator` needs a reference to `NodeHitTester`.



`Node` 

`InteractiveGizmo`











[  ] 