
============================== MIT License ===================================

Copyright (c) 2018 Dominik Kopczynski   -   dominik.kopczynski {at} isas.de
                   Bing Peng   -   bing.peng {at} isas.de
                   Nils Hoffmann  -  nils.hoffmann {at} isas.de

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.



============================== Installation ==================================

System Requirements:
 - either Windows Vista / 7 / 8 / 10 (only 64 bit) with .NET framwork 4.5.1 or higher [1]
 - or Ubuntu 16.04 or higher* (with 'mono-complete' package installed [2], only 64 bit)
 - at least 256MB main memory or higher
 
* LipidCreator has been tested on Ubuntu 16.04 and 18.04, as well as on 
  Debian 10 (Buster). Other Linux distributions *should* also work, but have not 
  been tested.

Note: If you install Skyline first, all necessary .NET framework libraries will be installed
automatically! Later versions of Windows 8 and 10 already ship with up-to-date versions of .NET
installed. These also do not require additional installation.

Installation for standalone mode:
No installation required, please click on 'LipidCreator.exe' to run the software.

Installation as external program in Skyline [3]:
 1) Open Skyline with a blank document
 2) Go in the menu on 'Tools' -> 'External Tools...'
 3) Click on 'Add' -> 'From File'
 4) Browse to the 'LipidCreator.zip' archive and select it
 5) Confirm your selection two times
 6) To run LipidCreator, go in the menu on 'Tools' -> 'LipidCreator'

[1] https://dotnet.microsoft.com/download/dotnet-framework/net452
[2] https://www.mono-project.com/download/stable 
[3] https://skyline.ms


============================== Demo ========================================
 
LipidCreator provides several predefined data sets for a number of organisms
and tissues.
 1) Start LipidCreator
 2) Click in the menu on 'File' -> 'Import Predefined' -> 'Yeast' -> 'Lipidome'
 3) After loading, click on 'Review Lipids'. The new window shows 406 precursors.
 4) Click on 'Continue' to get the final list containing 5268 transitions for the
    yeast lipidome which can be used / stored for further method development.
This demo can be performed in few seconds.


=============================== Tutorials ====================================

After LipidCreator has been started, four interactive tutorials can be started
from the 'Home' tab, namely:
 - PRM tutorial
 - SRM tutorial
 - heavy labeled isotope tutorial
 - collision energy tutorial

Additionally, a text version (pdf) is available in LipidCreator, please click
on 'Help' -> 'Documentation' within LipidCreator's menu.

============================ Help & Support ==================================

If you experience any issues with LipidCreator, we kindly ask you to report them
using our support form at https://lifs.isas.de/support with the support category
'Lipid Creator'. Please include your Operating System, Skyline version (if 
applicable), and the full LipidCreator version, which is available from the 
'Help' -> 'About' menu dialog.
