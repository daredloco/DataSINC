# DataSINC
 Data Editor for [Software INC](https://softwareinc.coredumping.com/)
 
 With this handy tool you can create your own Datamods with ease. You can edit CompanyTypes, SoftwareTypes, Personalities and NameGenerators with just a few clicks and without the need to "code". The mod will be created automatically for you, no additional work outside of the editor is needed!
 
## Important Information

This project is at a very early stage and isn't usable.
No guarantee for any losses or such, it will ignore variables it doesn't know and would most likely **break** your mod!

**Always** make a backup of your mod before working on it in the Editor!

#### Usable parts

* CompanyTypes
* SoftwareTypes (Only loading)
* Personalities
* NameGenerators
* New/Load/Save functions
* Links to SINC Wiki for better understanding of modding and this Github repository

#### Not included

* Hardware mods (Do not load hardware mods with the Editor as the hardware parts **WILL** be cut out from the tyd file!) - They will be added within the first major update after release

## Usage
TODO

## Licenses

DataSINC is released under the MIT License

TyDSharp is forked from https://github.com/khornel/TyDSharp which was released under the MIT License. The version used in the Editor is a **customized** version and is not the same as in the fork!
Changes were mostly made in the TydConverter.cs file and the Editor would crash if you use TydSharp for it!
