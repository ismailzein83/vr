//(function (appControllers) {

//    "use strict";

//    CustomCodePricingLogicEditor.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

//    function CustomCodePricingLogicEditor($scope, UtilsService, VRNotificationService, VRNavigationService) {

//        var textAreaAPI;
//        var expression;
//        var isEditMode;

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            $scope.scopeModel = {};
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined && parameters != null && parameters.params != undefined) {
//                var params = parameters.params;
//                expression = params.expression;

//                $scope.scopeModel.targetRecordTypeFields = params.targetRecordTypeFields;
//                if ($scope.scopeModel.targetRecordTypeFields != undefined && $scope.scopeModel.targetRecordTypeFields.length > 0) {
//                    $scope.scopeModel.targetRecordTypeFields.sort(sortByName);
//                }

//                $scope.scopeModel.chargeSettingsRecordTypeFields = params.chargeSettingsRecordTypeFields;
//                if ($scope.scopeModel.chargeSettingsRecordTypeFields != undefined && $scope.scopeModel.chargeSettingsRecordTypeFields.length > 0) {
//                    $scope.scopeModel.chargeSettingsRecordTypeFields.sort(sortByName);
//                }
//            }

//            isEditMode = expression != undefined && expression.CodeExpression != undefined;
//        }

//        function defineScope() {

//            $scope.scopeModel.onTextAreaReady = function (api) {
//                textAreaAPI = api;
//            };

//            $scope.scopeModel.saveValue = function () {
//                saveValue();
//                $scope.modalContext.closeModal();
//            };

//            $scope.scopeModel.insertText = function (item) {
//                if (textAreaAPI != undefined) {
//                    textAreaAPI.appendAtCursorPosition(item.Name);
//                }
//            };

//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };

//            $scope.scopeModel.filterValueChanged = function (value) {

//                if (value == undefined || value.length == 0) {
//                    setHideItemsFalse($scope.scopeModel.chargeSettingsRecordTypeFields);
//                    setHideItemsFalse($scope.scopeModel.targetRecordTypeFields);
//                    return;
//                }

//                var filter = value.toLowerCase();

//                for (var i = 0; i < $scope.scopeModel.chargeSettingsRecordTypeFields.length; i++) {
//                    var chargeSettingsRecordField = $scope.scopeModel.chargeSettingsRecordTypeFields[i].Name.toLowerCase();
//                    if (chargeSettingsRecordField.indexOf(filter) == -1)
//                        $scope.scopeModel.chargeSettingsRecordTypeFields[i].hideItem = true;
//                    else
//                        $scope.scopeModel.chargeSettingsRecordTypeFields[i].hideItem = false;
//                }

//                for (var j = 0; j < $scope.scopeModel.targetRecordTypeFields.length; j++) {
//                    var targetRecordField = $scope.scopeModel.targetRecordTypeFields[j].Name.toLowerCase();
//                    if (targetRecordField.indexOf(filter) == -1)
//                        $scope.scopeModel.targetRecordTypeFields[j].hideItem = true;
//                    else
//                        $scope.scopeModel.targetRecordTypeFields[j].hideItem = false;
//                }
//            };
//        }

//        function load() {

//            $scope.scopeModel.isLoading = true;

//            if (isEditMode) {
//                loadAllControls().finally(function () {
//                });
//            }
//            else
//                loadAllControls();
//        }

//        function loadAllControls() {

//            $scope.title = UtilsService.buildTitleForUpdateEditor('Pricing Logic Builder');
//            $scope.scopeModel.expressionValue = expression != undefined ? expression.CodeExpression : undefined;
//            var promises = [];

//            return UtilsService.waitPromiseNode({ promises: promises }).catch(function (error) {
//                VRNotificationService.notifyExceptionWithClose(error, $scope);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });
//        }

//        function saveValue() {
//            if ($scope.onSetValue != undefined)
//                return;

//            if ($scope.scopeModel.expressionValue != undefined && $scope.scopeModel.expressionValue != '') {
//                $scope.onSetValue({
//                    CodeExpression: $scope.scopeModel.expressionValue
//                });
//                return;
//            }
//            else {
//                $scope.onSetValue();
//            }
//        }

//        function setHideItemsFalse(objectsList) {

//            if (objectsList == undefined || objectsList.length == 0)
//                return;

//            for (var i = 0; i < objectsList.length; i++)
//                objectsList[i].hideItem = false;
//        }

//        function sortByName(a, b) {
//            var nameA = a.Name.toLowerCase();
//            var nameB = b.Name.toLowerCase();
//            if (nameA < nameB) {
//                return -1;
//            }
//            if (nameA > nameB) {
//                return 1;
//            }
//            return 0;
//        }
//    }

//    appControllers.controller('RetailBilling_ChargeType_CustomCodePricingLogicEditor', CustomCodePricingLogicEditor);
//})(appControllers);