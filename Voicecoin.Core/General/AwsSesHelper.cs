using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voicecoin.Core
{
    public class AwsSesHelper
    {
        private IConfiguration config;

        public AwsSesHelper(IConfiguration config)
        {
            this.config = config;
        }

        private SendEmailRequest PrepareEmailRequest(EmailRequestModel model, String from)
        {
            // Construct an object to contain the recipient address.
            Destination destination = new Destination();
            destination.ToAddresses = model.ToAddresses.Split(',').Select(x => x.Trim()).ToList();
            if (!String.IsNullOrEmpty(model.Bcc))
            {
                destination.BccAddresses = model.Bcc.Split(',').Select(x => x.Trim()).ToList();
            }

            if (!String.IsNullOrEmpty(model.Cc))
            {
                destination.CcAddresses = model.Cc.Split(',').Select(x => x.Trim()).ToList();
            }

            // Create the subject and body of the message.
            Content subject = new Content(model.Subject);

            Body body = new Body();
            body.Html = new Content(model.Body);

            // Create a message with the specified subject and body.
            Message message = new Message(subject, body);

            // Assemble the email.
            return new SendEmailRequest(from, destination, message);
        }

        private AmazonSimpleEmailServiceClient PrepareEmailClient(EmailRequestModel model, IConfiguration config)
        {
            // Choose the AWS region of the Amazon SES endpoint you want to connect to. Note that your sandbox 
            // status, sending limits, and Amazon SES identity-related settings are specific to a given 
            // AWS region, so be sure to select an AWS region in which you set up Amazon SES. Here, we are using 
            // the US West (Oregon) region. Examples of other regions that Amazon SES supports are USEast1 
            // and EUWest1. For a complete list, see http://docs.aws.amazon.com/ses/latest/DeveloperGuide/regions.html 
            Amazon.RegionEndpoint REGION = Amazon.RegionEndpoint.USEast1;

            // Instantiate an Amazon SES client, which will make the service call.
            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(config.GetSection("AWS:AWSAccessKey").Value, config.GetSection("AWS:AWSSecretKey").Value, REGION);

            client.BeforeRequestEvent += delegate (object sender, RequestEventArgs e)
            {
                WebServiceRequestEventArgs args = e as WebServiceRequestEventArgs;
                SendEmailRequest request = args.Request as SendEmailRequest;

                //$"Sending email {model.Subject} to {model.ToAddresses}".Log();
            };

            client.ExceptionEvent += delegate (object sender, ExceptionEventArgs e)
            {
                Console.WriteLine($"Sent email {model.Subject} error: {e.ToString()}");
            };

            client.AfterResponseEvent += delegate (object sender, ResponseEventArgs e)
            {
                WebServiceResponseEventArgs args = e as WebServiceResponseEventArgs;
                SendEmailResponse response = args.Response as SendEmailResponse;

                //$"Sent email {model.Subject} to {model.ToAddresses} {response.HttpStatusCode} {response.MessageId}".Log();
            };

            return client;
        }

        private void Client_BeforeRequestEvent(object sender, RequestEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Send(EmailRequestModel model, IConfiguration config)
        {
            if (String.IsNullOrEmpty(model.ToAddresses)) return String.Empty;

            SendEmailRequest request = PrepareEmailRequest(model,  config.GetSection("AWS:SESVerifiedEmail").Value);
            model.From = request.Source;

            AmazonSimpleEmailServiceClient client = PrepareEmailClient(model, config);

            SendEmailResponse response = await client.SendEmailAsync(request);
            return response.MessageId;
        }
    }

    public class EmailRequestModel
    {
        public string Subject { get; set; }
        /// <summary>
        /// Support mutiple addresses seperated by comma
        /// </summary>
        public string ToAddresses { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        /// <summary>
        /// Template file name
        /// </summary>
        public string Template { get; set; }
    }
}
