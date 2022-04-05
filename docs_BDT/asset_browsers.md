# Asset browsers

Each asset type, contain propertly assets browser. 
With navigation and filtering. You can, search, create, modify, copy or delete assets only in it.

![ScreenShot](Images/asset_browser/asset_browser.PNG)

```diff
! Important:                                                                                            
You dont move,rename, copy or delete assets, in unity assets herarchy.
All assets manipulation need to do with assets browser editors.
```
![ScreenShot](Images/asset_browser/asset_mod_cop.png)

### - Filtering and Navigation
Every assety type contains specific sorting posibilites besides default "Sort by ID" &  "Sort by name".
For example for NPC, you can sort it by occupation or for Factions you can sort it by Kingdom.

![ScreenShot](Images/asset_browser/sort_by.png)

You can search any prefab by selected Sort type

![ScreenShot](Images/asset_browser/filter_by_selected_sort.PNG)

You can filer any asset type, by it most important properties.

![ScreenShot](Images/asset_browser/asset_filers.PNG)

#### - You can select select stack of assets
##### - Shift + Click from (define action, select/deselect) + Click to
&nbsp;
![ScreenShot](Images/asset_browser/smart_selection.gif)

---------------------------------------------

### - Create Asset
You can create new assets/stacks, using this window

![ScreenShot](Images/asset_browser/create_0.PNG)

#### - Stack count
Number of created assets

#### - StacK ID
Stack start from specific count

#### - Use object reference
You can assign any object of asset type for use it as creation template.

#### - ID
New ID at creation.

---------------------------------------------

### - Modify Asset
You can modify selected assets/stacks, using this window

![ScreenShot](Images/asset_browser/modify_0.PNG)

### - Edit Mode
You can change ID for existed assets (no links replacement, [see here](tips.md#--properties-windows))

#### - Edit as Copy
Create new asset at modify, with new ID. 
Else, modify directly selected assets.

#### - Stack count
Number of created assets

#### - StacK ID
Stack start from specific count

### - Copy Mode
You can copy selected assets to any imported module.

![ScreenShot](Images/asset_browser/modify_1.PNG)

### - Remove Mode
You can remove selected assets from current module.

![ScreenShot](Images/asset_browser/modify_2.PNG)

---------------------------------------------
#### [Common Info -->](tips.md)
#### [<-- Translation Tools](translations.md)

#### [Main Page](/../..)
