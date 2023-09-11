using System;
using System.IO;
using System.Text.RegularExpressions;


// Files
// Input
const string inputFilePathT = @"D:\Source\EricRegex\technician.txt";
const string inputFilePathG1 = @"D:\Source\EricRegex\2019-2023GeneralClassPoolFinalRelease.txt";
const string inputFilePathG2 = @"D:\Source\EricRegex\General Class Pool and Syllabus 2023-2027 Public Release with 2nd Errata April 15 2023.txt";
const string inputFilePathE = @"D:\Source\EricRegex\2020ExtraClassPoolJan22.txt";
//
// The inputFilePath will be one of the above text files
//
string inputFilePath = inputFilePathE;

// Output
const string quizPath = @"D:\Source\EricRegex\quiz.txt";
const string quizKeyPath = @"D:\Source\EricRegex\quizKey.txt";




try {
    // Read the entire file into a string
    string fileContent = File.ReadAllText(inputFilePath);

    // The data structures to be loaded with data from the input file
    List<Questions> lQuestions = new List<Questions>();
    List<SubElements> lSubE = new List<SubElements>();
    List<Groups> lGrp = new List<Groups>();
    // because the Group elements occur multiple times in the document (by key), lGrpBag keeps track of whether or not
    // a regex uncovered Group line's data should be added to the lGrp List. When there is an element in lGrpBag it means
    // that this Group has already been seen and should NOT be added to lGrp.
    List<string> lGrpBag = new List<string>();
    // The regular expressions for parsing the input file
    string questionsRE =
             @"(?<QstnKey>" +
                 @"(?<subElem>[EGT][0-9])" +
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
        @"SUBELEMENT\s+(?<subElem>[EGT][0-9])((\s\-\s)|\s*)(?<subElemDesc>.*)\[(?<ExamQCt>[0-9])\s{0,1}[Ee]xam\s*[Qq]uestions{0,1}((\s\-\s)|\s*)(?<groupCt>[1-9][0-9]{0,1})\s*[Gg]roups{0,1}\]\s*(?<questionCt>[1-9][0-9]{0,1})";

    string groupRE =
        //    @"(?:\n)(?<ky>(?<subElem>[EGT][0-9])(?<group>[A-Z]))\s\S\s(?<groupDesc>.*)";
        @"(?:\n)(?<ky>(?<subElem>[EGT][0-9])(?<group>[A-Z]))([\-–\s]|\s\S\s)(?<groupDesc>.*)";

    // Use Regex.Matches to find all occurrences of the pattern
    MatchCollection matchesSubE = Regex.Matches(fileContent, subElementRE);
    // printOut == true causes Technician question data to be written to console (in addition
    //             to the quiz and quizKey data written to files.
    bool printOut = false;

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
     
        if (lGrpBag.Contains(tempGrpKey))
            continue;

        lGrpBag.Add(tempGrpKey);
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
        Questions q = new Questions();

        q.subElem = match.Groups["subElem"].Value.Trim();
        q.group = match.Groups["group"].Value.Trim();
        q.ky = q.subElem + q.group;
        
        q.questionNum = int.Parse(match.Groups["qnum"].Value.Trim());
        q.answer = match.Groups["Ans"].Value.Trim();
        q.documenationReference = match.Groups["Refr"].Value.Trim();
        q.question =  match.Groups["QstnTxt"].Value.Trim();
        q.selectionA = match.Groups["SelA"].Value.Trim();
        q.selectionB = match.Groups["SelB"].Value.Trim();
        q.selectionC = match.Groups["SelC"].Value.Trim();
        q.selectionD = match.Groups["SelD"].Value.Trim();
        if (printOut)
            q.print();
        lQuestions.Add(q);
       }

    if (printOut)
        Console.WriteLine("\n\n");
    {
        List<string> lQuizLine = new List<string>();

        StreamWriter quizKey = new StreamWriter(quizKeyPath);
        // define a randomizer for quiz questions
        Random random = new Random();

        foreach (var grp in lGrp)
        {
            // Determine how many questions there are for this subElement/group. Use random to pick
            // one of those questions.
            int question = random.Next(1, lQuestions.Count(iT => iT.ky == grp.ky) + 1);

            // Pull the random question out of lQuestions List
            var query = from tq in lQuestions
                        where tq.ky == grp.ky &&
                        tq.questionNum == question
                        select tq;

            // write to instructor's quiz key file.
            foreach (var tq in query)
                tq.print(quizKey);

            // Add the question (and section reference, if applicable) to the quiz
            foreach (var tq in query)
                lQuizLine.Add(tq.buildQuizLine());
        }
        quizKey.Close();
        using (StreamWriter quiz = new StreamWriter(quizPath))
        {
            //write the quiz
            foreach (string s in lQuizLine)
                quiz.WriteLine(s);
            quiz.Close();
        }
    }


    Console.WriteLine("Finished");
}
catch (FileNotFoundException)
{
    Console.WriteLine("File " + inputFilePath + " not found.");
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    Environment.Exit(1);
}
