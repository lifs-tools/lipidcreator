# LipidCreator #

LipidCreator is a plugin for Skyline supporting targeted workflow development in lipidomics.
It can be used to create user-defined target lists and fragment libraries for PRM and MRM experiments in Skyline.
It also supports standalone and command-line operation.

It has been tested with Thermo QExactive HF and Waters QTof instruments.

## Installation ##

LipidCreator has been written in the C# programming language but can be compiled and run using the Mono framework on Linux (Ubuntu 18.04 tested) and MacOS (untested) as well.

We currently build against .NET framework v4.5.2, but any later version should also work.

### Windows ###

For development on Windows, we recommend to install [VisualStudio Community Edition](https://visualstudio.microsoft.com/vs/community/), 
or any of the other VisualStudio editions. Please note that VisualStudio Code currently does not have great support for UI development.

You also need to install the Windows Git client for the build to work [Git for Windows](https://git-scm.com/download/win). Select that Git should be added to the PATH during the installation.
After installation of the Git client, you need to restart VisualStudio so that Git becomes available via the PATH.

To build and/or run the project, simply select the appropriate menu entry in VisualStudio.

### Ubuntu ###

LipidCreator requires the Mono development and runtime libraries. 
Please follow the official guidelines to install the [latest stable Mono](https://www.mono-project.com/download/stable/).

For Ubuntu, we recommend MonoDevelop as an integrated development environment. 
Please follow the official guidelines to install [MonoDevelop](https://www.monodevelop.com/download/linux/).

In MonoDevelop, simply open the LipidCreator.sln file to import the project.

To build and/or run the project, select the appropriate menu entry in MonoDevelop.

To build the project from the command line, you will need the msbuild program installed. It comes with the mono libraries, so it should already be available.

    msbuild LipidCreator.sln
    
will build the default Debug-enabled version of LipidCreator with output below `bin/Debug`. To build the release optimized version, run

    msbuild LipidCreator.sln /p:Configuration=Release /p:Platform=x64

which will produce output in `bin/Release`. 

## Usage ##

### Windows ###

#### With Skyline ####
You can install the LipidCreator zip file (name must be LipidCreator.zip) as an external tool. Go to "Tools" &gt; "External Tools" and click on "Add". Select the LipidCreator.zip file and wait until 
installation completes. Click "OK" on the "External Tools" dialog. LipidCreator can now be started via "Tools" &gt; "LipidCreator".

#### Stand-Alone ####
If you locate the LipidCreator.exe file, either in your Skyline installation under the "Tools" folder, or within the LipidCreator folder extracted from the zip archive, you need to double click on it to start in stand-alone mode. It is also possible to start LipidCreator from the command line.

### Ubuntu ###
To run LipidCreator, locate the directory containing LipidCreator.exe, open a terminal and type

    mono LipidCreator.exe

Please note that there are sometimes issues with the repainting of certain windows and controls (scrollable areas) due to the not 100% compatible Mono implementation.
