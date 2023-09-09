using System;
using System.IO;
using System.Text.RegularExpressions;


// Files
string inputFilePath = @"D:\Source\EricRegex\technician.txt";
string quizPath = @"D:\Source\EricRegex\quiz.txt";
string quizKeyPath = @"D:\Source\EricRegex\quizKey.txt";

// printOut == true causes Technician question data to be written to console (in addition
//             to the quiz and quizKey data written to files.
bool printOut = false;

// define a randomizer for quiz questions
Random random = new Random();

// define a string to hold the entire file contents in memory
string fileContent = "";

// The data structures to be loaded with data from the input file
List<TechnicianQuestions> lTechQ = new List<TechnicianQuestions>();
List<SubElements> lSubE = new List<SubElements>();
List<Groups> lGrp = new List<Groups>();
List<string> lGroupBag = new List<string>();
List<string> lQuizLine = new List<string>();

// The regular expressions for parsing the input file
string questionsRE =
         @"(?<QstnKey>" +
             @"(?<subElem>T[0-9])" +
             @"(?<group>[A-F])" +
             @"(?<qnum>[0-1][0-9])" +
         @")" +
         @"\s*" +
         @"\((?<Ans>[A-D])\)\s*" +
         @"(\[(?<Refr>.*)\]){0,1}\s*\n" +
     @"(?<QstnTxt>.*\?)\s*\n" +
     @"A\.(?<SelA>.*)\n" +
     @"B\.(?<SelB>.*)\n" +
     @"C\.(?<SelC>.*)\n" +
     @"D\.(?<SelD>.*)\n";

string subElementRE =
    @"SUBELEMENT\s+(?<subElem>T[0-9])\s+.\s+(?<subElemDesc>.*)\s+.\s+\[(?<ExamQCt>[0-9])\s*Exam\s*Questions\s+.\s+(?<groupCt>[1-9][0-9]{0,1})\s*Groups\]\s*(?<questionCt>[1-9][0-9]{0,1})";

string groupRE =
    @"(?<ky>(?<subElem>T[0-9])(?<group>[A-Z]))\s\S\s(?<groupDesc>.*)";


try
{
    // Read the entire file into a string
    fileContent = File.ReadAllText(inputFilePath);
}
catch (FileNotFoundException)
{
    Console.WriteLine("File " + inputFilePath + " not found.");
    Environment.Exit(1);
} 

try { 
     // Use Regex.Matches to find all occurrences of the pattern
    MatchCollection matchesSubE = Regex.Matches(fileContent, subElementRE);
    if (printOut)
        Console.WriteLine($"Found {matchesSubE.Count} sub-element matches:");

    foreach (Match match in matchesSubE)
    {
    
        SubElements s = new SubElements();
        s.subElem = match.Groups["subElem"].Value.Trim();
        s.description = match.Groups["subElemDesc"].Value.Trim();
        s.examQuestionCt = int.Parse(match.Groups["ExamQCt"].Value.Trim());
        s.groupCt = int.Parse(match.Groups["groupCt"].Value.Trim());
        s.questionCt = int.Parse(match.Groups["questionCt"].Value.Trim());
        lSubE.Add(s);
    }
    
    MatchCollection matchesGrp = Regex.Matches(fileContent, groupRE);
    if (printOut)
        Console.WriteLine($"Found {matchesGrp.Count} sub-element/group matches:");
    foreach (Match match in matchesGrp)
    {
        string tempGrpKey = match.Groups["ky"].Value.Trim();
     
        if (lGroupBag.Contains(tempGrpKey))
            continue;

        lGroupBag.Add(tempGrpKey);
        Groups g = new Groups();
        g.subElem = match.Groups["subElem"].Value.Trim();
        g.group = match.Groups["group"].Value.Trim(); ;
        g.ky = g.subElem + g.group;
        g.description = match.Groups["groupDesc"].Value.Trim();
        lGrp.Add(g);
    }

    MatchCollection matchesTechQ = Regex.Matches(fileContent, questionsRE);
    // Display the matched occurrences
    if (printOut)
        Console.WriteLine($"Found {matchesTechQ.Count} Technician Question matches:");
    foreach (Match match in matchesTechQ)
    {
        TechnicianQuestions t = new TechnicianQuestions();

        t.subElem = match.Groups["subElem"].Value.Trim();
        t.group = match.Groups["group"].Value.Trim();
        t.ky = t.subElem + t.group;
        
        t.questionNum = int.Parse(match.Groups["qnum"].Value.Trim());
        t.answer = match.Groups["Ans"].Value.Trim();
        t.documenationReference = match.Groups["Refr"].Value.Trim();
        t.question =  match.Groups["QstnTxt"].Value.Trim();
        t.selectionA = match.Groups["SelA"].Value.Trim();
        t.selectionB = match.Groups["SelB"].Value.Trim();
        t.selectionC = match.Groups["SelC"].Value.Trim();
        t.selectionD = match.Groups["SelD"].Value.Trim();
        if (printOut)
            t.print();
        lTechQ.Add(t);
       }

    if (printOut)
        Console.WriteLine("\n\n");
 
    StreamWriter quizKey = new StreamWriter(quizKeyPath);
    foreach (var grp in lGrp)
    {
        int question = random.Next(1,lTechQ.Count(iT => iT.ky == grp.ky) + 1);

        var query = from tq in lTechQ
                    where tq.ky == grp.ky && 
                    tq.questionNum == question
                    select tq;
 
        foreach (var tq in query)
            tq.print(quizKey);

        foreach (var tq in query)
            lQuizLine.Add(tq.buildQuizLine());
    }
    quizKey.Close();
 
    using (StreamWriter quiz = new StreamWriter(quizPath))
    {
        quiz.WriteLine("\n\nQUIZ:");
        foreach (string s in lQuizLine)
            quiz.WriteLine(s);
        quiz.Close();
    }

    Console.WriteLine("Finished");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    Environment.Exit(1);
}

