main:
	dotnet build -c Release
clean:
	dotnet build -t:Clean
run:
	mono LipidCreator/bin/Release/net47/LipidCreator.exe &
