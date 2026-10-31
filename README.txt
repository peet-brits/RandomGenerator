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

TODO / SKIP (too much effort, will never need it, and if I do, use "www.dicelog.com/dice" instead)
Usage 2.x: Highest (H), lowest (L), re-roll 1s (R), and extra (E) numbers.
dotnet run 4d4-L        ### drop the lowest value.
dotnet run 6d6-2L+H     ### drop the two lowest values, and add the highest.

TODO: not yet implemented, because I need to modify number addition (see below).
Usage 2.X: Random letter: dA / dAZ.
Format 1: [x]dA - randomly pick a letter in the range A-Z, x times.
Format 2: [x]d<A-Z><A-Z> - randomly pick a letter in the specified range, x times.
dotnet run dA       # A-Z
dotnet run dDM      # D-M
dotnet run 3dA      # A-Z, 3x times
dotnet run 2dCY     # C-Y, 2x times
dotnet run dM       # ERROR: NOT SUPPORTED (a single letter might conflict with other command, e.g. dF)
TODO: modify number addition.
    E.g. dA d6 = B 2
    E.g. d4 2c6 dA 2dF = 3 + 2 + 5 + V - 1 + 0 = 10 V -1
TODO: how?
    Move the current number builder structures to a class, add this class to a list,
        and with every change in type we start a new class in this list.
    Or rather... A class that has a string (initial requested calculation command)
        (e.g. [d4 2c6 dA 2dF] or [+d4 +2c6 +dA +2dF] -> [d4 + 2c6 + dA + 2dF])
        and a list of objects, e.g. [+3 +2 +5], [+V], [-1 +0].
    Every class:
        string representing the calculation
        flag indicating whether it is a number or a letter
        number representing the final amount (if number, else 0)
        letter representing the letter (if letter, else ?)
TODO: I want to change the final output
    Current:
        Result: 2 + 7 + 2 + 5 + 3 = 19
    Desired:
        d2 + 4d10 = 2 + 7 + 2 + 5 + 3 = 19
    Or maybe (but probably not):
        <text>: d2 + 4d10 = 2 + 7 + 2 + 5 + 3 = 19
    Desired sample 2:
        Query: d4 -2d6 dA 2dF
        Result: d4 - 2c6 + dA + 2dF = 3 - 2 - 5 + V - 1 + 0 = -4 V -1
    Desired sample 3:
        Query: -d6 dAX d12 dF -d2 2dBD
        Result: -d6 + dAX + d12 + dF - d2 + 2dBD = -6 + B + 12 + 0 - 2 + B + C = -6 B 10 B C (or -6 B 10 BC)
        Pick one:
            -6 B 10 B C     (probably this one, subject to change)
            -6 B 10 BC      (question: are consecutive letters one or multiple calculation objects?)
    Breakdown
        <original request> = <individual results> = <calculated summary>
        <string>             <list of dice          <output from list of calculated objects>
                                converted to
                                list of calculated objects>
