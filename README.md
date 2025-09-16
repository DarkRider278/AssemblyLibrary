# AssemblyLibrary

This repository contains controls + helper classes to work with WPF.

[!IMPORTANT]
 ***Disclaimer:
I developed the following functions/controls for my own projects, as I needed them.
Therefore, some functions may be limited or currently only suitable for specific use cases.***

[![GitHub License](https://img.shields.io/github/license/DarkRider278/AssemblyLibrary)](LICENSE.md)
[![Nuget Version](https://img.shields.io/nuget/v/AssemblyLibrary](https://www.nuget.org/packages/AssemblyLibrary)
![Nuget Downloads](https://img.shields.io/nuget/dt/AssemblyLibrary)

#GUIObj
User Controls + Dialogs

## Controls
### CustomTable
Customizable table based on stackpanel and base controls
**Available colum types:**
*Sorter
*Textbox
*Checkbox
*ComboBox
*Label

### DataBar
Control to save/load the content/state of controls in a file

Need to be placed on a Panel
**Supported Controls:**
*TextBox
*ComboBox
*RadioButton
*CustomTable

###PicSelector
Control to read all images(png) from a folder and display it in a selecable list

###RotationSelector
Control to selecz roation(at 45°)

###SpinnerInt / SpinnerDouble
Control to input numbers

###TimeCheckerGui
Control to setup a DateTime check for a specific Time/Day

###TimeLineControl   
Control to create a simple TimeLine(up to 8 channels)

##Dialogs
###CameraControl
Dialog to control postion and viewpoint of a virtual camera

###PropertyWindow
Simple dialog to show prperties

###TranslateWindow
Simple frontent for a dictionary

#Help Utilitis
Some helpful functions

##SerializeHelper
###SerializeDictonary / DeSerializeDictonary
Serialize/DeSerialize a Dictionary in/from xml string

###LoadXml / SaveXml
Load/Save an object as xml file

##StringHelpers
###GetSubString / GetSubStringIncludeStartEnd
Get a substring from a string with contain start and end 

###GetSubStrings / GetSubStringsRegEx
Get all substring from a string with contain start and end

###SplitbyChars / SplitbySpace
Split a string by given character

###EscapeStringJSon
Escape special character in a json string

###NormalizeEmptyString
Return default string value for empty strings

##TimeChecker
###IsValid
Check status of configurate time and day (Day of Week/ Hour + Minute)

###SetConfig / GetConfig
Set/Get config string for the TimeChecker

##Utility
###HSVtoRGB
Convert HSV values to RGB values

###GetPan
Calculate the pan value of 2 points

###GetTilt
Calculate the tilt value of 2 points

##VideoUtils
###EncodeFrames / DecodeFrames
Convert Frames to hour/minutes/seconds or vice versa

###FramesToTimeCode / TimeCodeToFrames
Convert frames to a byte timecode or vice versa

###FramesToStrTimeCode / StrTimeCodeToFrames
Formats frames to a string or vice versa

#TCPServerPlugin
Simple TCP Server

#TCPClientPlugin
Simple TCP Client


#UDPServerPlugin
Simple UDP Server

#UDPClientPlugin
Simple UDP Client

#CPLoader
Class to handle CP Files (simple textbase datebase)
