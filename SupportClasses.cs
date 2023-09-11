using System;
using System.Runtime.InteropServices;

public class SubElements
{
	public string subElem = "";
    public int questionCt = 0;
    public int examQuestionCt = 0 ;
    public int groupCt = 0;
	public string description = "";
}

public class Groups
{
    public string ky = "";
    public string subElem = "";
	public string group = "";
	public string description = "";
}

public class Questions
{
    public string ky = "";
    public string subElem = "";
    public string group = "";
    public int questionNum = 0;
    public string documenationReference = "";
    public string question = "";
    public string selectionA = "";
    public string selectionB = "";
    public string selectionC = "";
    public string selectionD = "";
    public string answer = "";
    
    public void print()
    {
        Console.WriteLine("QstnKey: " + subElem + group + questionNum.ToString("00"));
        Console.WriteLine("Ans    : " + answer);
        Console.WriteLine("Refr   : " + documenationReference);
        Console.WriteLine("QstnTxt: " + question);
        Console.WriteLine("SelA   : " + selectionA);
        Console.WriteLine("SelB   : " + selectionB);
        Console.WriteLine("SelC   : " + selectionC);
        Console.WriteLine("SelD   : " + selectionD);
    }

    public void print(StreamWriter qk)
    {
        qk.WriteLine("QstnKey: " + subElem + group + questionNum.ToString("00"));
        qk.WriteLine("Ans    : " + answer);
        qk.WriteLine("Refr   : " + documenationReference);
        qk.WriteLine("QstnTxt: " + question);
        qk.WriteLine("SelA   : " + selectionA);
        qk.WriteLine("SelB   : " + selectionB);
        qk.WriteLine("SelC   : " + selectionC);
        qk.WriteLine("SelD   : " + selectionD);

    }
    public void printKey()
    {
        Console.WriteLine("QstnKey: " + subElem + group + questionNum.ToString("00"));
    }

    public string buildQuizLine()
    {
        return question + "\n" + 
            "\tA: " + selectionA + "\n" +
            "\tB: " + selectionB + "\n" +
            "\tC: " + selectionC + "\n" +
            "\tD: " + selectionD +
            (documenationReference != "" ? "\nReference: " + documenationReference + "\n":"") + "\n";
         }
}
