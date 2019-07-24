app.constant("WhS_Sales_ImportedRowStatus", {
    valid: { value: 0, description: "Valid" },
    Invalid: { value: 1, description: "Invalid" },
    OnlyNormalRateValid: { value: 2, description: "Only Normal Rate Valid" },
});

app.constant("WhS_Sales_CustomerTargetMatchImportedRowStatus", {
    valid: { value: 0, description: "Valid" },
    Invalid: { value: 1, description: "Invalid" },
    InvalidDueExpectedRateViolation: { value: 2, description: "Invalid Due Expected Rate Violation" }
});
