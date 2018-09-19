using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.SendEmailVerification
{
    public interface ISendingVerifyingEmail
    {
        int sendEmailToVerification(string email, string nazwisko, string imie);
    }
}
