CC = x86_64-w64-mingw32-g++

main:
	mcs -pkg:dotnet -r:System.Xml.Linq.dll -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 LipidCreator.cs CreatorGUI.cs CreatorGUI.Designer.cs MS2Form.cs MS2Form.Designer.cs LipidsReview.cs LipidsReview.Designer.cs NewFragment.cs NewFragment.Designer.cs SpectralName.cs SpectralName.Designer.cs AboutDialog.cs AboutDialog.Designer.cs FattyAcid.cs FattyAcidGroup.cs MS2Fragment.cs Lipid.cs GLLipid.cs PLLipid.cs SLLipid.cs Cholesterol.cs Mediator.cs MediatorMS2Form.cs MediatorMS2Form.Designer.cs AddHeavyPrecursor.cs AddHeavyPrecursor.Designer.cs Precursor.cs Tutorial.cs -out:LipidCreator.exe

test:
	mcs -pkg:dotnet -r:System.Xml.Linq.dll -d:DEBUG -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 LipidCreator.cs TestTransitionList.cs FattyAcid.cs FattyAcidGroup.cs MS2Fragment.cs Lipid.cs GLLipid.cs PLLipid.cs SLLipid.cs Cholesterol.cs Mediator.cs Precursor.cs -out:TestTransitionList.exe

complete: main
	zip -r LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll tool-inf

zip:
	zip -r LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll tool-inf

clean:
	rm LipidCreator.exe && rm -r obj/

run:
	mono LipidCreator.exe &
	
runtest:
	mono TestTransitionList.exe
