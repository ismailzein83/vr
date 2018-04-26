app.constant('AuthenticateOperationResultEnum', {
    Succeeded: { value: 0, description: "Succeeded" },
    Failed: { value: 1, description: "Failed" },
    Inactive: { value: 2, description: "Inactive" },
    WrongCredentials: { value: 3, description: "WrongCredentials" },
    UserNotExists: { value: 4, description: "UserNotExists" },
    ActivationNeeded: { value: 5, description: "ActivationNeeded" },
    PasswordExpired: { value: 6, description: "PasswordExpired" }

});