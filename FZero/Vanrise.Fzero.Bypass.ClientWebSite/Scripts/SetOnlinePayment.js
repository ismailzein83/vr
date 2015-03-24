

function CalculateCommission(
    amount, registration, fixedCommisssion, percentageCommission,
     commissionctrl, totalctrl, totalhdn, totalhdnInt) {
    //var registration = parseFloat('<%= RegistrationFees %>');
    //var fixedCommisssion = parseFloat('<%= FixedCommissionAmount %>');
    //var percentageCommission = parseFloat('<%= PercentageCommission %>');

    var paymentAmount = parseFloat(registration) + parseFloat(amount);
    var commission = ((100 * (parseFloat(fixedCommisssion) + paymentAmount)) / (100 - parseFloat(percentageCommission))) - paymentAmount;
    commission = parseFloat(commission).toFixed(2);
    var total = (parseFloat(commission) + parseFloat(paymentAmount)).toFixed(2);

    document.getElementById(commissionctrl).innerText = commission;
    document.getElementById(totalctrl).innerText = total;
    document.getElementById(totalhdn).value = total;
    document.getElementById(totalhdnInt).value = parseFloat(total) * 100;
}

function GetAmountInt(totalhdnInt) { return document.getElementById(totalhdnInt).value; }