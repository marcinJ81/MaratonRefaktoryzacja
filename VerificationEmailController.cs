using LibDatabase.DbContext;
using LibDatabase.Interaces;
using maratonMszana_v4.VerificationData;
using SendMail;
using SendMail.DescriptionEndMail;
using SendMail.DescriptionVerificationNumber;
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
        private readonly INewRecord _inew;
        private readonly IDataVerification _idata;
        private readonly IRandomNumber _irandom;
        private readonly ISMTP_Configuration _ismtp;
        private readonly IDescriptionVerificationNumber _idesc;
        private readonly ICheckWerification _icheck;
        private readonly IUniqueException _ithrow;
        private readonly IDataForDescription _idataDescription;
        private readonly IDescriptionEndCreate _iend;
        private readonly IExceptionBase _iexception;

        public VerificationEmailController(INewRecord _inew, IDataVerification _idata, ISMTP_Configuration _ismtp, IDescriptionVerificationNumber _desc
            , ICheckWerification _icheck, IRandomNumber _irandom, IExceptionBase _iexception
            , IDataForDescription _idataDescription, IDescriptionEndCreate _iend, IExceptionBase iexception, IUniqueException _ithrow)
        {
            this._inew = _inew;
            this._idata = _idata;
            this._ismtp = _ismtp;
            this._idesc = _desc;
            this._icheck = _icheck;
            this._ithrow = _ithrow;
            this._iexception = _iexception;
            this._irandom = _irandom;
            this._idataDescription = _idataDescription;
            this._iend = _iend;
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
                int _random = _irandom.generateNumber();
                Session["randomValue"] = _random;

                ViewBag.imie = kartoteka.kart_imie;
                ViewBag.nazwisko = kartoteka.kart_nazwisko;
                ViewBag.email = kartoteka.kart_email;

                var _mail = _idesc.create_description(kartoteka.kart_imie, kartoteka.kart_nazwisko, kartoteka.kart_email, _random);
                _ismtp.sendEmail(_mail);
                _icheck.saveTimeWerification(new kartoteka_TMP
                {
                    imie = kartoteka.kart_imie,
                    nazwisko = kartoteka.kart_nazwisko,
                    email = kartoteka.kart_email,
                    dataRej = DateTime.Now
                });
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
                            string mail_description = _iend.createDescription(_idataDescription.get_DataFoDescritpion(_kart.kart_imie, _kart.kart_nazwisko,
                                         _kart.kart_email));
                            var info = _ismtp.createResponse(new EmailDescription()
                            {
                                name = _kart.kart_imie,
                                surname = _kart.kart_nazwisko,
                                email_adress = _kart.kart_email,
                                sender = "mszana2017@maraton24.hw7.pl",
                                randomNumber = 0,
                                recipient = _kart.kart_email,
                                subject = "Potwierdzenie Rejestracji X Maraton Rowerowy Mszana 2019"
                            }, mail_description);
                            _ismtp.sendEmail(info);

                            _icheck.saveTimeWerification(new kartoteka_TMP
                            {
                                imie = _kart.kart_imie,
                                nazwisko = _kart.kart_nazwisko,
                                email = _kart.kart_email,
                                dataKoncowa = DateTime.Now,
                                rejestracja = 1
                            });
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
