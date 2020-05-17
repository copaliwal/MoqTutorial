﻿using Xunit;

namespace CreditCardApplications.Test
{
    public class CreditCardApplicationEvaluatorShould
    {

        [Fact]
        public void AcceptHighIncomeApplication()
        {
            var sut = new CreditCardApplicationEvaluator();
            var application = new CreditCardApplication() { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            var sut = new CreditCardApplicationEvaluator();

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
    }
}
