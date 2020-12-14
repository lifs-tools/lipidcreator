BUILD_NUMBER ?= 0
main:
	msbuild LipidCreator.sln /p:Configuration=Release /p:Platform=x64 /p:BuildNumber=$(BUILD_NUMBER)
clean:
	msbuild LipidCreator.sln /p:Configuration=Release /p:Platform=x64 -t:Clean
