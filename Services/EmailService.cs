using ReaB_Bakery.Models;
using System.Net;
using System.Net.Mail;

namespace ReaB_Bakery.Data
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendOrderConfirmationAsync(
            string toEmail,
            string customerName,
            string paymentMethod,
            decimal total)
        {
            try
            {
                var smtp = _config.GetSection("SmtpSettings");

                if (!int.TryParse(smtp["Port"], out int port))
                    throw new InvalidOperationException("Invalid SMTP port");

                using var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                    Subject = $"Order Confirmation - Lindi's Home Bakery",
                    IsBodyHtml = true,
                    Body = $@"
                        <h2>Thank you for your order, {customerName}!</h2>
                        <p>Your order has been received successfully.</p>
                        <p><strong>Payment Method:</strong> {paymentMethod}</p>
                        <p><strong>Total Amount:</strong> R {total:F2}</p>
                        <p>We'll contact you shortly regarding delivery.</p>
                        <br />
                        <p>❤️ Lindi's Home Bakery</p>"
                };

                message.To.Add(toEmail);
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order confirmation email");
            }
        }

        //CUSTOMIZATION OF ORDER
        public async Task SendCustomizationRequestAsync(
      string customerName,
      string customerEmail,
      string customerPhone,
      string productName,
      DateTime eventDate,
      string designNotes,
      string photoPath)
        {
            try
            {
                var smtp = _config.GetSection("SmtpSettings");
                if (!int.TryParse(smtp["Port"], out int port))
                    throw new InvalidOperationException("Invalid SMTP port");

                var adminEmail = _config["AdminSettings:AdminEmail"];

                // Email to bakery owner
                var adminBody = $@"
            <h2 style='color: rgb(235,67,126);'>New Customization Request 🎂</h2>
            <table style='font-family: Arial; font-size: 14px; border-collapse: collapse;'>
                <tr><td style='padding: 8px; font-weight:bold;'>Customer:</td>
                    <td style='padding: 8px;'>{customerName}</td></tr>
                <tr><td style='padding: 8px; font-weight:bold;'>Email:</td>
                    <td style='padding: 8px;'><a href='mailto:{customerEmail}'>{customerEmail}</a></td></tr>
                <tr><td style='padding: 8px; font-weight:bold;'>Phone:</td>
                     <td style='padding: 8px;'><a href='tel:{customerPhone}'>{customerPhone}</a></td></tr>
                <tr><td style='padding: 8px; font-weight:bold;'>Product:</td>
                    <td style='padding: 8px;'>{productName}</td></tr>
                <tr><td style='padding: 8px; font-weight:bold;'>Delivery Date:</td>
                    <td style='padding: 8px;'>{eventDate:dd MMMM yyyy}</td></tr>
                <tr><td style='padding: 8px; font-weight:bold;'>Design Notes:</td>
                    <td style='padding: 8px;'>{designNotes}</td></tr>
                {(photoPath != null ? "<tr><td style='padding: 8px; font-weight:bold;'>Photo:</td><td style='padding: 8px;'>Uploaded — check your admin dashboard</td></tr>" : "")}
            </table>
            <br/>
            <p style='color:#888; font-size:12px;'>Sent from ReaB's Bakery website</p>";

                using (var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                })
                {
                    using var adminMessage = new MailMessage
                    {
                        From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                        Subject = $"New Customization Request — {productName} for {customerName}",
                        IsBodyHtml = true,
                        Body = adminBody
                    };
                    adminMessage.To.Add(adminEmail);
                    await client.SendMailAsync(adminMessage);
                }

                // Confirmation email to customer
                var customerBody = $@"
            <h2 style='color: rgb(235,67,126);'>We got your request! 🎉</h2>
            <p>Hi {customerName},</p>
            <p>Thank you for your customization request for <strong>{productName}</strong>.</p>
            <p>We'll review your design notes and get back to you as soon as possible to confirm details and pricing.</p>
            <br/>
            <p><strong>Your delivery date:</strong> {eventDate:dd MMMM yyyy}</p>
            <p><strong>Your notes:</strong> {designNotes}</p>
            <br/>
            <p>With love 🩷,<br/><strong>ReaB's Bakery</strong></p>";

                using (var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                })
                {
                    using var customerMessage = new MailMessage
                    {
                        From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                        Subject = "We received your customization request — ReaB's Bakery",
                        IsBodyHtml = true,
                        Body = customerBody
                    };
                    customerMessage.To.Add(customerEmail);
                    await client.SendMailAsync(customerMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send customization request email");
            }
        }
        // END OF CUSTOMIZATION OF ORDER

        public async Task SendCustomOrderAsync(CustomOrder order, string flavour, string colour)
        {
            try
            {
                var smtp = _config.GetSection("SmtpSettings");
                if (!int.TryParse(smtp["Port"], out int port))
                    throw new InvalidOperationException("Invalid SMTP port");

                var adminEmail = _config["AdminSettings:AdminEmail"];

                var adminBody = $@"
            <h2 style='color:rgb(235,67,126);'>🎂 New Custom Order — {order.OrderReference}</h2>
            <table style='font-family:Arial;font-size:14px;border-collapse:collapse;'>
                <tr><td style='padding:8px;font-weight:bold;'>Order Ref:</td>
                    <td style='padding:8px;'>{order.OrderReference}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Customer:</td>
                    <td style='padding:8px;'>{order.CustomerName}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Email:</td>
                    <td style='padding:8px;'><a href='mailto:{order.CustomerEmail}'>{order.CustomerEmail}</a></td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Phone:</td>
                    <td style='padding:8px;'><a href='tel:{order.CustomerPhone}'>{order.CustomerPhone}</a></td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Product:</td>
                    <td style='padding:8px;'>{order.ProductName}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Flavour:</td>
                    <td style='padding:8px;'>{flavour}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Colour:</td>
                    <td style='padding:8px;'>{colour}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Delivery Date:</td>
                    <td style='padding:8px;'>{order.DeliveryDate:dd MMMM yyyy}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Design Notes:</td>
                    <td style='padding:8px;'>{order.DesignNotes}</td></tr>
                {(order.InspirationPhotoPath != null ? "<tr><td style='padding:8px;font-weight:bold;'>Photo:</td><td style='padding:8px;'>Uploaded — check admin dashboard</td></tr>" : "")}
            </table>
            <br/>
            <a href='https://wa.me/27760708055' style='background:rgb(235,67,126);color:white;padding:10px 20px;border-radius:999px;text-decoration:none;font-weight:bold;'>
                Open WhatsApp
            </a>";

                using (var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                })
                {
                    using var msg = new MailMessage
                    {
                        From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                        Subject = $"New Order {order.OrderReference} — {order.ProductName}",
                        IsBodyHtml = true,
                        Body = adminBody
                    };
                    msg.To.Add(adminEmail);
                    await client.SendMailAsync(msg);
                }

                // Customer confirmation
                var customerBody = $@"
            <h2 style='color:rgb(235,67,126);'>We received your order! 🎉</h2>
            <p>Hi {order.CustomerName},</p>
            <p>Thank you for ordering from <strong>ReaB's Bakery</strong>!</p>
            <br/>
            <table style='font-family:Arial;font-size:14px;border-collapse:collapse;'>
                <tr><td style='padding:8px;font-weight:bold;'>Order Reference:</td>
                    <td style='padding:8px;'><strong>{order.OrderReference}</strong></td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Product:</td>
                    <td style='padding:8px;'>{order.ProductName}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Flavour:</td>
                    <td style='padding:8px;'>{flavour}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Colour:</td>
                    <td style='padding:8px;'>{colour}</td></tr>
                <tr><td style='padding:8px;font-weight:bold;'>Delivery Date:</td>
                    <td style='padding:8px;'>{order.DeliveryDate:dd MMMM yyyy}</td></tr>
            </table>
            <br/>
            <div style='background:#fff0f5;border-left:4px solid rgb(235,67,126);padding:16px;border-radius:8px;'>
                <p style='font-weight:bold;margin:0;'>💳 Deposit Payment</p>
                <p style='margin:8px 0 0;'>To confirm your order, please pay your deposit and use 
                <strong>{order.OrderReference}</strong> as your payment reference.</p>
                <p>Bank: <strong>FNB</strong><br/>
                Account Name: <strong>ReaB Bakery</strong><br/>
                Account Number: <strong>XXXXXXXXXX</strong></p>
            </div>
            <br/>
           
            <p>We'll confirm your order via WhatsApp or email once your deposit is received.</p>
            <a href='https://wa.me/27760708055' style='background:rgb(235,67,126);color:white;padding:10px 20px;border-radius:999px;text-decoration:none;font-weight:bold;'>
                💬 Send Proof on WhatsApp
            </a>
            </br>
            <p>With love 🩷,<br/><strong>ReaB's Bakery</strong></p>
            
";

                using (var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                })
                {
                    using var msg = new MailMessage
                    {
                        From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                        Subject = $"Your ReaB's Bakery Order — {order.OrderReference}",
                        IsBodyHtml = true,
                        Body = customerBody
                    };
                    msg.To.Add(order.CustomerEmail);
                    await client.SendMailAsync(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send custom order email");
            }
        }




        public async Task SendAdminOrderNotificationAsync(Order order)
        {
            try
            {
                // Get SMTP settings
                var smtp = _config.GetSection("SmtpSettings");

                if (!int.TryParse(smtp["Port"], out int port))
                    throw new InvalidOperationException("Invalid SMTP port");

                using var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = smtp.GetValue<bool>("EnableSsl")
                };

                // Get admin emails
                var adminSettings = _config.GetSection("AdminSettings");
                var adminEmails = new List<string> { adminSettings["AdminEmail"] };

                if (!string.IsNullOrEmpty(adminSettings["AdditionalAdminEmails"]))
                {
                    adminEmails.AddRange(adminSettings["AdditionalAdminEmails"]
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => e.Trim()));
                }

                // Compose email
                using var message = new MailMessage
                {
                    From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                    Subject = $"New Order Received - {order.OrderNumber}",
                    IsBodyHtml = true,
                    Body = $@"
                <h2>New Order Received!</h2>
                <p><strong>Order Number:</strong> {order.OrderNumber}</p>
                <p><strong>Customer:</strong> {order.CustomerName} ({order.CustomerEmail})</p>
                <p><strong>Phone:</strong> {order.CustomerPhone}</p>
                <p><strong>Payment Method:</strong> {order.PaymentMethod}</p>
                <p><strong>Total:</strong> R {order.Total:F2}</p>
                <h3>Order Items:</h3>
                <ul>
                    {string.Join("", order.Items.Select(i => $"<li>{i.ProductName} x {i.Quantity} - R {i.TotalPrice:F2}</li>"))}
                </ul>
                <p><a href='{adminSettings["AdminUrl"]}'>View in Admin Dashboard</a></p>"
                };

                // Add all admin emails
                foreach (var email in adminEmails)
                    message.To.Add(email);

                // Send email
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin notification email");
            }
        }


    }
}
