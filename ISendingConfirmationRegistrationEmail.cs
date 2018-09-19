using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.SendConfirmRegistration
{
    public interface ISendingConfirmationRegistrationEmail
    {
        void sendWelcomeEmail(string imie, string nazwisko, string email);
    }
}
