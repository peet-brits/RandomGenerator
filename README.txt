Generates a random number.

Usage 1:
dotnet run
dotnet run <max>
dotnet run <min> <mix> [<repeat>]

Usage 2:
Any number of arguments in the format [x]d<y>, e.g. roll a y-sided die x times.
Example:
dotnet run 3d10
dotnet run 4d6 d12

Usage 2.1: "+" and "-" signs, for example:
dotnet run 4d10 -2d4
dotnet run -d6 d20

Usage 2.2: raw numbers, as long as they are not the first item.
dotnet run 2d6 5
dotnet run 5d4 -3 -1d6
dotnet run 10 2d6   ### incorrect ###
