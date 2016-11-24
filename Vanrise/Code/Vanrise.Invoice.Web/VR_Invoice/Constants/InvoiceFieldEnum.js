app.constant('VR_Invoice_InvoiceFieldEnum', {
    CustomField: { value: 0, description: "Custom Field", fieldName: "Entity.Details", type: "Text" },
    InvoiceId: { value: 1, description: "ID", fieldName: "Entity.InvoiceId", type: "Text" },
    Partner: { value: 2, description: "Partner", fieldName: "Entity.PartnerName", type: "Text" },
    SerialNumber: { value: 3, description: "Serial Number", fieldName: "Entity.SerialNumber", type: "Text" },
    FromDate: { value: 4, description: "From Date", fieldName: "Entity.FromDate", type: "Datetime" },
    ToDate: { value: 5, description: "To Date", fieldName: "Entity.ToDate", type: "Datetime" },
    IssueDate: { value: 6, description: "Issue Date", fieldName: "Entity.IssueDate", type: "Datetime" },
    DueDate: { value: 7, description: "Due Date", fieldName: "Entity.DueDate", type: "Datetime" },
    Paid: { value: 8, description: "Paid Date", fieldName: "Entity.PaidDate", type: "Datetime" },
    UserId: { value: 9, description: "User Name", fieldName: "Entity.UserName", type: "Text" },
    CreatedTime: { value: 10, description: "Created Date", fieldName: "Entity.CreatedTime", type: "Datetime" },
    Lock: { value: 11, description: "Lock", fieldName: "Lock", type: "Datetime" }
});