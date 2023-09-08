using System;
using System.IO;
using System.Text.RegularExpressions;

string filePath = @"D:\Downloads\technician.txt"; // Console.ReadLine();
bool printOut = false;

List<TechnicianQuestions> lTechQ = new List<TechnicianQuestions>();
List<Sub_elements> lSubE = new List<Sub_elements>();
List<Groups> lGrp = new List<Groups>();
// The "group" matching regular expression has multiple matches on the group description lines due to each line
// being repeated twice in the document. We need to keep track of what is added to the group list in a separate
// list that keeps track of only the group key and not its description.
List<string> groupBag = new List<string>();

try
{
    // Read the entire file into a string
    string fileContent = File.ReadAllText(filePath);

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

    // Use Regex.Matches to find all occurrences of the pattern
    MatchCollection matchesSubE = Regex.Matches(fileContent, subElementRE);

    foreach (Match match in matchesSubE)
    {
    
        Sub_elements s = new Sub_elements();
        s.subElem = match.Groups["subElem"].Value.Trim();
        s.description = match.Groups["subElemDesc"].Value.Trim();
        s.examQuestionCt = int.Parse(match.Groups["ExamQCt"].Value.Trim());
        s.groupCt = int.Parse(match.Groups["groupCt"].Value.Trim());
        s.questionCt = int.Parse(match.Groups["questionCt"].Value.Trim());
        lSubE.Add(s);
    }
    
    MatchCollection matchesGrp = Regex.Matches(fileContent, groupRE);
    foreach (Match match in matchesGrp)
    {
        string tempGrpKey = match.Groups["ky"].Value.Trim();
     
        if (groupBag.Contains(tempGrpKey))
            continue;

        groupBag.Add(tempGrpKey);
        Groups g = new Groups();
        g.subElem = match.Groups["subElem"].Value.Trim();
        g.group = match.Groups["group"].Value.Trim(); ;
        g.description = match.Groups["groupDesc"].Value.Trim();
        lGrp.Add(g);
    }

    MatchCollection matchesTechQ = Regex.Matches(fileContent, questionsRE);
    // Display the matched occurrences
    if (printOut)
        Console.WriteLine($"Found {matchesTechQ.Count} matches:");
    foreach (Match match in matchesTechQ)
    {
        TechnicianQuestions t = new TechnicianQuestions();

        t.subElem = match.Groups["subElem"].Value.Trim();
        

        t.questionNum = match.Groups["qnum"].Value.Trim();
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
    // select all subElement T1 questions
    var query = from TechnicianQuestion in lTechQ
                where TechnicianQuestion.subElem == "T1"
                & TechnicianQuestion.questionNum != "04"
                select TechnicianQuestion;
    foreach (var tq in query)
        tq.print();
    Console.WriteLine("Finished");
}
catch (FileNotFoundException)
{
    Console.WriteLine("File not found.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
