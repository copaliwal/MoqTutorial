using System;

namespace CreditCardApplications
{
    public interface IServiceName
    {
        string ServiceName { get; }
    }

    public interface IServiceInformation
    {
        IServiceName Service { get; set; }
    }

    public enum ValidationMode
    {
        Quick,
        Detailed
    }

    public interface IFrequentFlyerNumberValidator
    {
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
        string LicenseKey { get; }

        public IServiceInformation ServiceInformation { get; }

        ValidationMode ValidationMode { get; set; }
    }
}