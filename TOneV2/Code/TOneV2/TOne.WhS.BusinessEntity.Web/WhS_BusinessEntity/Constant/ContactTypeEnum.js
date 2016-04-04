app.constant('WhS_BE_ContactTypeEnum', {
    BillingContactPerson: { value: 1, label: "Billing Person" , type:"text"},
    BillingEmail: { value: 2, label: "Billing Email", type: "email" },

    PricingContactPerson: { value: 4, label: "Pricing Person", type: "text" },
    PricingEmail: { value: 5, label: "Pricing Email", type: "email" },
    
    AccountManagerContact: { value: 6, label: "Account Manager Person", type: "text" },
    AccountManagerEmail: { value: 7, label: "Account Manager Email", type: "email" },

    SupportContactPerson: { value: 8, label: "Support Person", type: "text" },
    SupportEmail: { value: 9, label: "Support Email ", type: "email" },

    TechnicalContactPerson: { value: 10, label:"Technical Person" , type:"text"},
    TechnicalEmail: { value: 11, label: "Technical Email", type: "email" },

    CommercialContactPerson: { value: 12, label: "Commercial Person", type: "text" },
    CommercialEmail: { value: 13, label: "Commercial Email", type: "email" },

    AlertingSMSPhoneNumbers: { value: 14, label: "Alerting SMS Phone Numbers", type: "number" },

    DisputeEmail: { value: 3, label: "Dispute Email", type: "email" }
});