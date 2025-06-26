using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
public partial class ChatWindow : System.Web.UI.Page
{
    string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
    public string empCode;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserCode"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            empCode = Session["UserCode"].ToString();
            if (!IsPostBack)
            {
                string receiverId = Request.QueryString["receiverid"];
                if (!string.IsNullOrEmpty(receiverId))
                {
                    hdnReceiverID.Value = receiverId;
                }
                BindUserList();

                hdnSenderID.Value = Session["ID"].ToString(); 

            }
        }

    }
    private void BindUserList()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "SELECT ID AS Userid, Username,(SELECT COUNT(*) FROM ChatMessages  WHERE ReceiverId = U.id AND IsSeen = 0) AS UnreadCount FROM tbl_UserMaster AS U where status=1 and isdeleted=0";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@empCode", empCode);
            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            rptUsers.DataSource = dr;
            rptUsers.DataBind();
        }
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        int senderId = int.Parse(hdnSenderID.Value);
        int receiverId = int.Parse(hdnReceiverID.Value);
        string message = txtMessage.Text.Trim();

        if (!string.IsNullOrEmpty(message))
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "INSERT INTO ChatMessages (SenderID, ReceiverID, Message) VALUES (@SenderID, @ReceiverID, @Message)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SenderID", senderId);
                cmd.Parameters.AddWithValue("@ReceiverID", receiverId);
                cmd.Parameters.AddWithValue("@Message", message);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            txtMessage.Text = "";
            // LoadChat();
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string LoadChatAjax(string senderId, string receiverId)
    {
        StringBuilder chatHtml = new StringBuilder();
        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            string updateQuery = "UPDATE ChatMessages SET IsSeen = 1 WHERE ReceiverID = @ReceiverID AND SenderID = @SenderID AND IsSeen = 0";
            SqlCommand updateCmd = new SqlCommand(updateQuery, con);
            updateCmd.Parameters.AddWithValue("@ReceiverID", senderId); // because sender now becomes the receiver
            updateCmd.Parameters.AddWithValue("@SenderID", receiverId);
            con.Open();
            updateCmd.ExecuteNonQuery();

            // Then load the chat messages
            string selectQuery = "SELECT * FROM ChatMessages WHERE (SenderID = @SenderID AND ReceiverID = @ReceiverID) OR (SenderID = @ReceiverID AND ReceiverID = @SenderID) ORDER BY Sentat";
            SqlCommand selectCmd = new SqlCommand(selectQuery, con);
            selectCmd.Parameters.AddWithValue("@SenderID", senderId);
            selectCmd.Parameters.AddWithValue("@ReceiverID", receiverId);
            SqlDataReader dr = selectCmd.ExecuteReader();
        

            while (dr.Read())
            {
                string msg = dr["Message"].ToString();
                string cssClass = (int)dr["SenderID"] == int.Parse(senderId) ? "me" : "you";
                string time = Convert.ToDateTime(dr["SentAt"]).ToString("hh:mm tt");

                chatHtml.Append(string.Format(
    "<div class='chat-message {0}'>" +
    "<div class='msg-text'>{1}</div>" +
    "<div class='msg-time'>{2}</div>" +
    "</div>", cssClass, msg, time));
            }
            con.Close();
        }

        return chatHtml.ToString();
    }
    [WebMethod]
    public static void SendMessageAjax(string senderId, string receiverId, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "INSERT INTO ChatMessages (SenderID, ReceiverID, Message, SentAt) VALUES (@SenderID, @ReceiverID, @Message, GETDATE())";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SenderID", senderId);
            cmd.Parameters.AddWithValue("@ReceiverID", receiverId);
            cmd.Parameters.AddWithValue("@Message", message);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    protected void idbuttonback_Click(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            Response.Redirect("~/Admin/Dashboard.aspx");
        }

    }
}