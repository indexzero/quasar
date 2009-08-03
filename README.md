### Quasar
---

#### Author: Charlie Robbins, 2008
[http://www.charlierobbins.com](http://www.charlierobbins.com)

Last Update: January 19th, 2008

### Requirements
   Depends on the SilverlightToolkit and is currently using the December 2008 drop.

### Quality Bands
   Currently all controls in Quasar are in the pre-Alpha stages of development. I have tried to outline all of the limitations in "Limitations below."

   1. pre-Alpha: There are no unit tests, sample usage in Quasar.Samples, or other documentation.
   2. Alpha: There is sample usage in Quasar.Samples and limited documentation 
   3. Beta: There are unit tests, sample usage in Quasar.Samples, and limited documentation
   4. Released: Ready for prime time.

### Dependencies  
   1. Built against Silverlight 2 RTM
   2. Silverlight Toolkit, December 2008

### Limitations
   1. AttachedDragDropBehavior
	* EventHandlers cannot be set in ControlTemplates
	* EventHandlers must be public methods on whatever class they are declared
   2. AttachedEventHandlerManager
	* EventHandlers cannot be set in ControlTemplates
	* EventHandlers must be public methods on whatever class they are declared
   3. ImageListView
        * The ImageListViewItem does not utilize any sort of bounding (i.e. ViewBox), so there may be possible clipping in some Stretch usage scenarios. 
   4. LightBox
        * Currently has no "Adorner" like layer built into it, so to get the faded background effect, you must have it stretch to the entire area. 

### Credits
   * ItemContainerManager: Josh Wagoner (http://blog.dobaginski.com/josh/?p=52)
   * AttachedEventHandlerManager: Ary Borenszweig (http://weblogs.manas.com.ar/ary/2008/06/24/adding-double-click-support-in-silverlight/)
