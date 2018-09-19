using LibDatabase.DbContext;
using LibDatabase.Interaces;
using SendMail.DescriptionEndMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.SendConfirmRegistration
{
    public class SendingConfirmationRegistrationEmail : ISendingConfirmationRegistrationEmail
    {
        private readonly IDescriptionEndCreate _iend;
        private readonly IDataForDescription _idataDescription;
        private readonly ISMTP_Configuration _ismtp;
        private readonly ICheckWerification _icheck;

        public SendingConfirmationRegistrationEmail(IDescriptionEndCreate _iend, IDataForDescription _idataDescription
                , ISMTP_Configuration _ismtp, ICheckWerification _icheck)
        {
            this._iend = _iend;
            this._idataDescription = _idataDescription;
            this._icheck = _icheck;
            this._ismtp = _ismtp;
        }
        public void sendWelcomeEmail(string imie, string nazwisko, string email)
        {
            string mail_description = _iend.createDescription(_idataDescription.get_DataFoDescritpion(imie, nazwisko,email));
            var info = _ismtp.createResponse(new EmailDescription()
            {
                name = imie,
                surname = nazwisko,
                email_adress = email,
                sender = "mszana2017@maraton24.hw7.pl",
                randomNumber = 0,
                recipient = email,
                subject = "Potwierdzenie Rejestracji X Maraton Rowerowy Mszana 2019"
            }, mail_description);
            _ismtp.sendEmail(info);

            _icheck.saveTimeWerification(new kartoteka_TMP
            {
                imie = imie,
                nazwisko = nazwisko,
                email = email,
                dataKoncowa = DateTime.Now,
                rejestracja = 1
            });
        }
    }
}
