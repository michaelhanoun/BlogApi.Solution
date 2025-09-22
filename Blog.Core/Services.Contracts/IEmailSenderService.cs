using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Services.Contracts
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string to , string subject , string body);
    }
}
