CC = x86_64-w64-mingw32-g++

main:
	dmcs -r:SkylineTool.dll -r:Ionic.Zip.dll -r:System.Data.SQLite.dll -lib:/usr/lib/mono/4.5 -pkg:dotnet Program.cs CreatorGUI.cs CreatorGUI.Designer.cs MS2Form.cs MS2Form.Designer.cs LipidsReview.cs LipidsReview.Designer.cs NewFragment.cs NewFragment.Designer.cs SpectralName.cs SpectralName.Designer.cs -out:LipidCreator.exe

complete: main
	zip -r LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll tool-inf

zip:
	zip -r LipidCreator.zip LipidCreator.exe data images SkylineTool.dll Ionic.Zip.dll System.Data.SQLite.dll tool-inf
