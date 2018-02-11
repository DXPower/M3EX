Mach3Extender hooks into a running instance of Mach3 to allow for the automatic running of gcode. 

It uses .m3ex files (created by M3EXer), which are text files that contain serialized data of the toolpath gcode and other values that go along with it, such as material size, tool offsets, speed, etc. 

In order to work, both Mach3 and Mach3Extender must be running in Administrator mode. Additionally, some changes must be made to the registry for Mach3Extender to work due to Mach3 not setting some runtime variables correctly (these changes are handled by M3EX Installer; if you want to run the binary without installing, you must do these registry yourself using the regfix.reg file). Mach3 must be running before you start Mach3Extender.
