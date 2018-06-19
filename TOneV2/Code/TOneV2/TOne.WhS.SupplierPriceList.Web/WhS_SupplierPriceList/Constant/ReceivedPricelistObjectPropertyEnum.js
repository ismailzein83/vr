app.constant('WhS_SupPL_ReceivedPricelistObjectPropertyEnum', {
	PricelistType: { value: 0, description: "Pricelist Type" },
	PricelistStatus: { value: 1, description: "Pricelist Status" },
	PricelistName: { value: 2, description: "Pricelist Name" },
	ReceivedDateTime: { value: 3, description: "Received DateTime" },
	StartProcessingDateTime: { value: 4, description: "Start Processing DateTime" },
	SupplierName: { value: 5, description: "Supplier Name" },
	SupplierEmail: { value: 6, description: "Supplier Email" },
	ErrorDetails: { value: 7, description: "Error Details" },
	SupplierId: { value: 8, description: "Supplier Id" },
	ProcessInstanceId: { value: 9, description: "Process Instance Id" },
	PricelistId: { value: 10, description: "Pricelist Id" },
	FileId: { value: 11, description: "File Id" },
	ReceivedPricelistId: { value: 12, description: "Received Pricelist Id" },
});

app.constant('WhS_SupPL_AutoImportEmailTypeEnum', {
	Received: { value: 0, description: "Received" },
	Succeeded: { value: 1, description: "Succeeded" },
	Failed: { value: 2, description: "Failed" },
	Internal: { value: 3, description: "Internal" },
});