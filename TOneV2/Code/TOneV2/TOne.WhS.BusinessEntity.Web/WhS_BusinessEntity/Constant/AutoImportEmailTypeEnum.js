app.constant('WhS_BE_InternalAutoImportEmailTypeEnum', {
	Received: { value: 0, description: "Received" ,isrequired:false},
	Succeeded: { value: 1, description: "Succeeded", isrequired: false },
	Failed: { value: 2, description: "Failed", isrequired: false },
	WaitingConfirmation: { value: 3, description: "Waiting Confirmation", isrequired: true }
});

app.constant('WhS_BE_SupplierAutoImportEmailTypeEnum', {
    Received: { value: 0, description: "Received", isrequired: true },
    Succeeded: { value: 1, description: "Succeeded", isrequired: true },
    Failed: { value: 2, description: "Failed", isrequired: true },
    Rejected: { value: 4, description: "Rejected", isrequired: true }
});