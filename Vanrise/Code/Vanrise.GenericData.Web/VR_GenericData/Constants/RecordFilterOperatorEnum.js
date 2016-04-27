﻿app.constant('VR_GenericData_StringRecordFilterOperatorEnum', {
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
    Equals: { value: 0, description: '=' },
    NotEquals: { value: 1, description: '<>' },
    Greater: { value: 2, description: '>' },
    GreaterOrEquals: { value: 3, description: '>=' },
    Less: { value: 4, description: '<' },
    LessOrEquals: { value: 5, description: '<=' }
});

app.constant('VR_GenericData_DateTimeRecordFilterOperatorEnum', {
    Equals: { value: 0, description: '=' },
    NotEquals: { value: 1, description: '<>' },
    Greater: { value: 2, description: '>' },
    GreaterOrEquals: { value: 3, description: '>=' },
    Less: { value: 4, description: '<' },
    LessOrEquals: { value: 5, description: '<=' }
});

app.constant('VR_GenericData_ListRecordFilterOperatorEnum', {
    In: { value: 0, description: 'In' },
    NotIn: { value: 1, description: 'Not In' }
});

app.constant('VR_GenericData_RecordQueryLogicalOperatorEnum', {
    And: { value: 0, description: 'And' },
    Or: { value: 1, description: 'Or' }
});

app.constant('VR_GenericData_ConditionEnum', {
    Value: { description: 'Value', showDirective: true, editor: '', type: '' },
    Empty: { description: 'Empty', showDirective: false, editor: 'vr-genericdata-datarecordtypefield-emptyfilter', type: 'Vanrise.GenericData.Entities.EmptyRecordFilter, Vanrise.GenericData.Entities' },
    NonEmpty: { description: 'Non Empty', showDirective: false, editor: 'vr-genericdata-datarecordtypefield-nonemptyfilter', type: 'Vanrise.GenericData.Entities.NonEmptyRecordFilter, Vanrise.GenericData.Entities' },
});