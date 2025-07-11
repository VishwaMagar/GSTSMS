using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GSTSMSHelper
{
    public class EmailHelper
    {
        private readonly string fromEmail = ConfigurationManager.AppSettings["OfficialEmail"];

        public async Task<bool> SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                string subject = "Your OTP - Housing Society Login";
                string body = $@"
                <div style='font-family: Poppins, sans-serif; max-width: 600px; margin: auto; background: #f9f9f9; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.05); color: #333;'>
                    <div style='text-align: center; border-bottom: 1px solid #eee; padding-bottom: 20px; margin-bottom: 30px;'>
                        <h1 style='color: #5c6bc0;'>🏢 Housing Society Portal</h1>
                        <p style='font-size: 14px; color: #777;'>Secure • Reliable • Community Focused</p>
                    </div>

                    <p style='font-size: 16px;'>Dear Resident,</p>
                    <p style='font-size: 15px;'>We've received a request to reset your password for your Housing Society account.</p>

                    <div style='margin: 30px 0; text-align: center;'>
                        <p style='font-size: 16px; margin-bottom: 10px;'>Your One-Time Password (OTP) is:</p>
                        <div style='display: inline-block; font-size: 28px; color: #fff; background: #5c6bc0; padding: 10px 30px; border-radius: 8px; letter-spacing: 6px; font-weight: bold;'>{otp}</div>
                        <p style='margin-top: 10px; font-size: 13px; color: #888;'>This OTP is valid for 3 minutes</p>
                    </div>

                    <p style='font-size: 15px;'>Please enter this OTP on the website to continue resetting your password.</p>

                    <div style='margin-top: 40px; border-top: 1px solid #eee; padding-top: 20px; text-align: center;'>
                        <p style='font-size: 13px; color: #aaa;'>Need help? Email us at <a href='mailto:support@gayasofttech.com' style='color: #5c6bc0;'>support@gayasofttech.com</a></p>
                        <p style='font-size: 13px; color: #aaa;'>© {DateTime.Now.Year} Housing Society System. All rights reserved.</p>
                    </div>
                </div>";

                MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(); // pulls SMTP settings from web.config
                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                throw new Exception("SMTP error: " + smtpEx.Message, smtpEx);
            }
            catch (Exception ex)
            {
                throw new Exception("General error while sending OTP: " + ex.Message, ex);
            }
        }

        public async Task<bool> SendEnquiryConfirmationEmail(string toEmail, string chairmanName)
        {
            try
            {
                string subject = "Thank You for Your Enquiry - Housing Society Management";
                //string videoLink = "https://youtu.be/Fgnl6QbQiQk?si=xf_kl7GsKD94p9Lj";

                string body = $@"
                Hi {chairmanName},<br><br>
                Thank you for your interest in our Housing Society Management Platform!<br>
                We’ve received your enquiry and will reach out shortly.<br><br>
                Meanwhile, here’s a quick demo video: 
                <a href='https://youtu.be/Fgnl6QbQiQk?si=xf_kl7GsKD94p9Lj' target='_blank'>Click here to watch</a><br><br>
                Regards,<br>Housing Society Team";

                MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                await smtp.SendMailAsync(mail);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sending enquiry email: " + ex.Message, ex);
            }
        }
        public async Task<bool> SendSubscriptionOtpEmail(string toEmail, string otp)
        {
            try
            {
                string subject = "Your OTP - Housing Society Subscription";
                string body = $@"
                <div style='font-family: Poppins, sans-serif; max-width: 600px; margin: auto; background: #f9f9f9; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.05); color: #333;'>
                <div style='text-align: center; border-bottom: 1px solid #eee; padding-bottom: 20px; margin-bottom: 30px;'>
                    <h1 style='color: #5c6bc0;'>📩 Housing Society Subscription</h1>
                    <p style='font-size: 14px; color: #777;'>Secure your subscription with the OTP below</p>
                </div>

                <p style='font-size: 16px;'>Dear User,</p>
                <p style='font-size: 15px;'>To proceed with your subscription, please verify your email by entering this OTP:</p>

                <div style='margin: 30px 0; text-align: center;'>
                    <div style='display: inline-block; font-size: 28px; color: #fff; background: #5c6bc0; padding: 10px 30px; border-radius: 8px; letter-spacing: 6px; font-weight: bold;'>{otp}</div>
                    <p style='margin-top: 10px; font-size: 13px; color: #888;'>Valid for 3 minutes</p>
                </div>

                <p style='font-size: 15px;'>Thank you for trusting our platform to manage your society better!</p>

                <div style='margin-top: 40px; border-top: 1px solid #eee; padding-top: 20px; text-align: center;'>
                    <p style='font-size: 13px; color: #aaa;'>Need help? Email us at <a href='mailto:support@gayasofttech.com' style='color: #5c6bc0;'>support@gayasofttech.com</a></p>
                    <p style='font-size: 13px; color: #aaa;'>© {DateTime.Now.Year} Housing Society System. All rights reserved.</p>
                </div>
                </div>";

                MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(); // Reads settings from Web.config
                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sending subscription OTP email: " + ex.Message, ex);
            }
        }

        public async Task<bool> SendSubscriptionReceiptEmail(string toEmail, string planName, decimal amount, string duration, DateTime startDate, DateTime expiryDate, string paymentId)
        {
            try
            {
                string subject = "🧾 Your Housing Society Subscription Receipt";

                decimal total = amount;

                //decimal tax = Math.Round(amount * 0.18m, 2);
                //decimal total = amount + tax;

                string body = $@"
                <div style='font-family: Poppins, sans-serif; max-width: 600px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 20px rgba(0,0,0,0.08); color: #333;'>
                    <h2 style='text-align: center; color: #5d3fd3;'>Thank you for subscribing!</h2>
                    <p><strong>Order number:</strong> {paymentId}</p>
                    <p><strong>Order date:</strong> {DateTime.Now.ToString("MMM dd, yyyy hh:mm tt")}</p>
    
                    <hr style='margin: 20px 0;' />

                    <table width='100%' cellpadding='10'>
                        <tr>
                            <td><strong>Plan</strong></td>
                            <td align='right'>{planName} ({duration})</td>
                        </tr>
                        <tr>
                            <td><strong>Base Price</strong></td>
                            <td align='right'>₹{amount:F2}</td>
                        </tr>
                       <tr style='font-weight: bold;'>
                        <td>Total</td>
                        <td align='right'>₹{total:F2}</td>
                    </tr>

                    </table>

                    <hr style='margin: 20px 0;' />

                    <p><strong>Start Date:</strong> {startDate:dd MMM yyyy}</p>
                    <p><strong>Expiry Date:</strong> {expiryDate:dd MMM yyyy}</p>

                    <p style='margin-top: 20px;'>This subscription is now active. Thank you for choosing Housing Society System! 🎉</p>

                    <p style='font-size: 12px; color: #888; margin-top: 40px; text-align: center;'>
                        Need help? Contact <a href='mailto:support@gayasofttech.com'>support@gayasofttech.com</a><br />
                        © {DateTime.Now.Year} Housing Society System. All rights reserved.
                    </p>
                </div>";

                MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(); 
                await smtp.SendMailAsync(mail);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sending subscription receipt email: " + ex.Message, ex);
            }
        }






        public string SendEmailAccountant(string fromEmail, string contactNumber)
        {
            return $@"
<div style='font-family: Poppins, sans-serif; color: #333;'>

   
    <p style='font-size: 12px; color: #777; margin-top: 0; margin-bottom: 15px;'>Important updates regarding your monthly maintenance</p>

    <p style='font-size: 14px; margin-bottom: 10px;'><span style='color:#007bff;'>🤝</span> <strong>Dear Member,</strong></p>

    <p style='font-size: 14px; margin-bottom: 10px;'><span style='color:#28a745;'>😊</span> We hope you are doing well and staying safe.</p>

    <p style='font-size: 14px; margin-bottom: 10px;'><span style='color:#ffc107;'>📣</span><strong> Notice:</strong><br />
    Your <strong>monthly maintenance bill</strong> and society updates are now available.<br />
    You may view the details by logging into the
    <a href='https://yourportal.com' style='color: #007bff; text-decoration: none;'>🌐 Member Portal</a> or visiting the society office.</p>

    <p style='font-size: 14px; margin-bottom: 10px;'><span style='color:#17a2b8;'>💳</span><strong> Payment Reminder:</strong><br />
    Kindly ensure all dues are cleared before the <strong>due date</strong> to avoid late fees.</p>

    <p style='font-size: 14px; margin-bottom: 10px;'><span style='color:#6f42c1;'>❓</span><strong> Need Assistance?</strong><br />
    Reach out to us via email or phone. We’re always happy to help!</p>

    <p style='font-size: 14px; margin-top: 25px; margin-bottom: 0;'>
        <strong>Warm regards,</strong> <br />
<b>Vishwaraj Magar </b>
         <br /> Accountant<br />
        Green Valley Housing Society<br />
        <span style='color:#dc3545;'>✉️ Email:</span> {fromEmail}<br />
        <span style='color:#28a745;'>📞 Phone:</span> {contactNumber}
    </p>
</div>";
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.From = new MailAddress("savitagavali15@gmail.com");

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("savitagavali15@gmail.com", "ndouhycpgvjomitj")
                };

                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }



    }
}
