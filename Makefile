play:
	dotnet publish -r win-x64 /p:TieredCompilation=false --no-self-contained
	./bin/Debug/netcoreapp3.1/win-x64/TwoManSky

clean:
	rm -r "bin/Debug/netcoreapp3.1/win-x64/*"