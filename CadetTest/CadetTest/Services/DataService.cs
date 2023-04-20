using CadetTest.Entities;
using CadetTest.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadetTest.Services
{
    public interface IDataService
    {
        string GetRandomString(int stringLength);
        void InitConsents();
        List<Consent> GetRangeById(int id, int adet);

        Consent AddConsents(NewConsent newConsent);
        bool DeleteConsent(Consent consentToDelete);

        bool UpdateConsent(Consent updateConsent);
    }
    public class DataService : IDataService
    {
        private DataContext _context;
        private readonly AppSettings _appSettings;
        private readonly ILogger<UserService> _logger;
        private Random _random;

        public DataService(DataContext context, IOptions<AppSettings> appSettings, ILogger<UserService> logger)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _logger = logger;

            _random = new Random();
        }

        #region Public Methods

        public List<Consent> GetRangeById(int id, int adet)
        {
            var cevap = _context.Consents.Where(k => k.Id >= id).Take(adet).ToList();
            return cevap;
        }
        #endregion


        public void InitConsents()
        {
            if (_context.Consents.Any()) return;

            for (int i = 1; i < _appSettings.ConsentCount; i++)
            {
                var consent = new Consent
                {
                    Recipient = $"{GetRandomString(10)}_{i}@ornek.com",
                    RecipientType = "EPOSTA",
                    Status = "ONAY",
                    Type = "EPOSTA"
                };

                _context.Consents.Add(consent);

            }
        }

        public Consent AddConsents(NewConsent newConsent)
        {
            var consent = new Consent
            {
                //Id = _context.Consents.Count() + 1,
                Recipient = newConsent.Recipient,
                RecipientType = newConsent.RecipientType,
                Status = newConsent.Status,
                Type = newConsent.Type
            };

            _context.Consents.Add(consent);
            _context.SaveChanges(); //_context in değişikliği commit etmesi için saveChanges yapmam gerekti

            var cevap = GetRangeById(_context.Consents.Count(), 1).FirstOrDefault();
            return cevap;
        }

        public bool DeleteConsent(Consent consentToDelete)
        {
            try
            {
                _context.Consents.Remove(consentToDelete);
                _context.SaveChanges(); //_context in değişikliği commit etmesi için saveChanges yapmam gerekti

                return true;
            }
            catch
            {
                return false;
            }
           
        }

        public bool UpdateConsent(Consent updateConsent)
        {
            try
            {        
                 _context.Consents.Update(updateConsent);
                _context.SaveChanges(); //_context in değişikliği commit etmesi için saveChanges yapmam gerekti

                return true;
            }
            catch
            {
                return false;
            }

        }

        #region Private Methods

        public string GetRandomString(int stringLength)
        {
            var sb = new StringBuilder();
            int numGuidsToConcat = (((stringLength - 1) / 32) + 1);
            for (int i = 1; i <= numGuidsToConcat; i++)
            {
                sb.Append(Guid.NewGuid().ToString("N"));
            }

            return sb.ToString(0, stringLength);
        }
        #endregion
    }
}
