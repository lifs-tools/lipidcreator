main:
	dotnet build -c Release
clean:
	dotnet build -t:Clean
	rm -rf LipidCreator/bin
	rm -rf LipidCreator/obj

run:
	mono LipidCreator/bin/Release/net47/LipidCreator.exe &
