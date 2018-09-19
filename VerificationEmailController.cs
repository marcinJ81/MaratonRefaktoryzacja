using LibDatabase.DbContext;
using LibDatabase.Interaces;
using maratonMszana_v4.VerificationData;
using SendMail;
using SendMail.DescriptionEndMail;
using SendMail.DescriptionVerificationNumber;
using SendMail.SendConfirmRegistration;
using SendMail.SendEmailVerification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThrowException.Interface;

namespace maratonMszana_v4.Controllers
{
    public class VerificationEmailController : Controller
    {
        private readonly IDataVerification _idata;
        private readonly IUniqueException _ithrow;
        private readonly ISendingVerifyingEmail _isendingVer;
        private readonly ISendingConfirmationRegistrationEmail _isendingEnd;
        private readonly ICheckWerification _icheck;


        public VerificationEmailController( IDataVerification _idata,ISendingVerifyingEmail isendingE, IUniqueException _ithrow, 
            ISendingConfirmationRegistrationEmail _isending, ICheckWerification _icheck)
        {
            this._idata = _idata;
            this._ithrow = _ithrow;
            this._isendingVer = isendingE;
            this._isendingEnd = _isending;
            this._icheck = _icheck;
        }

        [HttpGet]
        public ActionResult VerificationNumber()
        {

            kartoteka2 kartoteka = new kartoteka2();
            if (Session["kartoteka"] != null)
            {
                kartoteka = (kartoteka2)Session["kartoteka"];
            }
            else
            {
                return RedirectToAction("Register","Registration");
            }

            if (_idata.emailVerfication(kartoteka.kart_email))
            {
                Session["randomValue"] = _isendingVer.sendEmailToVerification(kartoteka.kart_email, kartoteka.kart_nazwisko, kartoteka.kart_email);
                ViewBag.imie = kartoteka.kart_imie;
                ViewBag.nazwisko = kartoteka.kart_nazwisko;
                ViewBag.email = kartoteka.kart_email;
                return View(kartoteka);
            }
            else
            {
                return RedirectToAction("info2", new { messageWindow = "Adres Email nie poprawny" });
            }
        }

        [HttpPost]
        public ActionResult VerificationNumber(int _number, kartoteka2 _kart)
        {
            try
            {

                int _randomNumber = Session["randomValue"] == null ? 0 : int.Parse(Session["randomValue"].ToString());
             
                if ((_randomNumber > 0) && (_number == _randomNumber))
                {
                    var werification = _icheck.getTimeWerification(_kart.kart_imie, _kart.kart_nazwisko, _kart.kart_email);
                    if (_icheck.verification(werification.dataRej, DateTime.Now))
                    {
                        ViewBag.visibleTrue = true;
                        using (var db = new EntitiesMaraton())
                        {
                            db.pKartotekaZawodnikaDodaj(_kart.kart_imie, _kart.kart_nazwisko, _kart.kart_email,
                                _kart.kart_dataUr, _kart.plec_id, _kart.kart_telefon, _kart.kart_uwagi,
                                _kart.dys_id, _kart.grup_id, true, true);
                            db.SaveChanges();

                            _isendingEnd.sendWelcomeEmail(_kart.kart_imie, _kart.kart_nazwisko, _kart.kart_email);

                            return RedirectToAction("RegistrationList","RegistrationListUser");
                        }
                    }
                    else
                    {
                        return RedirectToAction("info2", new { messageWindow = "Przekroczony limit czasu na wprowadzenie numeru." });
                    }
                }
                else
                {
                    ViewBag.visibleTrue = false;
                    return RedirectToAction("info2", new { messageWindow = "Błędny numer." });
                }
            }
            catch (Exception ex)
            {
                string er = ex.InnerException.Message;
               
                _ithrow.setUniqueEcxeption("Rejestracja", ex.InnerException.Message);
                return RedirectToAction("info2", new { messageWindow = _ithrow.getUniqueException(er) });
            }
        }

    }
}
