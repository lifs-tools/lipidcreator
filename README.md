# LipidCreator #
[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.3529484.svg)](https://doi.org/10.5281/zenodo.3529484)

LipidCreator is a plugin for [Skyline](https://skyline.ms/project/home/software/Skyline/begin.view) supporting targeted workflow development in lipidomics.
It can be used to create user-defined target lists and fragment libraries for PRM and MRM experiments in Skyline.
It also supports standalone and command-line operation.

It has been tested with Thermo QExactive HF and Waters QTOF instruments.

## Latest Release ##
The LipidCreator releases are available from the [LIFS Portal](https://lifs.isas.de/lipidcreator) and from the [Skyline Tool Store](https://skyline.ms/skyts/home/software/Skyline/tools/details.view?name=LipidCreator).

Source releases are available from the [Github release page](https://github.com/lifs-tools/lipidcreator/releases).

**Please see the release specific page first for instructions on how to install LipidCreator.**

## Installation and Usage ##

### Windows ###

#### With Skyline ####
LipidCreator has been tested with [Skyline 20.2, 64 bit](https://skyline.ms/project/home/software/Skyline/begin.view). Please follow the installation instructions on the Skyline homepage if you do not have Skyline installed already.

#### Installation from the Skyline Tool Store ####
The preferred way to install the latest release versions of LipidCreator is to install it from the Skyline Tool Store:

1. Open Skyline with a blank document
2. Go to the "Tools" &gt; "Tool Store..." menu
3. Scroll down and select "LipidCreator" for installation
4. Wait for the installation to complete
5. LipidCreator can now be started via "Tools" &gt; "LipidCreator"

#### Manual Installation from ZIP ####
You can install the LipidCreator zip file (name must be LipidCreator.zip), e.g. for testing purposes, as an external tool:

1. Go to "Tools" &gt; "External Tools" and click on "Add"
2. Select the LipidCreator.zip file
3. Wait for the installation to complete
4. Click "OK" on the "External Tools" dialog.
5. LipidCreator can now be started via "Tools" &gt; "LipidCreator"

#### Stand-Alone ####
If you locate the LipidCreator.exe file, either in your Skyline installation under the "Tools" folder, or within the LipidCreator folder extracted from the zip archive, you need to double click on it to start in stand-alone mode. It is also possible to start LipidCreator from the command line.

### Linux / Ubuntu ###
To run LipidCreator, locate the directory containing LipidCreator.exe, open a terminal and type

    mono LipidCreator.exe

Please note that there may be issues with the repainting of certain windows and controls (scrollable areas) due to the not 100% compatible Mono implementation.

## Building from Source ##

LipidCreator has been written in the C# programming language but can be compiled and run using the Mono framework on Linux (Ubuntu 18.04 tested) and MacOS (untested) as well.

We currently build against .NET framework v4.5.2, but any later version should also work.

### Windows ###

For development on Windows, we recommend to install [VisualStudio Community Edition](https://visualstudio.microsoft.com/vs/community/), 
or any of the other VisualStudio editions. Please note that VisualStudio Code currently does not have great support for UI development.

You also need to install the Windows Git client for the build to work [Git for Windows](https://git-scm.com/download/win). Select that Git should be added to the PATH during the installation.
After installation of the Git client, you need to restart VisualStudio so that Git becomes available via the PATH.

To build and/or run the project, simply select the appropriate menu entry in VisualStudio.

### Linux / Ubuntu ###

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

In order to use the blib spectral library export functionality, please install the appropriate sqlite (>3) package for your distribution.
For Debian and Ubuntu, the following command installs them:

    sudo apt install sqlite3 

We ship a precompiled native sqLite.Interop.so library with LipidCreator for Debian and Ubuntu. 
If that fails to work under your Linux distribution, please follow these steps to build a custom one:

1. Install a GCC compilation toolchain, on Ubuntu: `sudo apt-get install build-essential`
2. Download https://system.data.sqlite.org/downloads/1.0.111.0/sqlite-netFx-source-1.0.111.0.zip
3. Unzip the downloaded zip archive and change into the `/Setup` folder
4. Run `bash compile-interop-assembly-release.sh`
5. Copy `../bin/2013/Release/bin/libSQLite.Interop.so` into the directory containing LipidCreator.exe
6. Try to export a blib file (after activating the collision energy module) from the lipid review dialog after adding some transitions.

Please note: we have developed and tested LipidCreator using Mono under Ubuntu 16.04 and 18.04 and Debian 10 (Buster). 
Other Linux distributions should also work, but were not tested. If you encounter any issues, please let us know!

## Reporting issues ##
If you encounter any issues with LipidCreator, please report them via https://lifs.isas.de/support, using the 'Support category' LipidCreator.
