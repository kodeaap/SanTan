// See https://aka.ms/new-console-template for more information
//1 is gold, 2 is silver
using System;
using System.Collections.Generic;
using System.Linq;

bool verbose = false;
bool isTest = false;// true;// true;
List<double> betList = new List<double>();
double dummyBet = 0.01;
double betOne = 0.05;// [Working 0.02, 0.05] [Test 0.1]
double increment = 2.2;// [Working 2.2, 2.2] [Test 2.2]
//betList.Add(dummyBet);
string bankOutput = "";
double betAmount = betOne;
int thresholdBets = 61;// [Working 30, 61] [Test 121]
int badRuns = 0;
double worstBet = 0.0;
//for (int x = 0; x < betCount; x++)
while (betAmount < thresholdBets)
{
    betList.Add(betAmount);
    betAmount = Math.Round(betAmount * increment, 2);
// betAmount = Math.Round(betList[betList.Count - 1] * increment, 2);
    
}
Console.WriteLine(bankOutput);
double baseMultiplier = 1;// 5;
double multiplier = baseMultiplier;
double highestBalance = 0;
int roundIndex = 0;
//double largestBet = 0;
var startTurn = 3; //2;
var endTurn = 5; // 3;
var lastEntries = new List<string>();
int showHistory = 5;
int threshold = 5;
var focus = -1;
var defaultBaseBet = 1;
var history = new List<int>();
string lastPrediction = "";
string additionalOutput = "\n";
//var testPrint = 1;
//includes 0, 1, 2, 3 (first 4 turns). i.e. betTurns = turns - 1
int betTurns = 2;// 3;
var historyLength = 20;// 30; //gives same percentage if 50 or 20 or 30
var nextBet = 0;
var predictedBet = 0;
var predictedWins = 0;
var predictedLoses = 0;
var predictedConsective = 0;
var predictedLargestConsecutive = 0;
var minEntry = 1;// 1;
var maxEntry = 2;// 4;
var multiplyCounter = 0;
Dictionary<int, int> tDic = new Dictionary<int, int>();
var filePath = "/Users/ramiemera/Documents/Rollbot/CoinFlip/SanTan/data.txt";
StreamReader reader = new StreamReader(File.OpenRead(filePath));
StreamWriter writer = new StreamWriter(filePath, append: true);
List<string> lines = new List<string>();
string inputFilename = "/Users/ramiemera/Documents/Rollbot/CoinFlip/SanTan/input.txt";
string outputFilename = "/Users/ramiemera/Documents/Rollbot/CoinFlip/SanTan/output.txt";
string fileOutput = "";
while (reader.EndOfStream == false) // read all lines in file until end
{
    string line = reader.ReadLine();
    if (line == null) continue;
    lines.Add(line);
}//string readerInput = Convert.ToString(reader.ReadToEnd);
    //if (readerInput != null)
    //{
    //string[] lines = readerInput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    //string[] lines = readerInput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
    //System.IO.File.ReadAllLines(@"/Users/ramiemera/Documents/Rollbot/CoinFlip/SanTan/data.txt");
    //}
    //reader.Close();
    var pattern = new List<string> { "1",  "1",  "1",  "1",  "1",  "2",  "2",  "2",  "2",  "2",  "1",  "1",  "1",  "1",  "1",  "2",  "2",  "2",  "2",  "2",  "1",  "1",  "1",  "1",  "1",  "2",  "2",  "2",  "2",  "2",  "1",  "1",  "1",  "1",  "1",  "2",  "2",  "2",  "2",  "2"};
//Function
void AddNumber(string line, bool write)
{
    int newNum = int.Parse(line);
    if (newNum >= minEntry && newNum <= maxEntry)
    {
        if (history.Count >= historyLength)
        {
            history.RemoveAt(0);
        }
        history.Add(newNum);
        if (predictedBet != 0)
        {
            if (newNum == predictedBet)
            {
                predictedWins++;
                if (tDic.ContainsKey(predictedLoses))
                {
                    tDic[predictedLoses] = tDic[predictedLoses] + 1;
                }
                else
                {
                    tDic.Add(predictedLoses, 1);
                }
                if (predictedLoses >= 0 && predictedLoses <= betTurns)
                {
                   // round = 0;
                    roundIndex = 0;
                    if (multiplier > baseMultiplier)
                    {
                        multiplyCounter += 1;
                        if (multiplyCounter > 10)
                        {
                            multiplier = baseMultiplier;
                            Console.WriteLine("Reached 10 high risk wins... reset multiplier");
                        }
                        else
                        {
                            Console.WriteLine("Need 10 wins on high risk. Only at " + multiplyCounter);
                        }
                    }
                    //Console.WriteLine("Bad Reset");
                }
                predictedLoses = 0;
                predictedConsective = 0;
                
            }
            else
            {
                predictedWins = 0;
                if (predictedLoses >= 0 && predictedLoses <= betTurns)
                {
                    roundIndex++;
                    if (roundIndex >= betList.Count)
                    {
                        badRuns++;
                        Console.WriteLine("Bad Runs: " + badRuns);
                        roundIndex = 0;
                        history = new List<int>();
                        worstBet = 0.0;
                        if (multiplier == baseMultiplier)
                        {
                            Console.WriteLine("Raise to high risk bets" + multiplyCounter);
                            multiplier *= 10;
                        }
                        else
                        {
                            Console.WriteLine("High risk just lost..." + multiplyCounter);
                            multiplier = baseMultiplier;
                        }
                    }
                    //if (predictedLoses == betTurns)
                    //{
                    //    round++;
                    //}
                }
                predictedLoses++;
                predictedConsective++;
                if (predictedConsective > predictedLargestConsecutive)
                {
                    predictedLargestConsecutive = predictedConsective;
                }
            }
        }

        if (write)
        {
            writeToFile(newNum);
        }
        if (focus >= 0)
        {
            focus++;
            if (focus >= 20)
            {
                focus = 0;
            }
        }
    }
}

Dictionary<int, List<float>> LoopHistory(int patternIndex, Dictionary<int, List<float>> results)
{
    bool printResult = false;
    if (patternIndex == 10)
    {
        printResult = true;
    }
    var turnList = new List<int> {
    1
  };
    double baseBet = defaultBaseBet;
    double balance = 0;
    double lowestBalance = 0;
    double highestBet = 0;
    for (int historyIndex = 0; historyIndex < history.Count; historyIndex++)
    {
        int historyValue = history[historyIndex];
        if (patternIndex + historyIndex < pattern.Count)
        {
            int turns = turnList[turnList.Count - 1];
            int patternValue = int.Parse(pattern[patternIndex + historyIndex]);
            if (historyValue == patternValue)
            {
                //Win
                //profit otherwise missed profit opportunity due to outside bounds
                if (turns >= startTurn && turns <= endTurn)
                {
                    balance += baseBet * 1.96;
                    //balance += (baseBet * 3 * 2.58);
                    if (baseBet > highestBet)
                    {
                        highestBet = baseBet;
                    }
                    baseBet = defaultBaseBet;
                }

                turns = 1;
                turnList.Add(turns);

            }
            else
            {
                //Loss

                if (turns >= startTurn && turns <= endTurn)
                {
                    balance -= baseBet;
                    //balance -= baseBet * 3;
                    if (balance < lowestBalance)
                    {
                        lowestBalance = balance;
                    }
                    baseBet *= 2;
                    //if (baseBet > largestBet)
                    //{
                    //    largestBet = baseBet;
                    //}
                }
                turns++;
                turnList[turnList.Count - 1] = turns;
            }
        }
        else
        {
            Console.WriteLine("Pattern List Out of bounds error");
        }

    }

    //CalculatePercentage
    List<float> newResults = new List<float>();
    //if (patternIndex == 11)
    // {
    //    CalculatePercentage(turnList, true);
    //}
    float percentage = -1;
    if (turnList.Count > 0)
    {
        percentage = CalculatePercentage(turnList, false);
        if (Double.IsNaN(percentage))
        {
            percentage = -1;
        }
    }
    newResults.Add(percentage);
    newResults.Add((float)turnList[turnList.Count - 1]);
    if (turnList.Count >= 2)
    {
        newResults.Add((float)turnList[turnList.Count - 2]);
    }
    else
    {
        newResults.Add((float)0);
    }
    //Result List
    //0 = Percentage
    //1 = NT
    //2 = PT
    //3 = Bal
    //4 = NNum
    //5 = LBet
    //6 = LBal
    //7 = Turns
    //8 = Rank
    newResults.Add((float)balance);//3
    //try
    //{
    newResults.Add(float.Parse(pattern[patternIndex + history.Count]));//4
    //} catch (Exception e)
    //{
    //    Console.WriteLine("History Count: " + history.Count + ", Pattern Size: " + pattern.Count + ", PatternIndex: " + patternIndex);
    //}
    newResults.Add((float)highestBet);//5
    newResults.Add((float)lowestBalance);//6
    int winTurns = 0;
    for (int x = 0; x < turnList.Count - 1; x++)
    {
        if (turnList[x] >= startTurn && turnList[x] <= endTurn)
        {
            winTurns++;
        }
    }
    newResults.Add((float)winTurns);
    if (highestBalance < balance)
    {
        highestBalance = balance;
    }
    if (results.ContainsKey(patternIndex))
    {
        results[patternIndex] = newResults;
    }
    else
    {
        results.Add(patternIndex, newResults);
    }

    return results;
}

void ProcessPatterns(bool isPrivate)
{
    var results = new Dictionary<int,
      List<float>>();
    if (focus == -1)
    {
        for (int patternIndex = 0; patternIndex < 20; patternIndex++)
        {
            results = LoopHistory(patternIndex, results);
        }
    }
    else
    {
        results = LoopHistory(focus, results);
    }
    results = CalculatRanking(results);
    string consoleOutput = "";

    var rankingDictionary = new Dictionary<int,
      float>();
    foreach (KeyValuePair<int, List<float>> entry in results)
    {
        int patternInd = entry.Key;
        float ranking = Convert.ToInt32(entry.Value[8]);
        float balance = Convert.ToInt32(entry.Value[3]);
        rankingDictionary.Add(entry.Key, ranking);
    }
    rankingDictionary = rankingDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    Dictionary<int, int> probabilityDictionary = new Dictionary<int, int>();

    //List<int> nextBetList = new List<int>();

    foreach (KeyValuePair<int, float> entry in rankingDictionary)
    {
        //foreach (KeyValuePair<int, List<float>> entry in results)
        int patternInd = entry.Key;
        List<float> valueList = results[patternInd];
        //Result List
        //0 = Percentage
        //1 = NT
        //2 = PT
        //3 = Bal
        //4 = NNum
        //5 = LBet
        //6 = LBal
        //7 = Turns
        //8 = Rank
        int ranking = Convert.ToInt32(valueList[8]);
        //Console.WriteLine("Ranking: " + ranking + " , Focus: " + focus);
        if (ranking > 0 || focus >= 0)
        {
            
            float nextTurn = valueList[1];
                string patternString = "(" + (patternInd + 1) + ")[";
                for (int i = patternInd; i < patternInd + 20; i++)
                {
                    patternString += pattern[i].ToString() + "-";
                }
                patternString = patternString.Remove(patternString.Length - 1, 1);
                int percentage = Convert.ToInt32(valueList[0]);

                float previousTurn = valueList[2];
                int balance = Convert.ToInt32(valueList[3]);
                int nextNumber = Convert.ToInt32(valueList[4]);
                if (probabilityDictionary.ContainsKey(nextNumber))
                {
                    probabilityDictionary[nextNumber] = probabilityDictionary[nextNumber] + 1;
                }
                else
                {
                    probabilityDictionary.Add(nextNumber, 1);
                }
                if (!isPrivate)
                {
                    int largestBet = Convert.ToInt32(valueList[5]);
                    int lowestBal = Convert.ToInt32(valueList[6]);
                    int wins = Convert.ToInt32(valueList[7]);
                if (verbose)
                    {
                    consoleOutput += patternString + "]\tR: " + ranking + "\t" + percentage.ToString() + " %\tN: " + nextNumber + "\tNT: " + nextTurn + "\tPT: " + previousTurn + "\t$: " + balance + "\n"; // + "\tW: " + wins +"\n";
                    }
                }
        }
    }

    additionalOutput = "\n";
    float dicSize = 0;
    foreach (KeyValuePair<int, int> entry in probabilityDictionary)
    {
        dicSize += entry.Value;
    }

    probabilityDictionary = probabilityDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    int counter = 0;
    predictedBet = 0;

    if (predictedWins != 0 || predictedLoses != 0)
    {
        if (!isPrivate && verbose)
        {
            additionalOutput += "PW: " + predictedWins + "\tPL: " + predictedLoses + "\tPCL: " + predictedConsective + "\tPLC: " + predictedLargestConsecutive + "\n";
        }

    }

    int small = 0;
    int large = 0;
    int even = 0;
    int odd = 0;
    int winners = 0;
    int losers = 0;
    //double median = 0;
    foreach (KeyValuePair<int, int> entry in probabilityDictionary.Reverse())
    {
        int key = entry.Key;
        
        if (counter == 0)
        {
            counter++;
            predictedBet = key;
            if (!isPrivate)
            {
                if (roundIndex < betList.Count - 1 && predictedLoses >= 0 && predictedLoses <= betTurns)
                {
                    if (verbose)
                    {
                        additionalOutput += "BET: $" + (betList[roundIndex] * multiplier) + " @[ " + predictedBet + " ] -- ";
                    }
                    else
                    {
                        additionalOutput += "BET: $" + (betList[roundIndex] * multiplier) + " @[ " + predictedBet + " ]";
                    }
                    if (isTest)
                    {
                        fileOutput = "0.01," + predictedBet;
                    }
                    else
                    {
                        fileOutput = betList[roundIndex] * multiplier + "," + predictedBet;
                    }
                }
                else
                {
                    if (verbose)
                    {
                        additionalOutput += "DUMMY BET: $" + dummyBet + " @[ " + predictedBet + " ] -- ";
                    }
                    else
                    {
                        additionalOutput += "DUMMY BET: $" + dummyBet + " @[ " + predictedBet + " ]";
                    }
                    if (isTest)
                    {
                        fileOutput = "0.01," + predictedBet;
                    }
                    else
                    {
                        fileOutput = dummyBet + "," + predictedBet;
                    }
                }
            }
            else
            {
                lastPrediction = "BET: $" + (betList[roundIndex] * multiplier) + " @[ " + predictedBet + " ]";
                if (isTest)
                {
                    fileOutput = "0.01," + predictedBet;
                }
                else
                {
                    fileOutput = (betList[roundIndex] * multiplier) + "," + predictedBet;
                }
            }
        }
        if (!isPrivate)
        {
            int value = entry.Value;
            //Console.WriteLine("V: " + value + ", K: " + key);
            float percent = ((float)value / dicSize) * 100.0f;
            if (verbose)
            {
                additionalOutput += "[" + key + " - " + (int)percent + "%], ";
            }
            if (key == 1)
            {
                small += (int)percent;
                odd += (int)percent;
            }
            else if (key == 2)
            {
                small += (int)percent;
                even += (int)percent;
            }
            else if (key == 3)
            {
                large += (int)percent;
                odd += (int)percent;
            }
            else if (key == 4)
            {
                large += (int)percent;
                even += (int)percent;
            }
        }
    }
    if (!isPrivate)
    {
        additionalOutput = additionalOutput.Trim();
        if (additionalOutput.Length > 1)
        {
            foreach (KeyValuePair<int, int> ent in tDic)
            {
                int entKey = ent.Key;
                //int entValue = ent.Value;
                //Console.WriteLine(entKey + " --> " + entValue);
                if (entKey <= betTurns)
                {
                    winners++;
                }
                else
                {
                    losers++;
                }
            }
            if (verbose)
            {
                additionalOutput = additionalOutput.Remove(additionalOutput.Length - 1);
            }
            float winPer = ((float)winners / (float)(winners + losers)) * 100.0f;
            //Console.WriteLine("Wins: " + winners + " , Loses: " + losers);
            //additionalOutput += " ||< M: " + median + " >|| [SM: " + small + "%] , [LG: " + large + "%], [ODD: " + odd + "%], [EVE: " + even + "%]";
            if (verbose)
            {
                additionalOutput += " |||| [SM: " + small + "%] , [LG: " + large + "%], [ODD: " + odd + "%], [EVE: " + even + "%], [W%: " + (int)winPer + "%]";
            }
        }

        consoleOutput += additionalOutput + "\n";

        writeToOutput();
        //Console.WriteLine(consoleOutput);
    }
}

void writeToOutput(){
    double currentBet;
    if (double.TryParse(fileOutput.Split(',')[0], out currentBet))
    {
        if (currentBet > worstBet)
        {
            worstBet = currentBet;
            Console.WriteLine("Worst Bet: $" + worstBet);
        }
    }
    using (StreamWriter outputFile = File.CreateText(outputFilename))
    {
        outputFile.WriteLine(fileOutput);
        outputFile.Close();
    }
}

Dictionary<int, List<float>> CalculatRanking(Dictionary<int, List<float>> results)
{
    var balanceDictionary = new Dictionary<int,
      float>();
    var percentageDictionary = new Dictionary<int,
      float>();
    var largestDictionary = new Dictionary<int,
      float>();
    var lowestDictionary = new Dictionary<int,
      float>();
    var turnDictionary = new Dictionary<int,
      float>();
    foreach (KeyValuePair<int, List<float>> entry in results)
    {
        int patternInd = entry.Key;
        float balance = Convert.ToInt32(entry.Value[3]);
        balanceDictionary.Add(patternInd, balance);

        try
        {
            float percentage = Convert.ToInt32(entry.Value[0]);
            percentageDictionary.Add(patternInd, percentage);
        }
        catch (Exception e)
        {
            Console.WriteLine("****" + entry.Value[0] + "****");
        }

        float largestBet = Convert.ToInt32(entry.Value[5]);
        largestDictionary.Add(patternInd, largestBet);

        float lowestBalance = Convert.ToInt32(entry.Value[6]);
        lowestDictionary.Add(patternInd, lowestBalance);

        float turns = Convert.ToInt32(entry.Value[7]);
        turnDictionary.Add(patternInd, turns);
    }
    balanceDictionary = balanceDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    percentageDictionary = percentageDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    largestDictionary = largestDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    lowestDictionary = lowestDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    turnDictionary = turnDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    List<int> negativeIndices = new List<int>();
    int ranking = 1;
    foreach (KeyValuePair<int, float> entry in balanceDictionary.Reverse())
    {
        double thisBalance = entry.Value;
        int patternInd = entry.Key;
        List<float> resultList = results[patternInd];
        if (thisBalance > 0)
        {
            resultList.Add(ranking);
            ranking++;
            results[patternInd] = resultList;
        }
        else
        {
            negativeIndices.Add(patternInd);
            //resultList.Add(0);
        }
        //results[patternInd] = resultList;
    }
    ranking++;
    foreach (int negInd in negativeIndices)
    {
        List<float> resultList = results[negInd];
        resultList.Add(ranking);
        results[negInd] = resultList;
    }

    ranking = 0;
    float lastPercntage = 0;
    foreach (KeyValuePair<int, float> entry in percentageDictionary)
    {
        double thisPercentage = entry.Value;
        int patternInd = entry.Key;
        if (lastPercntage != thisPercentage)
        {
            ranking++;
        }

        List<float> resultList = results[entry.Key];
        float oldRanking = resultList.Last();
        //float newRanking = (oldRanking + ranking) / 2.0f;
        float newRanking = oldRanking + ranking;
        resultList[resultList.Count - 1] = newRanking;
        results[patternInd] = resultList;
    }

    ranking = 0;
    foreach (KeyValuePair<int, float> entry in largestDictionary.Reverse())
    {
        //double thisLargest = entry.Value;
        int patternInd = entry.Key;
        ranking++;

        List<float> resultList = results[entry.Key];
        float oldRanking = resultList.Last();
        //float newRanking = (oldRanking + ranking) / 2.0f;
        float newRanking = oldRanking + ranking;
        resultList[resultList.Count - 1] = newRanking;
        results[patternInd] = resultList;
    }

    ranking = 0;
    foreach (KeyValuePair<int, float> entry in lowestDictionary.Reverse())
    {
        //double thisLowest = entry.Value;
        int patternInd = entry.Key;
        ranking++;

        List<float> resultList = results[entry.Key];
        float oldRanking = resultList.Last();
        float newRanking = oldRanking + ranking;
        //float newRanking = (oldRanking + ranking) / 4.0f;
        resultList[resultList.Count - 1] = newRanking;
        results[patternInd] = resultList;
    }

    ranking = 0;
    foreach (KeyValuePair<int, float> entry in turnDictionary.Reverse())
    {
        //double thisTurns = entry.Value;
        int patternInd = entry.Key;
        ranking++;

        List<float> resultList = results[patternInd];
        float oldRanking = resultList.Last();
        float newRanking = (oldRanking + ranking) / 5.0f;
        resultList[resultList.Count - 1] = newRanking;
        results[patternInd] = resultList;
    }

    return results;
}

float CalculatePercentage(List<int> turnList, bool debugPrint)
{
    //int inBounds = 0;
    int outBounds = 0;
    //We -1 of the count because we don't want to include the current round that might not be done yet.
    for (int i = 0; i < turnList.Count - 1; i++)
    {
        int turnValue = turnList[i];
        if (turnValue > threshold)
        {
            outBounds++;
        }
    }
    if (debugPrint)
    {
        foreach (float j in turnList)
        {
            Console.WriteLine(j);
        }
        Console.WriteLine("OB: " + outBounds + ", Total: " + (turnList.Count - 1));
    }

    //We -1 of the count because we don't want to include the current round that might not be done yet.
    return ((float)outBounds / (float)(turnList.Count - 1)) * 100.0f;
}

string GenerateDummyBet(string finalConsoleOutput)
{
    Random random = new Random();
    var n = random.Next(1, 3);
    finalConsoleOutput = "DUMMY BET: $0.01 @ [ " + n + " ]\n\n";
    if (isTest)
    {
        fileOutput = dummyBet + "," + n;
    }
    else
    {
        fileOutput = dummyBet + "," + n;
    }
    return finalConsoleOutput;
}

string getConsoleOutput()
{
    string finalConsoleOutput = "";
    string consoleOutput = "";
    if (!lastPrediction.Equals(""))
    {
        finalConsoleOutput = lastPrediction + "\n\n";
        lastPrediction = "";
    }
    else if (additionalOutput.Trim().Equals(""))
    {
        finalConsoleOutput = GenerateDummyBet(finalConsoleOutput);
    }
    if (history.Count >= showHistory)
    {
        for (int i = history.Count - showHistory; i < history.Count; i++)
        {
            consoleOutput += history[i] + " ";
        }
        consoleOutput = consoleOutput.Trim();
        if (consoleOutput.Length > 0)
        {
            consoleOutput = "{" + consoleOutput + "} ";
        }
    }
    consoleOutput += "Enter integer: ";
    return finalConsoleOutput + consoleOutput;
}

double findMedian(List<int> a)
{
    int n = a.Count;
    // Check for even case
    if (n % 2 != 0)
        return (double)a[n / 2];

    return (double)(a[(n - 1) / 2] + a[n / 2]) / 2.0;
}

void writeToFile(int newNum)
{
        writer.Write(Environment.NewLine + newNum);
        writer.Flush();
    //File.AppendAllText(@"C:\Crypto2022\SanTan\SanTan\SanTan\data.txt", Environment.NewLine + newNum);
}

// Display the file contents by using a foreach loop.
foreach (string line in lines)
{
    if (line.Equals("1") || line.Equals("2") || line.Equals("3") || line.Equals("4"))
    {
        AddNumber(line, false);
        if (history.Count == historyLength)
        {
            ProcessPatterns(true);
        }
    }
}

void CalculateRequiredBal()
{
    double askingBalance = 0;
    string outputBank = " [" + betList.Count + "] - ";
    //Console.WriteLine("Multiplier: " + multiplier);
    for (int i = 0; i < betList.Count; i++)
    {
        outputBank += (betList[i] * multiplier) + ", ";
        askingBalance += betList[i] * multiplier;// * 3;
    }
    Console.WriteLine("Balance at least required: $" + String.Format("{0:n0}", askingBalance) + outputBank);
}

string val;
bool ask = true;
CalculateRequiredBal();

while (ask)
{
    if (File.Exists(inputFilename))
    {
        //Console input
        //Console.Write(getConsoleOutput());
        //val = Console.ReadLine();
        //File input
        //Console.WriteLine("Input file exists");
        string fileContent = File.ReadAllText(inputFilename);
        if (!string.IsNullOrWhiteSpace(fileContent))
        {
            //Console.WriteLine("Input file is not null");
            if (fileContent.Length != 0)
            {
                //Console.WriteLine("Input file is not emptyl");
                fileOutput = "";
                val = File.ReadLines(inputFilename).First();
                //Console.WriteLine("Input file value: " + val);
                if (val == null || val.Trim().Equals(""))
                {
                    ask = false;
                }
                else if (val.StartsWith("!"))
                {
                    val = val.Substring(1);
                    try
                    {
                        nextBet = int.Parse(val);
                    }
                    catch
                    {

                    }
                }
                else if (val.StartsWith("/"))
                {
                    val = val.Substring(1);
                    try
                    {
                        int divide = int.Parse(val);
                        Console.WriteLine(divide);
                        multiplier = multiplier / divide;
                    }
                    catch
                    {
                        multiplier = baseMultiplier;
                    }
                    CalculateRequiredBal();
                }
                else if (val.StartsWith("<"))
                {

                    val = val.Substring(1);
                    try
                    {
                        focus = int.Parse(val) - 1;
                    }
                    catch
                    {
                        if (val.Equals("v"))
                        {
                            verbose = !verbose;
                        }
                        //Deal with different characters like undo, etc.
                    }
                    nextBet = 0;
                }
                else
                {
                    if (val.Length > 1)
                    {
                        //StreamWriter writer = new StreamWriter(filePath);
                        //writer.Write(string.Empty);
                        //writer.Close();
                        //System.IO.File.WriteAllText(@"/Users/ramiemera/Documents/Rollbot/CoinFlip/SanTan/data.txt", string.Empty);
                        foreach (char c in val)
                        {
                            try
                            {
                                AddNumber(c.ToString(), true);
                            }
                            catch
                            {
                                //Deal with different characters like undo, etc.
                            }
                        }
                    }
                    try
                    {
                        AddNumber(val, true);
                    }
                    catch
                    {
                        //Deal with different characters like undo, etc.
                    }
                    //Do next bet analysis here
                    //nextBet = 0;
                }
                if (history.Count == historyLength)
                {
                    ProcessPatterns(false);
                }
                else
                {
                    GenerateDummyBet("");
                    writeToOutput();
                }
                File.Delete(inputFilename);
            }
        }
    }

}