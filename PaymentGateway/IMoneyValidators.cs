using System;
using System.Collections.Generic;

namespace PaymentGateway
{
    /// <summary>
    /// Interface that defines money checks
    /// </summary>
    public interface IMoneyValidators
    {
        bool ValidateMoneyToCapture(Money moneyToBeCaptured, out List<String> errors);
        bool ValidateMoneyToRefund(Money moneyToBeRefunded, out List<String> errors);
    }
}
