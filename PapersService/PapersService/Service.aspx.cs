using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace PapersService
{
    public partial class Service : System.Web.UI.Page
    {
        const string connectionString = "Data Source=localhost\\sqlserver; Integrated Security=true; Initial Catalog=HackMen;";
        protected void Page_Load(object sender, EventArgs e)
        {
            int requestType;
            string searchText = Request.Form["searchText"]??String.Empty;
            string paperText = Request.Form["paperText"] ?? String.Empty;
            string paperID = Request.Form["paperID"] ?? String.Empty;
            string question=Request.Form["question"] ?? String.Empty;
            string question_id = Request.Form["question_id"] ?? String.Empty;
            string comment_content = Request.Form["comment_content"] ?? String.Empty;
            requestType = Convert.ToInt32(Request.Form["requestType"]);
            string responseString=String.Empty;
            switch (requestType)
            {
                case 1: // search papers from tags
                    {
                        responseString = SearchPaperList(searchText);
                        break;
                    }
                case 2: // add questions
                    {
                        responseString = InsertQuestion(paperText, paperID, question);
                        break;
                    }
                case 3: // request for questions
                    {
                        responseString = requestQuestions(paperID);
                        break;
                    }
                case 4: // send answers
                    {
                        responseString = sendAnswers(question_id, comment_content);
                        break;
                    }
                default: {
                    responseString = "{\"res\":\"Invalid request\"}";
                    break;
                }
            }
            Response.Write(responseString);
        }

        protected string SearchPaperList(string searchText)
        {
            string retValue = string.Empty;
             char[] delimiterChars = {';'};
             string[] tags = searchText.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            searchText=string.Empty;
             for (int i = 0; i < tags.Length; i++)
             {

                 searchText += "'" + tags[i].Trim() + "',";
             }
             searchText += "''";
             string query = "select p.PaperID,p.PaperTitle,p.PaperLink from PaperLinks P inner join Tags t on p.PaperID=t.PaperID and t.Tags in (" + searchText + ") group by p.PaperId,p.PaperTitle,p.PaperLink";

            retValue = QueryDatabase(query);
            return retValue;
        }

        protected string InsertQuestion(string paperText, string paperID,string question)
        {
            string retValue = string.Empty;
            string query = "Insert into Questions values ('"+question+"','"+paperText+"',"+paperID+")";
            return retValue = InsertDatabase(query);
        }
        protected string InsertDatabase(string query)
        {
            string retValue = "Successful";
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                retValue = "Error";
            }
            return retValue;
        }
        protected string QueryDatabase(string query)
        {
            string retValue = string.Empty;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            
            retValue = JsonConvert.SerializeObject(dataTable); // Serialization
            connection.Close();
            dataTable.Dispose(); 
            return retValue;
        }

        protected string requestQuestions(string paperID)
        {
            string retValue = string.Empty;
            string query = "select q.question_content, q.question_id, q.PaperText,ISNULL(a.comment_content,'') as comment_content,q.PaperID from questions q left join answers a  on q.question_id=a.question_id where q.PaperID = " + paperID;


            retValue = QueryDatabase(query);
            return retValue;
        }

        protected string sendAnswers(string question_id, string comment_content)
        {
            string retValue = string.Empty;
            string query = "delete from Answers where question_id=" + question_id;
            retValue = InsertDatabase(query);

            query = "insert into Answers values (" +question_id+",'"+ comment_content+"')";
            retValue = InsertDatabase(query);
            return retValue;
        }
    }
}