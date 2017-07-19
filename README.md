# vr-annotate / Documentation

# Overview

This unity-package allows to make annotations on arbitrary Unity-scenes of architectural sites. The generated annotation-data can be transformed into real world coordinates and because the data-format is JSON-ld, it can be cross references with other open databases.

Also check the [live version](https://paper.dropbox.com/doc/vr-annotate-Documentation-YxSHjwYodKZkEP93F6NNX) of this document.

# Contact

VR-Annotate is a project of the [TOPOI - Clusters of Excellence](http://www.topoi.org/) of the Humboldt-University Berlin.

Developed by [Framefield.com](http://www.framefield.com)

# Version-History

## 2017-07-19  v0.1.0 - initial release of unity package

# License

This project is released under M.I.T. license.

# Dependencies

Part of the package are the following non-unity dependencies:

- [JSONObject](https://github.com/mtschoen/JSONObject) by  Matt Schoen [schoen@defectivestudios.com](mailto:schoen@defectivestudios.com) of [Defective Studios](http://www.defectivestudios.com/)
- [Punchkeyboard](https://github.com/rjth/Punchkeyboard) by Jonathan Ravasz (MIT-license)
- [InputSimulator](https://github.com/michaelnoonan/inputsimulator) by Michael Noonan

# What it does

Once you added to required prefabs to a part of your scene, you use the Vive-controller to select it’s geometry, browse the hierarchical structure of the scene, place comments to certain parts of the geometry by pressing trigger and using a virtual keyboard or reading the comments of other users.

The package consists of several components:

1. **NodeGraph**** **- make meshes in parts of a unity-scene clickable
2. **Annotation**** **- create and save annotations to Nodes
3. **LocationMarker**** **- converts Unity Coordinates into real-world WGS84 Coordinates
4. **InfoPanel** - a virtual UI-control that show details about the current selections and allows navigating unity’s scene graph-structure.
5. **PunchKeyboard** - an virtual keyboard for the SteamVR (open source 3rd party package) See [github-repository](https://github.com/rjth/Punchkeyboard)

# Making a scene annotatable

## Add annotation-features

1. Install the **vr-annotate-package** into your project

2. Open a scene or create a new one

3. Additionally open the scene vr-annotate-example  and copy the following groups into your scene:

4. - [vr-annotate]
   - [geoCoordinates]
   - optionally also copy the adjusted [CameraRig] for SteamVR. 

5. **Alternatively **you can use to original CameraRig-prefab and…

6. - - Add the following components to the Controllers within [CameraRig]-prefab: InteractiveController and Teleportation
     - Add use the vr-annotate/SteamVR_WithTeleportation prefab
     - Add  vendors/punchkeyboard/prefabs/DrumStickForController prefab to both controllers
     - In the [Punchkeyboard] instance of your scene set references to the SteamVR-InteractiveControllers and the DrumStickForController-instances

7. Select the top-geometry nodes in your scene.

8. In the Inspector select [Add Component], search for NodeGraph and add it. This will scan the geometry of your node when starting the scene and make it annotatable.

## Add meta-information

1. Add good descriptive ID into the RootNodeId parameter  (ideally this should be a unique [IRI](https://www.w3.org/TR/ld-glossary/#internationalized-resource-identifier))
2. Optionally add additional details on how the geometry interprets an archeological structure.
3. Distribute and scale instances of the TeleportationPlane-prefab in your scene.
4. Start the scene

## Describe which real world location the scene uses

1. Insert the **GeoCoordinatesTransformer**-prefab to your scene
2. find two or more representative structures in your scene for which you have very precise longitude and latitude data: things like corners or buildings, centers of columns. You can get the geo-coordinates from satellite images.
3. For each of these features insert a GeolocationMarker-prefab.
4. position the GeolocationMarker at the features
5. enter the geo-coordinates for this feature

## Define the current user

Until we have a proper database, the current user is defined in through the parameters place-holder script:

1. Find or add the CurrentUserDefinition-prefab to your scene.
2. Enter its parameters for *username*, *rename *and *email** *(this data will be stored with the annotations and can later be used for a proper user database table).

# Components

The following section gives a quick introduction into the code-structure. The folder-structure within the package follows the namespace of the c# scripts. The package consists of several namespaces, each focusing on a different aspect. 

## ff.nodegraph

This MonoBehaviors of this namespace scan a nested structure of GameObjects within a unity-scene, looks for MeshRenderers and use BoundingBoxes to build data representation of the scene that can be used for hit-testing and picking. We decided to implement our own Ray-HitTesting because creating many instances of Unity-colliders had a big performance impact and required manipulation of your scene. By contrast just scanning the scene and saving an interpretation as  data allowed use to use this data for highlighting, picking, and storing precise target-references for annotations.

## ff.vr.annotation

The Components in the ff.annotation namespace handle the creation, serialisation, and interaction with Annotations. Most elements are combined into prefabs. 

## ff.locationmarker

The components inside this namespace help to converts Unity Coordinates into real-world WGS84 coordinates. For this you place two or more markers in your unity-scene and add precise geo-location coordinates for these markers. Good candidates are existing real-world remains like columns or base-structures. From this columns, unity-positions can be converted into lat-long. 

This can be very useful for…

- showing annotations on a map
- exporting annotations to show with a HoloLens or other AR devices
- importing and correctly placing annotations or other data into the scene

## InformationPanel, SelectionManager, ISelectable, NodeSelectionMarker

The components in these namespaces are related to showing additional information about the current selection. Currently selections can be  Nodes inside the NodeGraph, and AnnotationsGizmos. There are several methods to change the selection (clicking on objects, clicking into the NodeGraphOutliner, etc.). The SelectionManager handles to current active selection of object implementing the ISelectable-interface. Components like the NodeSelectionMarker  or the InfoPanels subscribe to the SelectionChanged-event to update their content.

# Notes on the JSON-Schema

Following the [JSON-LD](https://json-ld.org/) format was an important requirements for the implementation (for details please refer to [+vr-annotate / Standards](https://paper.dropbox.com/doc/6cwBVu8i6VSSSoqLgHyz0) and [this comment](https://paper.dropbox.com/doc/vr-annotate-Discussion-Updates-55GOPr9dfFic1OPSB6k7B#:uid=833873351002048394383899&h2=2017-06-13---Discussing-Data-f)). During the development we invested some time into suggesting a JSON-schema which closely follows the W3C-web annotation standard but makes some changes where necessary. The implementation of the serialization is done in Annotation.cs. We started with a light weight template mechanism for quick iterations. For now, the Annotations are stored as JSON-files on disk to Assets/db/*.json. Once a database or API is available it should be integrated and the JSON-serialization in Annotation.cs should be properly implemented through structs and classes.

The JSON schema should be treated as work in progress and leaves many details with potential for refinement. Especially details on simulation state and archeological time-formats need further work. (see section future work below).

An example format looks like this following:

```json
{
    "@context": "http://www.w3.org/ns/anno.jsonld",
    "id": "http://annotator/anno/1af622d6-b98f-4622-8d99-ccf42082338b",
    "type": "Annotation",
    "creator": {"id":"_alan","name":"Alan","email":"alan@google.com"},
    "created": "7/10/2017 7:02:44 PM",
    "generator": {
        "id": "http://vr-annotator/v/01",
        "type": "Software",
        "name": "VR-Annotator v0.1"
    },
    "body": [
        {
            "type": "TextualBody",
            "purpose": "describing",
            "value": "cx"
        }
    ],
    "target": {
        "type": "http://vr-annotator/feature/",
        "rootNodeId": "https://www.wikidata.org/wiki/Q1670679",
        "targetNodeId": "00000000-0000-0000-0000-000000000000",
        "targetNodeName": "Phase_2_3",
        "state": [
            {
                "type": "VRSimulation",
                "refinedBy": {
                    "type": "SimulationTime",
                    "sourceDate": "18:23:12",
                    "timeOfDay": "18:23:12"
                }
            },
            {
                "type": "InterpretiveReconstruction",
                "refinedBy": {}
            }
        ],
        "selector": {
                "type": "SceneGraphSelector",
                "value": "Stoa_komplett_low/Nordanbau/Phase_2_3"
        },
        "position": {
            "type":"AnnotationLocation",
            "AnnotationViewPoint": {"position":{"x":-1.8674207925796509,"y":0.6881608963012695,"z":11.212594032287598},"rotation":{"x":10.37573528289795,"y":227.96876525878907,"z":350.4180603027344},"type":"GeoCoordinates","coordinateSystem":"Unity.WorldSpace","latitude":37.97607442859378,"longitude":23.722392849200259,"elevation":0.6881608963012695},
            "AnnotationCoordinates" : {"position":{"x":-3.795814275741577,"y":0.04924760013818741,"z":9.872297286987305},"rotation":{"x":0.0,"y":0.0,"z":0.0},"type":"GeoCoordinates","coordinateSystem":"Unity.WorldSpace","latitude":37.97609476090733,"longitude":23.72240024216141,"elevation":0.04924760013818741}
        }
    }
}
```



# Other Requirements

- Model must not use “/” in GameObject names for Geometry-nodes inside NodeGraphs



# FAQ

## Does the it support the Oculus Rift?

Not yet, but a porting the InteractiveController-Script to the Rift should be very easy.

## Does it support other platforms like GearVR and HoloLens?

Currently the Interaction heavily relies on pointing and clicking with a controller. But adapting the interaction to triggering by gaze should be possible.

# Future work

We suggest the following steps to build on this ground-work

## Review and refine the JSON-ld schema

- Add proper @context-information
- Talk to other experts doing a similar thing.
- Find good IRIs for describing your buildings.
- Validate the context definition and RDF link structure with tools.
- Setup a database for interpretations (different 3d-models / Versions of a building) and use IRI to these models.
- document and publish the schema to encourage other database to use it.
- Clarify format for historic dates time.

## Expand the data models

Expand models for **Users** and **AnnotationSeries** to allow cross-referencing and filtering annotations.

## Implement contextual filters for annotations

Currently all annotations are always visible. Eventually these need to be filtered by things like: Tags, users, date-ranges, etc.

## Save additional information about the state of the VR-simulation

- examples would be time of day, weather conditions, year etc.

## Improve the representation of annotations in VR

Because we store the current user-view and perspective within an annotation we can use this data to visualize the perspective that the author of the annotation had when triggering the annotation.

## Improve picking and rendering of colliders

Although using world-space BoundingBoxes for hit testing has many advantages, they have many draw-backs:

- Concave meshes (like U or O-shaped walls) will block elements inside
- Nested elements can be hard to hit.
- The visualization might be misleading, distracting or covering important elements
- For rotated or round objects the visualization can be very off.

The NodeGraph-system could be easily extended by additional definition-nodes like “Mesh Colliders”, “Combined Boxes”, “Rotated Boxes” or other special solutions (e.g. for animated objects like avatars or NPGs). For well-balanced scene-graphs all of the before mentioned should be fast enough for realtime picking at 90fps. However, most of the require manual work on making the scene “annotatable”. Automatic solutions on finding good hit-testing can be very computing extensive and might require long pre-computation or caching.

## Add further input methods to make annotations

Examples could be:

- using tags
- using voice recognition
- using symbols or icons
- Sketching in VR

## Integrate an existing API or database(s)

## Export annotations into the real world

With technology like HoloLens and location-markers it is very feasible to show markers created in VR within their context. You can also port the components to work on the HoloLens to generate annotations.
