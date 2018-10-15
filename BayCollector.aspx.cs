using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using AIMLbot;
using System.IO;

public partial class BayCollector : System.Web.UI.Page
{

    const string UserId = "xyz";
    const string UserName = "Alper";
    private static Bot AimlBot;
    private static User myUser;

    string hash = "";
    public static string path = "";


    protected void Page_Load(object sender, EventArgs e)
    {

        //path = System.IO.Path.Combine(Server.MapPath(""), System.IO.Path.Combine("config", "Settings.xml"));
        path = @"D:\ALPER\AIML\";

        hash = DateTime.Now.Ticks.ToString().Substring(5);
        AimlBot = new Bot();
        myUser = new User(UserId, AimlBot);


        string yol = System.IO.Path.Combine(path, System.IO.Path.Combine("config", "Settings.xml"));
        AimlBot.loadSettings(yol);
        AimlBot.isAcceptingUserInput = false;
        AimlBot.loadAIMLFromFiles();
        AimlBot.isAcceptingUserInput = true;
        loadXML();

        txtUserId.Text = UserId;
        txtUserName.Text = UserName;
        txtSessionId.Text = hash;
    }
    public void CuteRobot()
    {
        //AimlBot = new Bot();
        //myUser = new User(UserId, AimlBot);
        //Initialize();
        //loadXML();
    }

    // Loads all the AIML files in the \AIML folder         
    public void Initialize()
    {

    }
    public void loadXML()
    {
        XmlDocument document = new XmlDocument();
        if (!File.Exists(path + UserId + ".xml"))
        {
            //Populate with data here if necessary, then save to make sure it exists
            //document.Save(path + UserId + ".xml");
            myUser.Predicates.DictionaryAsXML.Save(path + UserId + ".xml");
        }
        else
        {
            //       document.load(path + UserId + ".xml");
            //     myUser.Predicates.DictionaryAsXML.Save("C:\\source\\AIMLWebService\\aiml\\Default.aiml");
            myUser.Predicates.loadSettings(path + UserId + ".xml");
        }


    }
    public static String getOutput(String input)
    {

        Request r = new Request(input, myUser, AimlBot);
        Result res = AimlBot.Chat(r);

        myUser.Predicates.DictionaryAsXML.Save(path + UserId + ".xml");
        string Output = res.Output;

        return (Output);
    }
    [WebMethod]
    public static string Ask(string Input, string LU, string LB, string BG, string Ses, string BA)
    {
        System.Threading.Thread.Sleep(500);
        string output = "";
        if (BG == "1")
        {
            output = BA;
            AddToDB(Ses, LU, LB, Input, BA, BG);
        }
        else
        {
            output = getOutput(Input);
            AddToDB(Ses, LU, LB, Input, output, BG);
        }

        return output;
    }
    public static string Convert(string input)
    {

        return input;
    }
    private static void AddToDB(string Session, string LastUserInput, string LastBotInput, string UserInput, string BotInput, string LikeBot)
    {

        
        /*
        use aiml_db;
CREATE TABLE [dbo].[Bot_Tbl_Logs](
	[IntCode] [int] IDENTITY(1,1) NOT NULL,
	[SID] [varchar](50) NULL,
	[UID] [varchar](50) NULL,
	[CDT] [datetime] NULL,
	[LastUserInput] [varchar](500) NULL,
	[LastBotInput] [varchar](500) NULL,
	[UserInput] [varchar](500) NULL,
	[BotInput] [varchar](500) NULL,
	[LikeBot] [tinyint] NULL
) ON [PRIMARY]
    */
        string LU = Convert(LastUserInput);
        string LB = Convert(LastBotInput);
        string UI = Convert(UserInput);
        string BI = Convert(BotInput);
        string query = "INSERT INTO Bot_Tbl_Logs(SID,UID, CDT, LastUserInput, LastBotInput, UserInput, BotInput, LikeBot) VALUES('" + Session + "','" + UserId + "',GETDATE(),'" + LU + "','" + LB + "','" + UI + "','" + BI + "'," + LikeBot.ToString() + ");";
        PCL.MsSQL_DBOperations.ExecuteSQLStr(query, "aiml_db");


    }
}
