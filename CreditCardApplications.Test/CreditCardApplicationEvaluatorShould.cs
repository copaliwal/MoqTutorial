using Xunit;
using Moq;

namespace CreditCardApplications.Test
{
    public class CreditCardApplicationEvaluatorShould
    {

        [Fact]
        public void AcceptHighIncomeApplication()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication() { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Service.ServiceName).Returns("ABC");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            // Return True, only when the parameter value exactly match
            //mockValidator.Setup(x => x.IsValid("xyz")).Returns(true);

            // Return True, only when any string value parameter passes
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            // Return True, only when the parameter value start with 'x'
            //mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith('x')))).Returns(true);

            // Return True, only when the parameter value is "x" or "y" or "z"
            //mockValidator.Setup(x => x.IsValid(It.IsIn("x", "y", "z"))).Returns(true);

            // Return True, only when the parameter value between 'b' and 'z'
            //mockValidator.Setup(x => x.IsValid(It.IsInRange("b", "z", Range.Inclusive))).Returns(true);

            // Return True, only when the parameter value match the Regex expression
            //mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]",System.Text.RegularExpressions.RegexOptions.None))).Returns(true);

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.Service.ServiceName).Returns("ABC");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "xyz"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);

            mockValidator.Setup(x => x.ServiceInformation.Service.ServiceName).Returns("ABC");
            mockValidator.Setup(x => x.LicenseKey).Returns("NEW");
            mockValidator.SetupProperty(x => x.ValidationMode);

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);
            

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            var isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));
            mockValidator.Setup(x => x.LicenseKey).Returns("EXPIRED");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            
            var application = new CreditCardApplication() { Age=42 };

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void ReferWhenServiceNameAir()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            var isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            //var mockServeName = new Mock<IServiceName>();
            //mockServeName.Setup(x => x.ServiceName).Returns("AIR");

            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.Service).Returns(mockServeName.Object);

            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

            mockValidator.Setup(x => x.ServiceInformation.Service.ServiceName).Returns("AIR");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication() { Age = 42 };

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            // Enable change tracking for all properties
            //mockValidator.SetupAllProperties(); 

            var isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));
            mockValidator.Setup(x => x.ServiceInformation.Service.ServiceName).Returns("BUS");

            // Enable change tracking for ValidationMode
            mockValidator.SetupProperty(x => x.ValidationMode);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }

        [Fact]
        public void ShouldValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Service.ServiceName).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { FrequentFlyerNumber = "q" };

            sut.Evaluate(application);

            //Verifies the method has been has been called
            //mockValidator.Verify(x => x.IsValid(It.IsAny<string>()));

            //Verifies the method never called/executed
            //mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);

            //Verifies the method called/executed onces
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once);

            //Verifies the method called/executed number of times
            //mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void CheckLicenseKeyForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.Service.ServiceName).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

            sut.Evaluate(application);

            mockValidator.VerifyGet(x => x.ServiceInformation.Service.ServiceName, Times.Once);
        }
    }
}
