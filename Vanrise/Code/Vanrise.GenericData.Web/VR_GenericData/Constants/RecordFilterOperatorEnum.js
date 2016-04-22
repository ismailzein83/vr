app.constant('VR_GenericData_StringRecordFilterOperatorEnum', {
    Equals: { value: 0, description: 'Equals' },
    NotEquals: { value: 1, description: 'Not Equals' },
    StartsWith: { value: 2, description: 'Starts With' },
    NotStartsWith: { value: 3, description: 'Not Starts With' },
    EndsWith: { value: 4, description: 'Ends With' },
    NotEndsWith: { value: 5, description: 'Not Ends With' },
    Contains: { value: 6, description: 'Contains' },
    NotContains: { value: 7, description: 'Not Contains' }
});

app.constant('VR_GenericData_NumberRecordFilterOperatorEnum', {
    Equals: { value: 0, description: 'Equals' },
    NotEquals: { value: 1, description: 'Not Equals' },
    Greater: { value: 2, description: 'Greater' },
    GreaterOrEquals: { value: 3, description: 'Greater Or Equals' },
    Less: { value: 4, description: 'Less' },
    LessOrEquals: { value: 5, description: 'Less Or Equals' }
});

app.constant('VR_GenericData_DateTimeRecordFilterOperatorEnum', {
    Equals: { value: 0, description: 'Equals' },
    NotEquals: { value: 1, description: 'Not Equals' },
    Greater: { value: 2, description: 'Greater' },
    GreaterOrEquals: { value: 3, description: 'Greater Or Equals' },
    Less: { value: 4, description: 'Less' },
    LessOrEquals: { value: 5, description: 'Less Or Equals' }
});

app.constant('VR_GenericData_ListRecordFilterOperatorEnum', {
    In: { value: 0, description: 'In' },
    NotIn: { value: 1, description: 'Not In' }
});