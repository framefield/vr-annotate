# vr-annotate / Documentation

## Overview

This unity-package allows to make annotations on arbitrary Unity-scenes of architectural sites. The generated annotation-data can be transformed into real world coordinates and because the data-format is JSON-ld, it can be cross referenced with other open databases.

### Contact

VR-Annotate is a project of the [Image Knowledge Gestaltung - Cluster of Excellence](https://www.interdisciplinary-laboratory.hu-berlin.de/en/bwg/) of the Humboldt-University Berlin.
Developed by [Framefield.com](http://www.framefield.com)

### Version-History
- **2017-07-19 /  v0.1.0** - initial release of unity package
### License

This project is released under M.I.T. license.


### Dependencies

Part of the package are the following non-unity dependencies:

- [JSONObject](https://github.com/mtschoen/JSONObject) by  Matt Schoen [schoen@defectivestudios.com](mailto:schoen@defectivestudios.com) of [Defective Studios](http://www.defectivestudios.com/)
- [Punchkeyboard](https://github.com/rjth/Punchkeyboard) by Jonathan Ravasz (MIT-license)
- [InputSimulator](https://github.com/michaelnoonan/inputsimulator) by Michael Noonan


## What it does

Once you added to required prefabs to a part of your scene, you use the Vive-controller to select it’s geometry, browse the hierarchical structure of the scene, place comments to certain parts of the geometry by pressing trigger and using a virtual keyboard or reading the comments of other users.

The package consists of several components:


1. **Target**- make meshes in parts of a unity-scene click- and annotatable
2. **Annotation** - create and save annotations to Nodes
3. **LocationMarker** - converts Unity Coordinates into real-world WGS84 Coordinates
4. **InfoPanel** - a virtual UI-control that show details about the current selections and allows navigating unity’s scene graph-structure.
5. **PunchKeyboard** - an virtual keyboard for the SteamVR (open source 3rd party package) See [github-repository](https://github.com/rjth/Punchkeyboard)


## Making a scene annotatable
### Add annotation-features
1. Install the **vr-annotate-package** into your project
2. Open a scene or create a new one
3. Additionally open the scene `vr-annotate-example`  and copy the following groups into your scene:
  - [geoCoordinates]
  - optionally also copy the adjusted `[CameraRig]` for SteamVR. 
4. **Alternatively** you can use to original CameraRig-prefab and…
    - Add the following components to the Controllers within [CameraRig]-prefab: `InteractiveController` and `Teleportation`
    - Add use the `vr-annotate/SteamVR_WithTeleportation` prefab
    - Add  `vendors/punchkeyboard/prefabs/DrumStickForController` prefab to both controllers
    - In the [Punchkeyboard] instance of your scene set references to the SteamVR-`InteractiveController`s and the `DrumStickForController`-instances
5. Select the top-geometry nodes in your scene.
6. In the Inspector select [Add Component], search for `T`arget and add it. This will scan the geometry of your node when starting the scene and make it annotatable.

### Add meta-information
7. Make sure all TargetNodes have a GUID stored as an Suffix in their gameobject.name. 
  If necessary use the menu entry `vr-annotate/Generate GUIDs for all TargetNodes` to generate the missing GUIDs
8. Distribute and scale instances of the `TeleportationPlane`-prefab in your scene.
9. Start the scene

### Describe which real world location the scene uses
10. Insert the **GeoCoordinatesTransformer**-prefab to your scene
11. find two or more representative structures in your scene for which you have very precise longitude and latitude data: things like corners or buildings, centers of columns. You can get the geo-coordinates from satellite images.
12. For each of these features insert a `GeolocationMarker`-prefab.
13. position the GeolocationMarker at the features
14. enter the geo-coordinates for this feature

### Define the current user
Until we have a proper database, the current user is defined in through the parameters place-holder script:
16. Find or add the `CurrentUserDefinition`-prefab to your scene.
17. Enter its parameters for *username*, *rename* and *email* (this data will be stored with the annotations and can later be used for a proper user database table).

## Components

The following section gives a quick introduction into the code-structure. The folder-structure within the package follows the namespace of the c# scripts. The package consists of several namespaces, each focusing on a different aspect. 

### ff.nodegraph

This MonoBehaviors of this namespace scan a nested structure of GameObjects within a unity-scene, looks for MeshRenderers and use BoundingBoxes to build data representation of the scene that can be used for hit-testing and picking. We decided to implement our own Ray-HitTesting because creating many instances of Unity-colliders had a big performance impact and required manipulation of your scene. By contrast just scanning the scene and saving an interpretation as  data allowed use to use this data for highlighting, picking, and storing precise target-references for annotations.


### ff.vr.annotation

The Components in the `ff.annotation` namespace handle the creation, serialisation, and interaction with Annotations. Most elements are combined into prefabs. 


### ff.locationmarker

The components inside this namespace help to converts Unity Coordinates into real-world WGS84 coordinates. For this you place two or more markers in your unity-scene and add precise geo-location coordinates for these markers. Good candidates are existing real-world remains like columns or base-structures. From this columns, unity-positions can be converted into lat-long. 

This can be very useful for…

- showing annotations on a map
- exporting annotations to show with a HoloLens or other AR devices
- importing and correctly placing annotations or other data into the scene


### InformationPanel, SelectionManager, ISelectable, NodeSelectionMarker

The components in these namespaces are related to showing additional information about the current selection. Currently selections can be  `Node`s inside the NodeGraph, and `AnnotationsGizmos`. There are several methods to change the selection (clicking on objects, clicking into the NodeGraphOutliner, etc.). The SelectionManager handles to current active selection of object implementing the `ISelectable`-interface. Components like the `NodeSelectionMarker`  or the `InfoPanels` subscribe to the SelectionChanged-event to update their content.



## Using IDs for creating a network of linked data 

[GUIDs](https://de.wikipedia.org/wiki/Globally_Unique_Identifier) are used to identify **Targets**, **Annotations** and **Nodes.**

### IDs for Nodes

The following schema is used for 3d-Models  within Maya, Cinema4D, Unity, etc:  

  `**name**` **+** `**#**` **+** `**GUID**` ****

e.g.:     

  `***Crepidoma#936DA01F-9ABD-4D9D-80C7-02AF85C822A8***`

`**name**` ****is not used to identify the object and can be changed any time. 

**Workflow**

- The 3d-modeller creates geometry and decides on topology of the modell (its number of submeshes and their relationship).
- Before the modell is exported for Unity a GUID should be generated for each submesh and stored as a suffix of their name:


    Mesh#936DA01F-9ABD-4D9D-80C7-02AF85C822A8
        - SubMesh#936DA01F-9ABD-4D9D-80C7-02AF85C822A8
            - SubMesh#936DA01F-9ABD-4D9D-80C7-02AF85C822A8]
            - ...
        - SubMesh#936DA01F-9ABD-4D9D-80C7-02AF85C822A8]
        - SubMesh#936DA01F-9ABD-4D9D-80C7-02AF85C822A8]
        - ...


- The model is imported into Unity and used within a scene as a *Target*.
- In case the modeller did not identify all submeshes correctly, you can use the Unity-Helper 
  `vr-annotate/Generate GUIDs for all TargetNodes` to generate the missing GUID suffixes.
### IDs for Targets and Annotations:

As defined in [neonion-rest](https://github.com/FUB-HCC/neonion-rest), Targets and Annotations IDs declare their type in a suffix, like 
`type` + `:` + `GUID`, e.g. `target:234234-234234-234234`.

**Workflow**
In Unity all Targets- and Annotations-IDs are created automatically in the right format before serialization.

## De-/Serialization

At the moment Targets and Annotations are stored on a [local mock server](https://github.com/FUB-HCC/neonion-rest):

Targets are stored at: 
`**localhost:8301/targets/**`

e.g.
`**localhost:8301/targets/target:**``***936DA01F-9ABD-4D9D-80C7-02AF85C822A8***`**`**

Annotations at: 
`**localhost:8301/targets/targetid/annotations/**`

e.g.
`**localhost:8301/targets/target:936DA01F-9ABD-4D9D-80C7-02AF85C822A8**`
`**/annotations/annotation:AB12E3GD-9ABD-4D9D-80C7-KH6TGE8DNB7W**`




They are stored using the following JSON-schemas



## JSON-Schemas


### Notes on the JSON-Schema

Following the [JSON-LD](https://json-ld.org/) format was an important requirements for the implementation (for details please refer to +vr-annotate / Standards and [this comment](/doc/vr-annotate-Discussion-Updates-55GOPr9dfFic1OPSB6k7B#:uid=833873351002048394383899&h2=2017-06-13---Discussing-Data-f)). During the development we invested some time into suggesting a JSON-schema which closely follows the W3C-web annotation standard but makes some changes where necessary. The implementation of the serialization is done in `Annotation.cs`. We started with a light weight template mechanism for quick iterations. For now, the Annotations are stored as JSON-files on disk to `Assets/db/*.json`. Once a database or API is available it should be integrated and the JSON-serialization in `Annotation.cs` should be properly implemented through structs and classes.

The JSON schema should be treated as work in progress and leaves many details with potential for refinement. Especially details on simulation state and archeological time-formats need further work. (see section future work below).



### Annotations:
    {
        "@context": "http://www.w3.org/ns/anno.jsonld",
        "id": "annotation:0a1d64db-5b3d-41b8-8a92-14e8493a2fc2",
        "type": "Annotation",
        "creator": {
            "id": "_alan",
            "name": "Alan",
            "email": "alan@google.com"
        },
        "created": "9/20/2017 3:27:55 PM",
        "generator": {
            "id": "http://vr-annotator/v/01",
            "type": "Software",
            "name": "VR-Annotator v0.1"
        },
        "body": [
            {
                "type": "TextualBody",
                "purpose": "describing",
                "value": "annotation"
            }
        ],
        "target": {
            "id": "{target:8fa0f93c-6b2a-4eb0-bd16-3ddccca72e64}",
            "type": "http://vr-annotator/feature/",
            "targetNodeName": "Mauer_lp",
            "state": {
                "type": "VRSimulation",
                "refinedBy": {
                    "type": "SimulationTime",
                    "sourceDate": "302 BC",
                    "timeOfDay": "18:23:12"
                }
            },
            "selector": {
                "type": "nodeGraphPath",
                "guidPath": "8fa0f93c-6b2a-4eb0-bd16-3ddccca72e64/84280e5f-ab67-4ef7-bce6-2bd71d922152/"
            },
            "annotationPosition": {
                "position": {
                    "x": -2.088443,
                    "y": 1.86834,
                    "z": -2.16
                },
                "type": "GeoCoordinates",
                "coordinateSystem": "Unity.WorldSpace",
                "latitude": 37.97617,
                "longitude": 23.7223,
                "elevation": 1.879078E-05
            },
            "viewPortPosition": {
                "position": {
                    "x": 0.8888178,
                    "y": 0.9042585,
                    "z": -7.195422
                },
                "type": "GeoCoordinates",
                "coordinateSystem": "Unity.WorldSpace",
                "latitude": 37.97619,
                "longitude": 23.72224,
                "elevation": 9.094559E-06
            }
        }
    }



### Targets:
    {
      "@context": {
        "@vocab":"http://www.w3.org/ns/target.jsonld",
        "@base": "http://annotator/target/",
      },
      "id": "{TargetGUID}",
      "@type": "AnnotationTarget",
      "creator": {"id":"_alan","name":"Alan","email":"alan@google.com"},
      "created": "7/10/2017 7:02:44 PM",
      "generator": {
        "id": "http://vr-annotator/v/02",
        "type": "Software",
        "name": "VR-Annotator v0.2"
      },
      "interpretation": {
        "refinedBy": {
          "modellerName":"",
          "modellingSoftware":"",
          "references": []
        }     
      },
      "nodeGraph": [
        {
          "type":"AnnotatableNode",
          "@id":"{{TargetGUID}}",
          "children":[
            {
              "type":"AnnotatableNode",
              "@id":"{{ChildnodeNodeGUID}}",
              "children":[
                 ...
              ]
            }        
          ]
        }
      ],
    }



## Picking, Raycasts

![](https://d2mxuefqeaa7sj.cloudfront.net/s_4189704474AC678D2E263F90FDFBA4024E622E925DF615D1BA9574E1FF0600CA_1505920697055_UnbenanntWithIndex.png)



1. **Global Bounding Box**
2. **Local Bounding Box**
3. **Mesh Collider**

During picking of `TargetNodes` we use three types of raycast hittests to determine whether an object was hit. The first test against the **Global Bounding Box** is the fastest but least precise, while the test against the **Mesh Collider** is precise and expensive. Only if the **Global Bounding Box** is hit, the next more precise hittest is carried out, etc. 

Optimizing …

- Global Bounding Box: align your model to global axes
- Local Bounding Box: align your submodels to their local axes
- Mesh Collider: keep mesh size small



## Other Requirements

Model must not use “/” in GameObject names for Geometry-nodes inside `NodeGraph`s


## FAQ
### Does the it support the Oculus Rift?

Not yet, but a porting the `InteractiveController`-Script to the Rift should be very easy.


### Does it support other platforms like GearVR and HoloLens?

Currently the Interaction heavily relies on pointing and clicking with a controller. But adapting the interaction to triggering by gaze should be possible.

## Future work

We suggest the following steps to build on this ground-work

### Review and clarify the JSON-ld schema and integration with other linked data sources
- Add proper @context-information
- Talk to other experts doing a similar thing.
- Validate the context definition and RDF link structure with tools.
- Clarify format for historic dates time.
### Expand the data models

Expand models for **Users** and **AnnotationSeries** to allow cross-referencing and filtering annotations.

### Implement contextual filters for annotations

Currently all annotations are always visible. Eventually these need to be filtered by things like: Tags, users, date-ranges, etc.

### Save additional information about the state of the VR-simulation

Examples would be time of day, weather conditions, year etc.

### Add further input methods to make annotations

Examples could be:

- using tags
- using voice recognition
- using symbols or icons
- Sketching in VR
### Export or link annotations into the real world

With technology like HoloLens and location-markers it is very feasible to show markers created in VR within their context. You can also port the components to work on the HoloLens to generate annotations.

