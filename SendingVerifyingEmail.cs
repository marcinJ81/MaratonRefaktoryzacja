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
        private readonly ISendingEmailTimeVerification _isending;

        public SendingVerifyingEmail(IRandomNumber _irandom, IDescriptionVerificationNumber _desc, ISendingEmailTimeVerification _isending)
        {
            this._irandom = _irandom;
            this._idesc = _desc;
            this._isending = _isending;
        }
        public int sendEmailToVerification(string email, string nazwisko, string imie)
        {
            int _random = _irandom.generateNumber();
            var _mail = _idesc.create_description(imie,nazwisko, email, _random);
            _isending.sendEmailInsertTimeVerification(_mail, imie, nazwisko, email);
            return  _random;
        }
    }
}
