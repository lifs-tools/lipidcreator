CC = x86_64-w64-mingw32-g++
SQLite_DLL = ../packages/System.Data.SQLite.Core.1.0.110.0/lib/net40/System.Data.SQLite.dll
Log4Net_DLL = ../packages/log4net.2.0.8/lib/net40-client/log4net.dll

CORE_FILES = LipidCreator.cs FattyAcid.cs FattyAcidGroup.cs MS2Fragment.cs Lipid.cs Glycerolipid.cs Phospholipid.cs Sphingolipid.cs Cholesterol.cs Mediator.cs Precursor.cs CollisionEnergy.cs BaseParserEventHandler.cs LipidMapsParserEventHandler.cs IonFormulaParserEventHandler.cs MoleculeFormulaParserEventHandler.cs ParserEventHandler.cs Parser.cs LipidCreatorExtensionMethods.cs

GUI_FILES = CreatorGUI.cs CreatorGUI.Designer.cs MS2Form.cs MS2Form.Designer.cs LipidsReview.cs LipidsReview.Designer.cs NewMediatorFragment.cs NewMediatorFragment.Designer.cs NewFragment.cs NewFragment.Designer.cs SpectralName.cs SpectralName.Designer.cs AboutDialog.cs AboutDialog.Designer.cs AssemblyInfo.cs MediatorMS2Form.cs MediatorMS2Form.Designer.cs AddHeavyPrecursor.cs AddHeavyPrecursor.Designer.cs Tutorial.cs FilterDialog.cs FilterDialog.Designer.cs TranslatorDialog.cs TranslatorDialog.Designer.cs CEInspector.cs CEInspector.Designer.cs LCMessageBox.cs LCMessageBox.Designer.cs LipidsInterList.cs LipidsInterList.Designer.cs

DIR = ${CURDIR}
BUILD_DIR = $(DIR)/bin/make

RESX_FILES = AboutDialog.resx,$(BUILD_DIR)/LipidCreator.AboutDialog.resources AddHeavyPrecursor.resx,$(BUILD_DIR)/LipidCreator.AddHeavyPrecursor.resources CEInspector.resx,$(BUILD_DIR)/LipidCreator.CEInspector.resources FilterDialog.resx,$(BUILD_DIR)/LipidCreator.FilterDialog.resources LCMessageBox.resx,$(BUILD_DIR)/LipidCreator.LCMessageBox.resources LipidsReview.resx,$(BUILD_DIR)/LipidCreator.LipidsReview.resources MS2Form.resx,$(BUILD_DIR)/LipidCreator.MS2Form.resources NewFragment.resx,$(BUILD_DIR)/LipidCreator.NewFragment.resources NewMediatorFragment.resx,$(BUILD_DIR)/LipidCreator.NewMediator.resources TranslatorDialog.resx,$(BUILD_DIR)/LipidCreator.TranslatorDialog.resources LipidsInterList.resx,$(BUILD_DIR)/LipidCreator.LipidsInterList.resources

RES_FILES = -resource:$(BUILD_DIR)/LipidCreator.AboutDialog.resources -resource:$(BUILD_DIR)/LipidCreator.AddHeavyPrecursor.resources -resource:$(BUILD_DIR)/LipidCreator.CEInspector.resources -resource:$(BUILD_DIR)/LipidCreator.FilterDialog.resources -resource:$(BUILD_DIR)/LipidCreator.LCMessageBox.resources -resource:$(BUILD_DIR)/LipidCreator.LipidsReview.resources -resource:$(BUILD_DIR)/LipidCreator.MS2Form.resources -resource:$(BUILD_DIR)/LipidCreator.NewFragment.resources -resource:$(BUILD_DIR)/LipidCreator.NewMediator.resources -resource:$(BUILD_DIR)/LipidCreator.TranslatorDialog.resources -resource:$(BUILD_DIR)/LipidCreator.LipidsInterList.resources





main: setup
	resgen2 -compile $(RESX_FILES)

	mcs $(RES_FILES) -pkg:dotnet -platform:x64 -sdk:4.5 -r:$(Log4Net_DLL) -r:System.Xml.Linq.dll -r:SkylineTool.dll -r:Ionic.Zip.dll -r:$(SQLite_DLL) -lib:/usr/lib/mono/4.5 MainWindow.cs $(GUI_FILES) $(CORE_FILES) -out:$(BUILD_DIR)/LipidCreator.exe

tests: setup
	mcs -pkg:dotnet -r:$(Log4Net_DLL) -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:$(SQLite_DLL) -lib:/usr/lib/mono/4.5 $(CORE_FILES) TestTransitionList.cs -out:$(BUILD_DIR)/TestTransitionList.exe
	
	mcs -pkg:dotnet -r:$(Log4Net_DLL) -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:$(SQLite_DLL) -lib:/usr/lib/mono/4.5 $(CORE_FILES) TestParserEventHandler.cs TestParser.cs -out:$(BUILD_DIR)/TestParser.exe
	
	mcs -pkg:dotnet -r:$(Log4Net_DLL) -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:$(SQLite_DLL) -lib:/usr/lib/mono/4.5 $(CORE_FILES) TestLipidMaps.cs -out:$(BUILD_DIR)/TestLipidMaps.exe
	
	mcs -pkg:dotnet -r:$(Log4Net_DLL) -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:$(SQLite_DLL) -lib:/usr/lib/mono/4.5 $(CORE_FILES) $(GUI_FILES) TestTutorials.cs -out:$(BUILD_DIR)/TestTutorials.exe

complete:
	$(MAKE) setup
	$(MAKE) main
	$(MAKE) zip

zip:
	cd $(BUILD_DIR) && zip -r ../../LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll log4net.dll log4net.xml tool-inf
	
	
setup:  $(SQLite_DLL)
	if [ ! -d "$(BUILD_DIR)" ];then \
		mkdir -p bin; \
		mkdir -p $(BUILD_DIR); \
		cp -r data images SkylineTool.dll Ionic.Zip.dll $(SQLite_DLL) ../packages/log4net.2.0.8/lib/net40-client/* tool-inf $(BUILD_DIR); \
		echo -1 > $(BUILD_DIR)/data/analytics.txt; \
	fi
	mkdir -p $(BUILD_DIR)/data/predefined
	

clean:
	rm -rf bin && rm -rf obj/

run:
	cd $(BUILD_DIR) && mono LipidCreator.exe &
	
runtest:
	mono $(BUILD_DIR)/TestTransitionList.exe
	mono $(BUILD_DIR)/TestParser.exe
	mono $(BUILD_DIR)/TestLipidMaps.exe
