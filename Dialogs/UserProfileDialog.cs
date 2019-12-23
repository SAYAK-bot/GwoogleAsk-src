using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
namespace EchoBot.Dialogs
{

    public class UserProfileDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        //private readonly LuisRecognizer _luisRecognizer;
        public UserProfileDialog()
            : base(nameof(UserProfileDialog))
        {
            //_userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");
            //_luisRecognizer = luisRecognizer;
            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                DomainStepAsync,
                NameStepAsync,
                NameConfirmStepAsync,
                QuestionStepAsync,
                MailStepAsync,
                //AgeStepAsync,
                //PictureStepAsync,
                //ConfirmStepAsync,
                //SummaryStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AgePromptValidatorAsync));
            //AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            //AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            //AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt), PicturePromptValidatorAsync));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> DomainStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Please enter domain of information."),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Policy Center", "Claims Center", "Billing Center" }),
                    }, cancellationToken);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                stepContext.Values["domain"] = ((FoundChoice)stepContext.Result).Value;

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                stepContext.Values["name"] = (string)stepContext.Result;

                // We can send messages to the user at any point in the WaterfallStep.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What information are you looking for?") }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
          }

        private  async Task<DialogTurnResult> QuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                stepContext.Values["Question"] = ((String)stepContext.Result);
               
                //var luisResult = await _luisRecognizer.RecognizeAsync<RecognizerResult>(stepContext.Context, cancellationToken);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your mail id.") }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private  async Task<DialogTurnResult> MailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                
                stepContext.Values["Mail"] = ((String)stepContext.Result);
               var ListQuestions= findLinks(stepContext);
                sendMail(stepContext,ListQuestions);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text($"Thank you {stepContext.Values["name"]} We will get back to you") }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
          }
        public static List<string> findLinks(WaterfallStepContext stepContext)
        {

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "gwoogle.database.windows.net";
                builder.UserID = "Sayak";
                builder.Password = "jolu@ECE15";
                builder.InitialCatalog = "linkDetails";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                   
                    connection.Open();

                    List<Link> result = new List<Link>();
                    List<string> questionsList = new List<string>();
                    using (SqlCommand command = new SqlCommand("getLinks", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        var dom = stepContext.Values["domain"].ToString().Substring(0, stepContext.Values["domain"].ToString().IndexOf(" "));
                        command.Parameters.AddWithValue("@domain", dom);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //    Link lnk = new Link();
                                //    lnk.LinkUrl = reader[0].ToString();
                                //    lnk.LinkSubject= reader[1].ToString();
                                //    lnk.LinkCategory = reader[2].ToString();
                                //    result.Add(lnk);
                                if (stepContext.Values["Question"].ToString().ToLower().Contains(reader[1].ToString().ToLower()))
                                {
                                    questionsList.Add(reader[0].ToString());
                                }
                            }
                        }
                      return questionsList;
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    
        public static void sendMail(WaterfallStepContext stepContext,List<string> LinkUrlsList)
        {
            try
            {

                MailAddress fromAddress = new MailAddress("sayak.biswas@hotmail.com", "Gwoogle");
                MailAddress toAddress = new MailAddress(stepContext.Values["Mail"].ToString(), stepContext.Values["name"].ToString());
                const string fromPassword = "sayakSICK";
                string body = System.IO.File.ReadAllText(@"C:\Users\SayAk\Downloads\GwoogleAsk-src\emailTemplate.html");
                //string format
                body = body.Replace("&&Customer", stepContext.Values["name"].ToString()).Replace("&sample@email.com", stepContext.Values["Mail"].ToString()).Replace("&&Lorem", LinkUrlsList[0].ToString());
                const string subject = "Reply from Gwoogle";//email subject
                SmtpClient smtpClient = new SmtpClient()
                {
                    Host = "smtp.live.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (MailMessage msg = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtpClient.Send(msg);
                    msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                    if ((int)msg.DeliveryNotificationOptions == 1)//deliver success notification
                    {
                        // TempData["code"] = g.ToString();
                    }

                }
            }
            catch(Exception ex){
                throw ex;
            }
        }
      
    }
}
