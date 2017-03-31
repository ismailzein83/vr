(function (appControllers) {
    "use strict";

    AgentRequestNumberEditorController.$inject = ['$scope', 'Retail_Ringo_RingoAgentNumberRequestService', 'VRNavigationService', 'VR_GenericData_GenericRule', 'Retail_Ringo_AgentNumberStatusEnum', 'Retail_Ringo_AgentNumberRequestAPIService', 'Retail_Ringo_AgentNumberRequestStatusEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function AgentRequestNumberEditorController($scope, Retail_Ringo_RingoAgentNumberRequestService, VRNavigationService, VR_GenericData_GenericRule, Retail_Ringo_AgentNumberStatusEnum, Retail_Ringo_AgentNumberRequestAPIService, Retail_Ringo_AgentNumberRequestStatusEnum, UtilsService, vrUIUtilsService, vrNotificationService) {

        var numberRequest;
        var numberRequestId;

        loadParameters();
        defineScope();
        load();
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.numbers = [];
            $scope.scopeModel.enabled = true;

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.rejectRule = function () {
                var onNumberRequestRejected = function () {
                    if ($scope.onAgentNumberRequestProcessed != undefined)
                        $scope.onAgentNumberRequestProcessed(numberRequest);
                    $scope.modalContext.closeModal();
                };
                numberRequest.Status = Retail_Ringo_AgentNumberRequestStatusEnum.Rejected.value;
                numberRequest.StatusDescription = Retail_Ringo_AgentNumberRequestStatusEnum.Rejected.description;
                for (var i = 0; i < numberRequest.Settings.AgentNumbers.length; i++) {
                    numberRequest.Settings.AgentNumbers[i].Status = Retail_Ringo_AgentNumberStatusEnum.Rejected.value;
                }
                Retail_Ringo_RingoAgentNumberRequestService.rejectAgentNumberRequest($scope, numberRequest, onNumberRequestRejected);

            }
            $scope.scopeModel.addRule = function () {

                var preDefinedData = {
                    settings: {
                        Value: numberRequest.AgentId
                    },
                    criteriaFieldsValues: {
                        Number: {
                            Values: $scope.scopeModel.numbers
                        }
                    }
                };

                var onNumberRequestAdded = function (addedRule) {
                    for (var i = 0; i < numberRequest.Settings.AgentNumbers.length; i++) {
                        var agentNumber = numberRequest.Settings.AgentNumbers[i];
                        if (UtilsService.contains(addedRule.Entity.Criteria.FieldsValues.Number.Values, agentNumber.Number)) {
                            agentNumber.Status = Retail_Ringo_AgentNumberStatusEnum.Accepted.value;
                        }
                        else
                            agentNumber.Status = Retail_Ringo_AgentNumberStatusEnum.Rejected.value;
                    }

                    numberRequest.Status = Retail_Ringo_AgentNumberRequestStatusEnum.Accepted.value;
                    numberRequest.StatusDescription = Retail_Ringo_AgentNumberRequestStatusEnum.Accepted.description;
                    Retail_Ringo_AgentNumberRequestAPIService.UpdateAgentNumberRequest(numberRequest).then(function () {
                        if ($scope.onAgentNumberRequestProcessed != undefined)
                            $scope.onAgentNumberRequestProcessed(numberRequest);
                        $scope.modalContext.closeModal();
                    });
                };

                VR_GenericData_GenericRule.addGenericRule('432d290b-374a-4d50-860f-c8810af9a66d', onNumberRequestAdded, preDefinedData);

            };
        }
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                numberRequestId = parameters.agentNumberRequestId;
                //numberRequest = {
                //    Entity: parameters.agentNumberRequest.Entity,
                //    StatusDescription: parameters.agentNumberRequest.StatusDescription
                //};

                //
            }

        }
        function load() {
            $scope.scopeModel.isloading = true;
            GetAgentNumberRequest().then(function () {
                loadAllControls()
            }).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isloading = false;
            }).finally(function () {
                $scope.scopeModel.isloading = false;
            });
        }
        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function setTitle() {
            $scope.title = 'Agent Numbers';
        }
        function loadStaticData() {
            if (numberRequest == undefined)
                return;

            for (var i = 0; i < numberRequest.Settings.AgentNumbers.length; i++) {
                var agentNumber = numberRequest.Settings.AgentNumbers[i];
                $scope.scopeModel.numbers.push(agentNumber.Number);
            }
            $scope.scopeModel.enabled = numberRequest.Status == Retail_Ringo_AgentNumberRequestStatusEnum.Pending.value ? true : false;
        }
        function GetAgentNumberRequest() {
            return Retail_Ringo_AgentNumberRequestAPIService.GetAgentNumberRequest(numberRequestId).then(function (response) {
                numberRequest = response;
            });
        }
    }

    appControllers.controller('Retail_Ringo_AgentRequestNumberEditorController', AgentRequestNumberEditorController);
})(appControllers);