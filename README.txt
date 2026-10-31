Generate a random number.

Similar website: www.dicelog.com/dice

Usage 0: If not in the main project folder.
dotnet run --project .\RandomGenerator\ -- <...arguments...>

Usage 1: Random number.
dotnet run
dotnet run <max>
dotnet run <min> <mix> [<repeat>]

Usage 1.1: Random letter.
dotnet run <A-Z>                    # max
dotnet run <A-Z> <A-Z> [repeat]     # min - max, optional repeat

Chain usage example:                # Because adding is not yet supported.
dotnet run Z; dotnet run 10         # Random letter A-Z, then random number 1-10.

Usage 2: xdy.
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

Usage 2.3: Fudge dice: dF.
dotnet run 4dF
