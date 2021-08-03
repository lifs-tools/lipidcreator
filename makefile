BUILD_NUMBER ?= 0
main:
	xbuild LipidCreator.sln /p:Configuration=Release /p:Platform=x64 /p:BuildNumber=$(BUILD_NUMBER)
clean:
	xbuild LipidCreator.sln /p:Configuration=Release /p:Platform=x64 -t:Clean
run:
	mono LipidCreator/bin/x64/Release/LipidCreator.exe &
