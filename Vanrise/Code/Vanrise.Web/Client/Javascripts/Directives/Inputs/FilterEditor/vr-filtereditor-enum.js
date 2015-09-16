(function (app) {

    "use strict";

    var inputEnum = {
        Text: { value: 0, description: "text" },
        Switch: { value: 1, description: "switch" },
        Select: { value: 2, description: "select" },
        Datetime: { value: 3, description: "datetime" },
        Number: { value: 4, description: "number" }
    };

    var typeEnum = {
        String: { value: 0, description: "string", input: inputEnum.Text },
        //Integer: { value: 1, description: "integer", input: inputEnum.Text },
        Double: { value: 2, description: "double", input: inputEnum.Number },
        //Date: { value: 3, description: "date", input: inputEnum.Datetime },
        //Time: { value: 4, description: "time", input: inputEnum.Datetime },
        Datetime: { value: 5, description: "datetime", input: inputEnum.Datetime },
        Boolean: { value: 6, description: "boolean", input: inputEnum.Switch },
        Array: { value: 7, description: "boolean", input: inputEnum.Select }
    };

    var inputTypeEnum = {
        MultipleInput: { value: 0, description: "MultipleInput" },
        NoInput: { value: 1, description: "NoInput" }
    };
    //
    app.constant('FilterEditorOperatorEnum', {
        Equal: { value: 0, description: "equal", types: [typeEnum.Boolean, typeEnum.Double, typeEnum.Datetime, typeEnum.String] },
        NotEqual: { value: 1, description: "not equal", types: [typeEnum.Double, typeEnum.Datetime, typeEnum.String] },
        In: { value: 2, description: "in", types: [typeEnum.Array] },
        NotIn: { value: 3, description: "not in", types: [typeEnum.Array] },
        IsNull: { value: 4, description: "is null", types: [typeEnum.Array, typeEnum.Double, typeEnum.Datetime, typeEnum.String], InputType: inputTypeEnum.NoInput },
        IsNotNull: { value: 5, description: "is not null", types: [typeEnum.Array, typeEnum.Double, typeEnum.Datetime, typeEnum.String], InputType: inputTypeEnum.NoInput },
        Less: { value: 6, description: "less", types: [typeEnum.Double, typeEnum.Datetime] },
        LessOrEqual: { value: 7, description: "less or equal", types: [typeEnum.Double, typeEnum.Datetime] },
        Greater: { value: 8, description: "greater", types: [typeEnum.Double, typeEnum.Datetime] },
        GreaterOrEqual: { value: 9, description: "greater or equal", types: [typeEnum.Double, typeEnum.Datetime] },
        Between: { value: 10, description: "between", types: [typeEnum.Double, typeEnum.Datetime], InputType: inputTypeEnum.MultipleInput },
        NotBetween: { value: 11, description: "not between", types: [typeEnum.Double, typeEnum.Datetime], InputType: inputTypeEnum.MultipleInput }

    });

    app.constant('FilterEditorInputTypeEnum', inputTypeEnum);
    app.constant('FilterEditorFieldTypeEnum', typeEnum);
    app.constant('FilterEditorInputEnum', inputEnum);

})(app);