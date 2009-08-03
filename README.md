Quasar 0.1 (Released January 19th, 2008)

1. Requirements
   a. Depends on the SilverlightToolkit and is currently using the December 2008 drop.

2. Quality Bands
   Currently all controls in Quasar are in the pre-Alpha stages of development. I have tried to outline all of the limitations in "Limitations below."

   a. pre-Alpha: There are no unit tests, sample usage in Quasar.Samples, or other documentation.
   b. Alpha: There is sample usage in Quasar.Samples and limited documentation 
   c. Beta: There are unit tests, sample usage in Quasar.Samples, and limited documentation
   d. Released: Ready for prime time.

3. Dependencies
   a. Built against Silverlight 2 RTM
   b. Silverlight Toolkit, December 2008

4. Limitations
   a. AttachedDragDropBehavior
	+ EventHandlers cannot be set in ControlTemplates
	+ EventHandlers must be public methods on whatever class they are declared
   b. AttachedEventHandlerManager
	+ EventHandlers cannot be set in ControlTemplates
	+ EventHandlers must be public methods on whatever class they are declared
   c. ImageListView
        + The ImageListViewItem does not utilize any sort of bounding (i.e. ViewBox), so there may be possible clipping in some Stretch usage scenarios. 
   d. LightBox
        + Currently has no "Adorner" like layer built into it, so to get the faded background effect, you must have it stretch to the entire area. 
   e. 

5. Credits
   +ItemContainerManager: Josh Wagoner (http://blog.dobaginski.com/josh/?p=52)
   +AttachedEventHandlerManager: Ary Borenszweig (http://weblogs.manas.com.ar/ary/2008/06/24/adding-double-click-support-in-silverlight/)
