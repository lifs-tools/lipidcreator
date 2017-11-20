CC = x86_64-w64-mingw32-g++

main:
	mcs -pkg:dotnet -r:System.Xml.Linq.dll -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 Program.cs CreatorGUI.cs CreatorGUI.Designer.cs MS2Form.cs MS2Form.Designer.cs LipidsReview.cs LipidsReview.Designer.cs NewFragment.cs NewFragment.Designer.cs SpectralName.cs SpectralName.Designer.cs AboutDialog.cs AboutDialog.Designer.cs FattyAcid.cs FattyAcidGroup.cs MS2Fragment.cs Lipid.cs GLLipid.cs PLLipid.cs SLLipid.cs Cholesterol.cs -out:LipidCreator.exe

complete: main
	zip -r LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll tool-inf

zip:
	zip -r LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll tool-inf

clean:
	rm LipidCreator.exe && rm -r obj/
