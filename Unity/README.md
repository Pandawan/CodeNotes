# Research on Unity/Game Dev Systems

## Table of Contents

- [Inventory Systems](#Inventory-Systems)
- [2D Game Rendering](#2D-Game-Rendering)

## Inventory Systems

### Item Setup with Scriptable Objects

If you want to make the invetory system with Scriptable Object, look at this system.

![Image](ModularItemSo.png)

### Scriptable Object With Modular Effects/Behavior

This tip works regardless of the actual inventory system used. As long as it uses Scriptable Objects/some form of Item reference storage. It allow for items to be more modular (not need a new class for every specific item type).  

Make items have a List/Array of "Behaviors," which are identified by an ID (string/enum).  
When equipped, specific MonoBehavior/Components can search for them and see if they have the correct behavior to do a specific action.

Resources:

- [See this GIF showing the modular system in action](https://streamable.com/926i5)

### If No Need For Varying Items

If there is no need for items to change during runtime (durability, item health...), then use this system.

Best way to set this up is to have Scriptable Objects Item with ID database.  
Basically, each `Item` is a Scriptable Object created in Unity.  
The Inventory is a `List<string>` or `List<int>`, which means it does not directly hold the item but a reference to the ScriptableObject/Prefab.  
Then you have a `Database` which is a singleton/prefab with a `Dictionary` (in Inspector) which has references to those Scriptable Objects.  
Then when you need an item from inventory, (Add or Get) you just pass the ID and then fetch from `Database`.  

### If Need Varying Items

If there is a need for items to change during runtime (durability, item health...), then use this system.

Have two Item types:

- `ItemSO`: Scriptable Object version of the Item, used for modification/creation. This has all the normal values + initial/max durability (not current). (This can also be done in JSON or any other format).
- `Item`: A simple Object which is the one being manipulated at runtime. It also has values for current durability/health and other varying values...

The Inventory stores a `List<Item>` which allows for passing around of `Item`s with durability/varying values. Add/Get returns the actual `Item`.  
There is a `Database`, which is a singleton/prefab with a `Dictionary` (in Inspector) with references to `ItemSO`s and their string/int ID.  
Then when you want to CREATE a new Item, you call `Database`'s CreateItem method with a string/int ID. The Database will fetch all the values from the `ItemSO` and copy them to a new `Item` object which is returned.  

## 2D Game Rendering

### 2D Pixel Per Unit/Scale

Decide on a scale for the entire game (e.g. 16x16), and make every object use 16, then the Pixel Per Unit is 16 as well.  
This makes it so that every 16x16 object takes 1 full unit. And allows every pixel to always be the same size on screen.  
You can also use the Pixel Perfect Camera package from Unity to make the view work better with pixel-art.  

Resources:

- [Good guide on how Pixel Per Unit should be scaled](https://gamedev.stackexchange.com/questions/141609/understanding-pixel-art-in-unity-2d)

### 2D Shadows/Lighting System

Resources:

- [Examples of cool 2D dynamic lighting](https://www.youtube.com/watch?v=WWdGdE8ZwIA)
- [Another, even better, example of 2D dynamic lighting, Stardew-Valley-like](https://twitter.com/lazybeargames/status/967071011689648130)
- [Tutorial on a basic 2D lighting system using Normal Maps](https://www.youtube.com/watch?v=fwyAoE_uMFo)
  - NOTE: The above might be a bit limited, creating a custom shader might result in better results. But normal maps are definitely a must have!
- [Forum post on how to make Stardew-Valley-like lighting/shadows](https://forum.unity.com/threads/unity-2d-lightning-casting-chadows.474663/)

### 2D Sorting

2D Sorting is very complicated. It's how you make sprites render in front of others based on position/other factors so that it looks somewhat more realistic/3D.

How to do 2D Sorting:

- In Project Settings > Graphics, set Transparency Sort Mode to Custom Axis, and set the Axis to (0, 1, 0)
- Create 4 Sorting Layers, Foreground > Objects > Shadows > Background
- Explanation:
  - Foreground is for effects and things that are always on top
  - Objects is for anything that the player can be above/below (like walls and players)
  - Shadows is shadows
  - Background is JUST the ground or things that are ALWAYS rendered last/below



**EDIT**: To make sorting work better, you also need to set this up for GameObjects that the player can go behind/in front of:

- In Image Inspector/Import Settings, set Pivot to Bottom (or X: 0.5, Y: 0/negative value*)
  - \* Use a negative value when the object is floating in the air, so that the threshold is lower than sprite.
- In Sprite Renderer, set Sprite Sort Point to Pivot. This will make it so that when the feet of the player get past the item/block, it renders behind, rather than the middle

**EDIT 2**: Using this sorting technique (with Texture pivot from Edit 1) has some issues which can be corrected with additional methods

- This texture pivot will cause the `transform.position` value to be offset (at the position of the texture pivot rather than center of the texture)
- As a result, you need to make a new method `CenterPosition()`
- `CenterPosition()` returns -> `transform.position + GetComponent<SpriteRenderer>().sprite.bounds.center.normalized`
- Make sure this is a method and not a variable refreshed with `Update` because `Update` might be too fast when used with a `FixedUpdate`

Resources:

- [How Enter The Gungeon uses 3D to render a 2D game](https://www.reddit.com/r/EnterTheGungeon/comments/4sed7a/developers_how_did_you_get_into_programming_and/d5bfob1/)
- [A lot of info about how Enter the Gungeon works](https://www.reddit.com/r/Unity2D/comments/80x090/how_did_enter_the_gungeon_build_their_environments/)
- [Perhapse useful link about y-based sorting](https://www.reddit.com/r/Unity2D/comments/2qoh1n/topdownisometric_y_based_depth_sorting/)
- [Another useful link with y-based sorting with GitHub examples](https://github.com/creativitRy/TilemapHeightTest)
