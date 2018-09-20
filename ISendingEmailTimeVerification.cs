using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace SendMail.SendEmailVerification
{
    public interface ISendingEmailTimeVerification
    {
        void sendEmailInsertTimeVerification(MailMessage email, string imie, string nazwisko, string mail);
    }
}
