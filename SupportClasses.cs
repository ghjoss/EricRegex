using System;
using System.Runtime.InteropServices;

public class Sub_elements
{
	public string subElem = "";
    public int questionCt = 0;
    public int examQuestionCt = 0 ;
    public int groupCt = 0;
	public string description = "";
}

public class Groups
{
    public string subElem = "";
	public string group = "";
	public string description = "";
}

public class TechnicianQuestions
{
    public string subElem = "";
    public string group = "";
    public string questionNum = "";
    public string documenationReference = "";
    public string question = "";
    public string selectionA = "";
    public string selectionB = "";
    public string selectionC = "";
    public string selectionD = "";
    public string answer = "";
    
    public void TechnicianQuestion()
    {

    }
    public void print()
    {
        Console.WriteLine("QstnKey: " + subElem + group + questionNum);
        Console.WriteLine("Ans    : " + answer);
        Console.WriteLine("Refr   : " + documenationReference);
        Console.WriteLine("QstnTxt: " + question);
        Console.WriteLine("SelA   : " + selectionA);
        Console.WriteLine("SelB   : " + selectionB);
        Console.WriteLine("SelC   : " + selectionC);
        Console.WriteLine("SelD   : " + selectionD);

    }
}
