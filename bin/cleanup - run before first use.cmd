del "AnAusAutomat.Console.exe.config"
del "CommandLine.xml"
del "log.txt"

del /s "Serilog.xml"
del /s "Serilog.Sinks.Console.xml"
del /s "Serilog.Sinks.File.xml"
del /s "*.pdb"

cd "Controllers"
del /s "Toolbox.dll"
del /s "Contracts.dll"

cd "..\Sensors"
del /s "Toolbox.dll"
del /s "Contracts.dll"

pause