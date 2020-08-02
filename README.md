# Yapped
An editor for Dark Souls 3 and Sekiro param files, which determine the properties of items, attacks, effects, enemies, objects, and a whole lot more. For detailed instructions, please refer to the included readme.  
Requires [.NET 4.7.2](https://www.microsoft.com/net/download/thank-you/net472) - Windows 10 users should already have this.  
[Nexus Page](https://www.nexusmods.com/darksouls3/mods/298)  

## Note
This is a fork of [Yapped by JKAnderson](https://github.com/JKAnderson/Yapped). Its features diverge.

This fork is not adequately supported or tested, but is for my own use. Back up your data and use at your own risk!

# Warning
As far as we know, in DS3 *any* edits to the regulation file (where params are stored) will trigger anticheat, including simply opening it and resaving it.  
Only use modified params in offline mode. Back up your save file and restore it before going online again if you're doing anything that could affect it.  

# Changelog
### Forked
* Grids replaced with a more responsive custom implementation but with cruder editing facilities
* Values which differ from the (customary) default are highlighted
* The new Tools menu has an option to calculate attack rating for all weapons for given stats

### 1.1.2
* Beta for Sekiro support

### 1.1.1
* Fix name in create row dialog not doing anything
* Fix duplicated rows being unsaveable sometimes

### 1.1
* Ctrl+Shift+N: Duplicate selected row
* Ctrl+F: Search for row by name
* Ctrl+G: Go to row by ID
* Ctrl+Shift+F: Search for field by name
* Unused params are hidden by default
* Creating a new row has a nice dialog now
* Updated layouts for several params (thanks Pav)
* Updated names for several params (thanks GiveMeThePowa and Xylozi)
* Added brief descriptions for params on mouse-over (thanks Pav)
* Added support for field descriptions, but didn't actually write any yet

### 1.0.2
* Locales that use a comma for the decimal point are now supported
* Selected row and visible cells are now remembered for each param separately

### 1.0.1
* Backup actually works now. If you've already modified something, verify your game files through Steam. Sorry!

# Credits
**Pav** - Layouts  
**TKGP** - Application  
**GiveMeThePowa, Xylozi** - Contributing row names

# Libraries
[Octokit](https://github.com/octokit/octokit.net) by GitHub

[Semver](https://github.com/maxhauser/semver) by Max Hauser

[SoulsFormats](https://github.com/JKAnderson/SoulsFormats) by Me