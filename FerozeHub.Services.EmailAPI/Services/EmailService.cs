using System.Text;
using FerozeHub.Services.EmailAPI.Data;
using FerozeHub.Services.EmailAPI.Interface;
using FerozeHub.Services.EmailAPI.Models;
using FerozeHub.Services.EmailAPI.Models.Dto;
using FerozeHub.Services.EmailAPI.Utility;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FerozeHub.Services.EmailAPI.Services;

public class EmailService(ApplicationDbContext dbContext):IEmailService
{
    public async Task SendAsync(CartDto cartDto)
    {
        IEnumerable<CartDetailsDto> cartDetailsDtos = cartDto.CartDetails;
        string emailrow = GenerateEmailRows(cartDetailsDtos);
        string emailbody = GenerateEmailBody(emailrow, cartDto);
        StringBuilder message = new StringBuilder();

       
        message.AppendLine("<!DOCTYPE html>");
        message.AppendLine("<html lang=\"en\">");
        message.AppendLine("<head>");
        message.AppendLine("    <meta charset=\"UTF-8\">");
        message.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        message.AppendLine("    <title>Your Email Title</title>");
        message.AppendLine("</head>");
        message.AppendLine("<body>");
        message.AppendLine("    <h2>Cart Email Requested</h2>");
        message.AppendLine($"    <p>Total: ${cartDto.CartHeader.CartTotal}</p>");
        message.AppendLine("    <ul>");
        
        foreach (var item in cartDto.CartDetails)
        {
            message.AppendLine($"        <li>{item.Product.Name} x {item.Count}</li>");
        }
        
        message.AppendLine("    </ul>");
        message.AppendLine("</body>");
        message.AppendLine("</html>");
        
        var emailSent=SendEmail(emailbody,cartDto.CartHeader.Email);
        if (emailSent)
        {
            EmailLogger emailLogger = new()
            {
                Email = cartDto.CartHeader.Email,
                EmailSent = DateTime.Now,
                Message = message.ToString()
            };
            await dbContext.EmailLoggers.AddAsync(emailLogger);
            dbContext.SaveChangesAsync();

        }
        
    }

    private bool SendEmail(string body, string? cartHeaderEmail)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(EmailSD.EmailUsername));
            email.To.Add(MailboxAddress.Parse(cartHeaderEmail));
            email.Subject = "FerozeHub Hurry to Grab your order details";
            email.Body = new TextPart(body);
            using var smtp = new SmtpClient();
            smtp.Connect(EmailSD.EmailHost, 587, SecureSocketOptions.None);
            smtp.Authenticate(EmailSD.EmailUsername, EmailSD.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);

            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    private string GenerateEmailRows(IEnumerable<CartDetailsDto> cartDetailsList)
    {
        StringBuilder emailRows = new StringBuilder();

        string rowTemplate = @"<tr>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                            </tr>";

        int serialNumber = 1; // Assuming you want to start with a serial number

        foreach (var cartDetails in cartDetailsList)
        {
            double totalPrice = cartDetails.Product?.Price * cartDetails.Count ?? 0;

            string emailRow = string.Format(rowTemplate,
                serialNumber++,
                cartDetails.Product?.Name ?? "N/A",
                cartDetails.Product?.Price ?? 0,
                cartDetails.Count,
                totalPrice
               );

            emailRows.AppendLine(emailRow);
        }

        return emailRows.ToString();
    }
    private string GenerateEmailBody(string emailRows,CartDto cartDto)
    {
        string greeting=$"Hi {cartDto.CartHeader?.FirstName} {cartDto.CartHeader?.LastName}";

        string emailBody = $@"<style>table {{border-collapse: collapse;}}th, td {{border: 1px solid black;padding: 8px;}}</style>
    <h2>{greeting}</h2>
    <table>
        <thead>
            <tr>
                <th>Serial No.</th>
                <th>Product Name</th>
                <th>Product Price</th>
                <th>Product Count</th>
                <th>Total Price</th>
            </tr>
        </thead>
        <tbody>{emailRows}</tbody>
    </table>";

        return emailBody;
    }


}