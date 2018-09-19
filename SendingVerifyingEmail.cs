using LibDatabase.DbContext;
using LibDatabase.Interaces;
using SendMail.DescriptionVerificationNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.SendEmailVerification
{
    public class SendingVerifyingEmail : ISendingVerifyingEmail
    {
        private readonly IRandomNumber _irandom;
        private readonly IDescriptionVerificationNumber _idesc;
        private readonly ISMTP_Configuration _ismtp;
        private readonly ICheckWerification _icheck;

        public SendingVerifyingEmail(IRandomNumber _irandom, IDescriptionVerificationNumber _desc, ISMTP_Configuration _ismtp, ICheckWerification _icheck)
        {
            this._irandom = _irandom;
            this._idesc = _desc;
            this._ismtp = _ismtp;
            this._icheck = _icheck;
        }
        public int sendEmailToVerification(string email, string nazwisko, string imie)
        {
            int _random = _irandom.generateNumber();

            var _mail = _idesc.create_description(imie,nazwisko, email, _random);
            _ismtp.sendEmail(_mail);
            _icheck.saveTimeWerification(new kartoteka_TMP
            {
                imie = imie,
                nazwisko = nazwisko,
                email = email,
                dataRej = DateTime.Now
            });
            return  _random;
        }
    }
}
