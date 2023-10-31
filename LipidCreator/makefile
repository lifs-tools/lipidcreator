CC = mcs
SQLite_DLL = ../packages/System.Data.SQLite.Core.1.0.110.0/lib/net40/System.Data.SQLite.dll
Log4Net_DLL = ../packages/log4net.2.0.12/lib/net40-client/log4net.dll
LIBS = -r:$(Log4Net_DLL) -r:System.Xml.Linq.dll -r:csgoslin.dll -r:System.Text.Json.dll -r:System.Net.Http.dll -r:SkylineTool.dll -r:Ionic.Zip.dll -r:$(SQLite_DLL) -r:ExcelLibrary.dll 
DLLS = $(Log4Net_DLL) System.Xml.Linq.dll System.Text.Json.dll csgoslin.dll SkylineTool.dll Ionic.Zip.dll $(SQLite_DLL) ExcelLibrary.dll 

CORE_FILES = LipidCreator.cs FattyAcid.cs FattyAcidGroup.cs MS2Fragment.cs Lipid.cs Glycerolipid.cs Phospholipid.cs Sphingolipid.cs Sterol.cs Mediator.cs Precursor.cs CollisionEnergy.cs BaseParserEventHandler.cs IonFormulaParserEventHandler.cs MoleculeFormulaParserEventHandler.cs Parser.cs LipidCreatorExtensionMethods.cs Element.cs ListingParserEventHandler.cs FunctionalGroup.cs Adduct.cs IsotopeParserEventHandler.cs CrossPlatform.cs

GUI_FILES = CreatorGUI.cs CreatorGUI.Designer.cs MS2Form.cs MS2Form.Designer.cs LipidsReview.cs LipidsReview.Designer.cs NewMediatorFragment.cs NewMediatorFragment.Designer.cs NewFragment.cs NewFragment.Designer.cs SpectralName.cs SpectralName.Designer.cs AboutDialog.cs AboutDialog.Designer.cs AssemblyInfo.cs MediatorMS2Form.cs MediatorMS2Form.Designer.cs AddHeavyPrecursor.cs AddHeavyPrecursor.Designer.cs Tutorial.cs FilterDialog.cs FilterDialog.Designer.cs TranslatorDialog.cs TranslatorDialog.Designer.cs CEInspector.cs CEInspector.Designer.cs LCMessageBox.cs LCMessageBox.Designer.cs LipidsInterList.cs LipidsInterList.Designer.cs Wizard.cs Wizard.Designer.cs

DIR = ${CURDIR}
BUILD_DIR = $(DIR)/bin/make

RESX_FILES = AboutDialog.resx,$(BUILD_DIR)/LipidCreator.AboutDialog.resources AddHeavyPrecursor.resx,$(BUILD_DIR)/LipidCreator.AddHeavyPrecursor.resources CEInspector.resx,$(BUILD_DIR)/LipidCreator.CEInspector.resources FilterDialog.resx,$(BUILD_DIR)/LipidCreator.FilterDialog.resources LCMessageBox.resx,$(BUILD_DIR)/LipidCreator.LCMessageBox.resources LipidsReview.resx,$(BUILD_DIR)/LipidCreator.LipidsReview.resources MS2Form.resx,$(BUILD_DIR)/LipidCreator.MS2Form.resources NewFragment.resx,$(BUILD_DIR)/LipidCreator.NewFragment.resources NewMediatorFragment.resx,$(BUILD_DIR)/LipidCreator.NewMediator.resources TranslatorDialog.resx,$(BUILD_DIR)/LipidCreator.TranslatorDialog.resources LipidsInterList.resx,$(BUILD_DIR)/LipidCreator.LipidsInterList.resources Wizard.resx,$(BUILD_DIR)/LipidCreator.Wizard.resources

RES_FILES = -resource:$(BUILD_DIR)/LipidCreator.AboutDialog.resources -resource:$(BUILD_DIR)/LipidCreator.AddHeavyPrecursor.resources -resource:$(BUILD_DIR)/LipidCreator.CEInspector.resources -resource:$(BUILD_DIR)/LipidCreator.FilterDialog.resources -resource:$(BUILD_DIR)/LipidCreator.LCMessageBox.resources -resource:$(BUILD_DIR)/LipidCreator.LipidsReview.resources -resource:$(BUILD_DIR)/LipidCreator.MS2Form.resources -resource:$(BUILD_DIR)/LipidCreator.NewFragment.resources -resource:$(BUILD_DIR)/LipidCreator.NewMediator.resources -resource:$(BUILD_DIR)/LipidCreator.TranslatorDialog.resources -resource:$(BUILD_DIR)/LipidCreator.LipidsInterList.resources -resource:$(BUILD_DIR)/LipidCreator.Wizard.resources

WITH_DEBUG = 
#WITH_DEBUG = -d:DEBUG

main: setup
	resgen2 -compile $(RESX_FILES)

	$(CC) $(RES_FILES) -t:library -pkg:dotnet -platform:x64 -sdk:4.7.2 $(LIBS) -lib:/usr/lib/mono/4.7.2 $(CORE_FILES) -out:$(BUILD_DIR)/LipidCreator-Core.dll

	$(CC) $(RES_FILES) -t:library -pkg:dotnet -platform:x64 -sdk:4.7.2 $(LIBS) -r:$(BUILD_DIR)/LipidCreator-Core.dll -lib:/usr/lib/mono/4.7.2 $(GUI_FILES) -out:$(BUILD_DIR)/LipidCreator-GUI.dll
	
	$(CC) $(RES_FILES) -pkg:dotnet -platform:x64 -sdk:4.7.2 $(LIBS) -lib:/usr/lib/mono/4.7.2 MainWindow.cs -r:$(BUILD_DIR)/LipidCreator-Core.dll -r:$(BUILD_DIR)/LipidCreator-GUI.dll -out:$(BUILD_DIR)/LipidCreator.exe
	
	#mcs $(RES_FILES) -pkg:dotnet -platform:x64 -sdk:4.7.2 $(LIBS) -lib:/usr/lib/mono/4.7.2 MainWindow.cs $(GUI_FILES) $(CORE_FILES) -out:$(BUILD_DIR)/LipidCreator.exe

tests: setup
	resgen2 -compile $(RESX_FILES)
	$(CC) -pkg:dotnet $(LIBS) $(WITH_DEBUG) -lib:/usr/lib/mono/4.7.2 $(CORE_FILES) TestTransitionList.cs -out:$(BUILD_DIR)/TestTransitionList.exe
	
	$(CC) $(RES_FILES) -pkg:dotnet $(LIBS) $(WITH_DEBUG) -lib:/usr/lib/mono/4.7.2 $(CORE_FILES) $(GUI_FILES) TestTutorials.cs -out:$(BUILD_DIR)/TestTutorials.exe

complete:
	$(MAKE) setup
	$(MAKE) main
	$(MAKE) zip

zip:
	cd $(BUILD_DIR) && zip -r ../../LipidCreator.zip LipidCreator.exe data images $(DLLS) log4net.xml tool-inf
	
	
setup:  $(SQLite_DLL)
	if [ ! -d "$(BUILD_DIR)" ];then \
		mkdir -p bin; \
		mkdir -p $(BUILD_DIR); \
		cp -r data images $(DLLS) ../packages/log4net.2.0.12/lib/net40-client/* tool-inf $(BUILD_DIR); \
		echo -1 > $(BUILD_DIR)/data/analytics.txt; \
	fi
	mkdir -p $(BUILD_DIR)/data/predefined
	

clean:
	rm -rf bin && rm -rf obj/

run:
	cd $(BUILD_DIR) && mono LipidCreator.exe &
	
runtest:
	mono $(BUILD_DIR)/TestTransitionList.exe
