from random import Random

verbose = False
betList = []  # { 0.01, 0.02, 0.04, 0.08, 0.16, 0.32, 0.64, 1.28, 2.56, 5.12, 10.24, 20.48, 40.96, 81.92, 163.84, 327.68 }
betCount = 16
betOne = 0.01
betList.append(betOne)
for x in range(betCount):
    betList.append(betList[-1] * 2.0)
baseMultiplier = 1  # 5
multiplier = baseMultiplier
highestBalance = 0
roundIndex = 0
# largestBet = 0
startTurn = 3  # 2
endTurn = 5  # 3
lastEntries = []
showHistory = 5
threshold = 5
focus = -1
defaultBaseBet = 1
history = []
lastPrediction = ""
additionalOutput = "\n"
# testPrint = 1
# includes 0, 1, 2, 3 (first 4 turns). i.e. betTurns = turns - 1
betTurns = 3
historyLength = 20  # 30  # gives same percentage if 50 or 20 or 30
nextBet = 0
predictedBet = 0
predictedWins = 0
predictedLoses = 0
predictedConsective = 0
predictedLargestConsecutive = 0
minEntry = 1  # 1
maxEntry = 2  # 4
tDic = {}
filePath = "/Users/ramiemera/Documents/Rollbot/SanTan/SanTan/data.txt"
reader = open(filePath, "r")
writer = open(filePath, "a")
lines = []

for line in reader:
    if not line:
        continue
    lines.append(line)

pattern = [    "1",    "1",    "1",    "1",    "1",    "2",    "2",    "2",    "2",    "2",    "1",    "1",    "1",    "1",    "1",    "2",    "2",    "2",    "2",    "2",    "1",    "1",    "1",    "1",    "1",    "2",    "2",    "2",    "2",    "2",    "1",    "1",    "1",    "1",    "1",    "2",    "2",    "2",    "2",    "2",]

def AddNumber(line, write):
    newNum = int(line)
    if newNum >= minEntry and newNum <= maxEntry:
        if len(history) >= historyLength:
            history.pop(0)
        history.append(newNum)
        if predictedBet != 0:
            if newNum == predictedBet:
                predictedWins += 1
                if predictedLoses in tDic:
                    tDic[predictedLoses] += 1
                else:
                    tDic[predictedLoses] = 1
                if predictedLoses >= 0 and predictedLoses <= betTurns:
                    # round = 0
                    roundIndex = 0
                predictedLoses = 0
                predictedConsective = 0
            else:
                predictedWins = 0
                if predictedLoses >= 0 and predictedLoses <= betTurns:
                    roundIndex += 1
                    if roundIndex >= len(betList):
                        roundIndex = 0
                    # if predictedLoses == betTurns:
                    #    round++
                predictedLoses += 1
                predictedConsective += 1
                if predictedConsective > predictedLargestConsecutive:
                    predictedLargestConsecutive = predictedConsective
        if write:
            writeToFile(newNum)
        if focus >= 0:
            focus += 1
            if focus >= 20:
                focus = 0

def LoopHistory(patternIndex, results):
    printResult = False
    if patternIndex == 10:
        printResult = True
    turnList = [1]
    baseBet = defaultBaseBet
    balance = 0
    lowestBalance = 0
    highestBet = 0
    for historyIndex in range(len(history)):
        historyValue = history[historyIndex]
        if patternIndex + historyIndex < len(pattern):
            turns = turnList[-1]
            patternValue = int(pattern[patternIndex + historyIndex])
            if historyValue == patternValue:
                # Win
                # profit otherwise missed profit opportunity due to outside bounds
                if turns >= startTurn and turns <= endTurn:
                    balance += baseBet * 1.96
                    # balance += (baseBet * 3 * 2.58)
                    if baseBet > highestBet:
                        highestBet = baseBet
                    baseBet = defaultBaseBet
                turns = 1
                turnList.append(turns)
            else:
                # Loss
                if turns >= startTurn and turns <= endTurn:
                    balance -= baseBet
                    # balance -= baseBet * 3
                    if balance < lowestBalance:
                        lowestBalance = balance
                    baseBet *= 2
                    # if baseBet > largestBet:
                    #    largestBet = baseBet
                turns += 1
                turnList[-1] = turns
        else:
            print("Pattern List Out of bounds error")
    # CalculatePercentage
    newResults = []
    # if patternIndex == 11:
    #    CalculatePercentage(turnList, true)
    percentage = -1
    if len(turnList) > 0:
        percentage = CalculatePercentage(turnList, False)
        if math.isnan(percentage):
            percentage = -1
    newResults.append(percentage)
    newResults.append(turnList[-1])
    if len(turnList) >= 2:
        newResults.append(turnList[-2])
    else:
        newResults.append(0)
    # Result List
    # 0 = Percentage
    # 1 = NT
    # 2 = PT
    # 3 = Bal
    # 4 = NNum
    # 5 = LBet
    # 6 = LBal
    # 7 = Turns
    # 8 = Rank
    newResults.append(balance)  # 3
    # try
    # {
    newResults.append(float(pattern[patternIndex + len(history)]))  # 4
    # } catch (Exception e)
    # {
    #    Console.WriteLine("History Count: " + history.Count + ", Pattern Size: " + pattern.Count + ", PatternIndex: " + patternIndex)
    # }
    newResults.append(highestBet)  # 5
    newResults.append(lowestBalance)  # 6
    winTurns = 0
    for x in range(len(turnList) - 1):
        if startTurn <= turnList[x] <= endTurn:
            winTurns += 1
    newResults.append(winTurns)
    if highestBalance < balance:
        highestBalance = balance
    if patternIndex in results:
        results[patternIndex] = newResults
    else:
        results[patternIndex] = newResults

    return results

def ProcessPatterns(isPrivate):
    results = {}
    if focus == -1:
        for patternIndex in range(20):
            results = LoopHistory(patternIndex, results)
    else:
        results = LoopHistory(focus, results)
    results = CalculatRanking(results)
    output = ""
    rankingDictionary = {}
    for entry in results:
        patternInd = entry
        ranking = int(entry[8])
        balance = int(entry[3])
        rankingDictionary[patternInd] = ranking
    rankingDictionary = dict(sorted(rankingDictionary.items(), key=lambda x: x[1]))
    probabilityDictionary = {}
    for entry in rankingDictionary:
        patternInd = entry
        valueList = results[patternInd]
        ranking = int(valueList[8])
        if ranking > 0 or focus >= 0:
            nextTurn = valueList[1]
            patternString = "(" + str(patternInd + 1) + ")["
            for i in range(patternInd, patternInd + 20):
                patternString += str(pattern[i]) + "-"
            patternString = patternString[:-1]
            percentage = int(valueList[0])
            previousTurn = valueList[2]
            balance = int(valueList[3])
            nextNumber = int(valueList[4])
            if nextNumber in probabilityDictionary:
                probabilityDictionary[nextNumber] += 1
            else:
                probabilityDictionary[nextNumber] = 1
            if not isPrivate:
                largestBet = int(valueList[5])
                lowestBal = int(valueList[6])
                wins = int(valueList[7])
                if verbose:
                    output += patternString + "]\tR: " + str(ranking) + "\t" + str(percentage) + " %\tN: " + str(nextNumber) + "\tNT: " + str(nextTurn) + "\tPT: " + str(previousTurn) + "\t$: " + str(balance) + "\n"
    additionalOutput = "\n"
    dicSize = 0
    for entry in probabilityDictionary.items():
        dicSize += entry[1]

    probabilityDictionary = dict(sorted(probabilityDictionary.items(), key=lambda item: item[1]))

    counter = 0
    predictedBet = 0

    if predictedWins != 0 or predictedLoses != 0:
        if not isPrivate and verbose:
            additionalOutput += "PW: " + str(predictedWins) + "\tPL: " + str(predictedLoses) + "\tPCL: " + str(predictedConsective) + "\tPLC: " + str(predictedLargestConsecutive) + "\n"

    small = 0
    large = 0
    even = 0
    odd = 0
    winners = 0
    losers = 0

    for entry in probabilityDictionary.items():
        key = entry[0]
        if counter == 0:
            counter += 1
            predictedBet = key
            if not isPrivate:
                if roundIndex < len(betList) - 1 and predictedLoses >= 0 and predictedLoses <= betTurns:
                    if verbose:
                        additionalOutput += "BET: $" + (betList[roundIndex] * multiplier) + " @[ " + str(predictedBet) + " ] -- "
                    else:
                        additionalOutput += "BET: $" + (betList[roundIndex] * multiplier) + " @[ " + str(predictedBet) + " ]"
                else:
                    if verbose:
                        additionalOutput += "DUMMY BET: $" + betOne + " @[ " + str(predictedBet) + " ] -- "
                    else:
                        additionalOutput += "DUMMY BET: $" + betOne + " @[ " + str(predictedBet) + " ]"
            else:
                lastPrediction = "BET: $" + (betList[roundIndex] * multiplier) + " @[ " + str(predictedBet) + " ]"
        if not isPrivate:
            value = entry[1]
            percent = (value / dicSize) * 100.0
            if verbose:
                additionalOutput += "[" + str(key) + " - " + str(int(percent)) + "%], "
            if key == 1:
                small += int(percent)
                odd += int(percent)
            elif key == 2:
                small += int(percent)
                even += int(percent)
            elif key == 3:
                large += int(percent)
                odd += int(percent)
            elif key == 4:
                large += int(percent)
                even += int(percent)
    if not isPrivate:
        additionalOutput = additionalOutput.strip()
        if len(additionalOutput) > 1:
            for ent in tDic.items():
                entKey = ent[0]
                if entKey <= betTurns:
                    winners += 1
                else:
                    losers += 1
            if verbose:
                additionalOutput = additionalOutput[:-1]
            winPer = (winners / (winners + losers)) * 100
            if verbose:
                additionalOutput += " |||| [SM: " + str(small) + "%] , [LG: " + str(large) + "%], [ODD: " + str(odd) + "%], [EVE: " + str(even) + "%], [W%: " + str(int(winPer)) + "%]"
        output += additionalOutput + "\n"
        print(output)

def CalculatRanking(results: dict) -> dict:
    balanceDictionary = {}
    percentageDictionary = {}
    largestDictionary = {}
    lowestDictionary = {}
    turnDictionary = {}

    for entry in results.items():
        patternInd = entry[0]
        balance = int(entry[1][3])
        balanceDictionary[patternInd] = balance

        try:
            percentage = int(entry[1][0])
            percentageDictionary[patternInd] = percentage
        except Exception as e:
            print("****" + entry[1][0] + "****")

        largestBet = int(entry[1][5])
        largestDictionary[patternInd] = largestBet

        lowestBalance = int(entry[1][6])
        lowestDictionary[patternInd] = lowestBalance

        turns = int(entry[1][7])
        turnDictionary[patternInd] = turns

    balanceDictionary = dict(sorted(balanceDictionary.items(), key=lambda x: x[1]))
    percentageDictionary = dict(sorted(percentageDictionary.items(), key=lambda x: x[1]))
    largestDictionary = dict(sorted(largestDictionary.items(), key=lambda x: x[1]))
    lowestDictionary = dict(sorted(lowestDictionary.items(), key=lambda x: x[1]))
    turnDictionary = dict(sorted(turnDictionary.items(), key=lambda x: x[1]))

    negativeIndices = []
    ranking = 1
    for entry in reversed(balanceDictionary.items()):
        thisBalance = entry[1]
        patternInd = entry[0]
        resultList = results[patternInd]
        if thisBalance > 0:
            resultList.append(ranking)
            ranking += 1
            results[patternInd] = resultList
        else:
            negativeIndices.append(patternInd)
    ranking += 1
    for negInd in negativeIndices:
        resultList = results[negInd]
        resultList.append(ranking)
        results[negInd] = resultList

    ranking = 0
    lastPercentage = 0
    for entry in percentageDictionary.items():
        thisPercentage = entry[1]
        patternInd = entry[0]
        if lastPercentage != thisPercentage:
            ranking += 1
        resultList = results[entry[0]]
        oldRanking = resultList[-1]
        newRanking = oldRanking + ranking
        resultList[-1] = newRanking
        results[patternInd] = resultList

    ranking = 0
    for entry in reversed(largestDictionary.items()):
        patternInd = entry[0]
        ranking += 1
        resultList = results[entry[0]]
        oldRanking = resultList[-1]
        newRanking = oldRanking + ranking
        resultList[-1] = newRanking
        results[patternInd] = resultList

    ranking = 0
    for entry in reversed(lowestDictionary.items()):
        patternInd = entry[0]
        ranking += 1
        resultList = results[entry[0]]
        oldRanking = resultList[-1]
        newRanking = oldRanking + ranking
        resultList[-1] = newRanking
        results[patternInd] = resultList

    ranking = 0
    for entry in reversed(turnDictionary.items()):
        patternInd = entry[0]
        ranking += 1
        resultList = results[entry[0]]
        oldRanking = resultList[-1]
        newRanking = (oldRanking + ranking) / 5.0
        resultList[-1] = newRanking
        results[patternInd] = resultList

    return results

def CalculatePercentage(turnList: list, debugPrint: bool) -> float:
    outBounds = 0
    for i in range(len(turnList) - 1):
        turnValue = turnList[i]
        if turnValue > threshold:
            outBounds += 1

    if debugPrint:
        for j in turnList:
            print(j)
        print("OB: " + str(outBounds) + ", Total: " + str(len(turnList) - 1))

    return (outBounds / (len(turnList) - 1)) * 100

def GetConsoleOutput() -> str:
    finalConsoleOutput = ""
    consoleOutput = ""
    if not lastPrediction:
        finalConsoleOutput = lastPrediction + "\n\n"
        lastPrediction = ""
    elif not additionalOutput.strip():
        random = Random()
        n = random.randint(1, 3)
        finalConsoleOutput = "DUMMY BET: $0.01 @ [ " + str(n) + " ]\n\n"
    if len(history) >= showHistory:
        for i in range(len(history) - showHistory, len(history)):
            consoleOutput += history[i] + " "
        consoleOutput = consoleOutput.strip()
        if consoleOutput:
            consoleOutput = "{" + consoleOutput + "} "
    consoleOutput += "Enter integer: "
    return finalConsoleOutput + consoleOutput

def WriteToFile(newNum: int):
    writer.write("\n" + str(newNum))
    writer.flush()

def CalculateRequiredBal():
    askingBalance = 0
    for i in range(len(betList)):
        askingBalance += betList[i] * multiplier
    print("Balance at least required: $" + format(askingBalance, "n0"))

for line in lines:
    if line in ["1", "2", "3", "4"]:
        AddNumber(line, False)
        if len(history) == historyLength:
            ProcessPatterns(True)

val = ""
ask = True
CalculateRequiredBal()


while ask:
    print(GetConsoleOutput())
    val = input()
    if not val or not val.strip():
        ask = False
    elif val.startswith("!"):
        val = val[1:]
        try:
            nextBet = int(val)
        except:
            pass
    elif val.startswith("/"):
        val = val[1:]
        try:
            divide = int(val)
            print(divide)
            multiplier = multiplier / divide
        except:
            multiplier = baseMultiplier
        CalculateRequiredBal()
    elif val.startswith("<"):
        val = val[1:]
        try:
            focus = int(val) - 1
        except:
            if val == "v":
                verbose = not verbose
        nextBet = 0
    else:
        for c in val:
            try:
                AddNumber(c, True)
            except:
                pass
        try:
            AddNumber(val, True)
        except:
            pass
    if len(history) == historyLength:
        ProcessPatterns(False)
