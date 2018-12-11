CC = x86_64-w64-mingw32-g++
CORE_FILES = LipidCreator.cs FattyAcid.cs FattyAcidGroup.cs MS2Fragment.cs Lipid.cs GLLipid.cs Phospholipid.cs SLLipid.cs Cholesterol.cs Mediator.cs Precursor.cs CollisionEnergy.cs BaseParserEventHandler.cs LipidMapsParserEventHandler.cs ParserEventHandler.cs Parser.cs

GUI_FILES = CreatorGUI.cs CreatorGUI.Designer.cs MS2Form.cs MS2Form.Designer.cs LipidsReview.cs LipidsReview.Designer.cs NewMediatorFragment.cs NewMediatorFragment.Designer.cs NewFragment.cs NewFragment.Designer.cs SpectralName.cs SpectralName.Designer.cs AboutDialog.cs AboutDialog.Designer.cs AssemblyInfo.cs MediatorMS2Form.cs MediatorMS2Form.Designer.cs AddHeavyPrecursor.cs AddHeavyPrecursor.Designer.cs Tutorial.cs FilterDialog.cs FilterDialog.Designer.cs TranslatorDialog.cs TranslatorDialog.Designer.cs CEInspector.cs CEInspector.Designer.cs LCMessageBox.cs LCMessageBox.Designer.cs

main: setup
	mcs -pkg:dotnet -r:../packages/log4net.2.0.8/lib/net45-full/log4net.dll -r:System.Xml.Linq.dll -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 $(GUI_FILES) $(CORE_FILES) -out:bin/make/LipidCreator.exe

tests: setup
	mcs -pkg:dotnet -r:../packages/log4net.2.0.8/lib/net45-full/log4net.dll -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 $(CORE_FILES) TestTransitionList.cs -out:bin/make/TestTransitionList.exe
	
	mcs -pkg:dotnet -r:../packages/log4net.2.0.8/lib/net45-full/log4net.dll -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 $(CORE_FILES) TestParserEventHandler.cs TestParser.cs -out:bin/make/TestParser.exe
	
	mcs -pkg:dotnet -r:../packages/log4net.2.0.8/lib/net45-full/log4net.dll -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 $(CORE_FILES) TestLipidMaps.cs -out:bin/make/TestLipidMaps.exe

complete:
	$(MAKE) setup
	$(MAKE) main
	$(MAKE) predefined
	$(MAKE) zip

zip:
	cd bin/make && zip -r ../../LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll log4net.dll log4net.xml tool-inf
	
setup: 
	if [ ! -d "bin/make" ];then \
		mkdir -p bin; \
		mkdir -p bin/make; \
		cp -r data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll ../packages/log4net.2.0.8/lib/net45-full/log4net.dll ../packages/log4net.2.0.8/lib/net45-full/log4net.xml tool-inf bin/make; \
		echo -1 > bin/make/data/analytics.txt; \
	fi
	
	
predefined:
	mono bin/make/LipidCreator.exe transitionlist bin/make/data/predefined-lists/human-platelet.csv data/predefined/Human/Platelet.lcXML -l -p 0 -h 0
	mono bin/make/LipidCreator.exe transitionlist bin/make/data/predefined-lists/mouse-platelet.csv data/predefined/Mouse/Platelet.lcXML -l -p 0 -h 0
	mono bin/make/LipidCreator.exe transitionlist bin/make/data/predefined-lists/mouse-brain.csv data/predefined/Mouse/Brain.lcXML -l -p 0 -h 0
	mono bin/make/LipidCreator.exe transitionlist bin/make/data/predefined-lists/yeast-lipidome.csv data/predefined/Yeast/Lipidome.lcXML -l -p 0 -h 0

clean:
	rm -r bin && rm -r obj/

run:
	mono bin/make/LipidCreator.exe &
	
runtest:
	mono bin/make/TestTransitionList.exe
	mono bin/make/TestParser.exe
	mono bin/make/TestLipidMaps.exe
