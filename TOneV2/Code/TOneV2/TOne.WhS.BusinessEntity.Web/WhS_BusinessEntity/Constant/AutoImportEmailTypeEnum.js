app.constant('WhS_BE_InternalAutoImportEmailTypeEnum', {
	Received: { value: 0, description: "Received" },
	Succeeded: { value: 1, description: "Succeeded" },
	Failed: { value: 2, description: "Failed" },
    WaitingConfirmation: { value: 3, description: "Waiting Confirmation" }
});

app.constant('WhS_BE_SupplierAutoImportEmailTypeEnum', {
    Received: { value: 0, description: "Received" },
    Succeeded: { value: 1, description: "Succeeded" },
    Failed: { value: 2, description: "Failed" },
    Rejected: {value: 4, description: "Rejected" }
});