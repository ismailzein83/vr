(function (app) {

    "use strict";

    var inputEnum = {
        Text: { value: 0, description: "text" },
        Switch: { value: 2, description: "switch" },
        Select: { value: 4, description: "select" },
        Datetime: { value: 4, description: "datetime" }
    };

    var typeEnum = {
        String: { value: 0, description: "string", input: inputEnum.Text },
        Integer: { value: 1, description: "integer", input: inputEnum.Text },
        Double: { value: 2, description: "double", input: inputEnum.Text },
        Date: { value: 3, description: "date", input: inputEnum.Datetime },
        Time: { value: 4, description: "time", input: inputEnum.Datetime },
        Datetime: { value: 5, description: "datetime", input: inputEnum.Datetime },
        Boolean: { value: 6, description: "boolean", input: inputEnum.Switch },
        Array: { value: 6, description: "boolean", input: inputEnum.Select }
    };

    var inputTypeEnum = {
        MultipleInput: { value: 0, description: "MultipleInput" },
        NoInput: { value: 1, description: "NoInput" }
    };

    app.constant('FilterEditorOperatorEnum', {
        Equal: { value: 0, description: "equal", types: [typeEnum.Boolean, typeEnum.Double] },
        NotEqual: { value: 1, description: "not equal", types: [typeEnum.Double] },
        In: { value: 2, description: "in", types: [typeEnum.Array, typeEnum.Double] },
        NotIn: { value: 3, description: "not in", types: [typeEnum.Array, typeEnum.Double] },
        IsNull: { value: 4, description: "is null", types: [typeEnum.Array, typeEnum.Double], InputType: inputTypeEnum.NoInput },
        IsNotNull: { value: 5, description: "is not null", types: [typeEnum.Array, typeEnum.Double], InputType: inputTypeEnum.NoInput },
        Less: { value: 6, description: "less", types: [typeEnum.Double] },
        LessOrEqual: { value: 7, description: "less or equal", types: [typeEnum.Double] },
        Greater: { value: 8, description: "greater", types: [typeEnum.Double] },
        GreaterOrEqual: { value: 9, description: "greater or equal", types: [typeEnum.Double] },
        Between: { value: 10, description: "between", types: [typeEnum.Double], InputType: inputTypeEnum.MultipleInput },
        NotBetween: { value: 11, description: "not between", types: [typeEnum.Double], InputType: inputTypeEnum.MultipleInput }

    });

    app.constant('FilterEditorInputTypeEnum', inputTypeEnum);
    app.constant('FilterEditorFieldTypeEnum', typeEnum);
    app.constant('FilterEditorInputEnum', inputEnum);

})(app);