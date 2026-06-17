using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class EmailTemplateBuilder
    {
        public static string Send(string email, string token, string component, string message)
        {
            var encodedToken = Uri.EscapeDataString(token);

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Email Confirmation</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            background-color: #f4f6f8;
            font-family: Arial, Helvetica, sans-serif;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 12px;
            box-shadow: 0 6px 25px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea, #764ba2);
            color: #ffffff;
            padding: 25px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 30px;
            color: #333333;
            line-height: 1.6;
            text-align: center;
        }}
        .content p {{
            font-size: 16px;
            margin-bottom: 30px;
        }}
        .button {{
            display: inline-block;
            padding: 14px 35px;
            background: linear-gradient(135deg, #ff7a18, #ffb347);
            color: #ffffff;
            text-decoration: none;
            border-radius: 30px;
            font-size: 16px;
            font-weight: bold;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }}
        .button:hover {{
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0, 0, 0, 0.3);
        }}
        .footer {{
            background-color: #f4f6f8;
            text-align: center;
            padding: 15px;
            font-size: 12px;
            color: #888888;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{message}</h1>
        </div>
        <div class='content'>
            <p>
                Please click the button below to continue the process.
                This link is valid for a limited time.
            </p>
            <a class='button'
               href='https://petopia.0xcode7.xyz/{component}?email={email}&code={encodedToken}'>
                {message}
            </a>
        </div>
        <div class='footer'>
            <p>If you didn’t request this action, you can safely ignore this email.</p>
        </div>
    </div>
</body>
</html>";
        }
    }

}