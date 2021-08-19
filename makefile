BUILD_NUMBER ?= 0
main:
	dotnet build -c Release -p:BuildNumber=$(BUILD_NUMBER) -p:PublishSingleFile=true
clean:
	dotnet build -t:Clean
run:
	mono LipidCreator/bin/Release/net47/LipidCreator.exe &
