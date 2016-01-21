
app.service('Fzero_FraudAnalysis_MainService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'Fzero_FraudAnalysis_DefineFixedPrefixesAPIService',

    function (VRModalService, VRNotificationService, UtilsService, Fzero_FraudAnalysis_DefineFixedPrefixesAPIService) {

        return ({
            addNewFixedPrefix: addNewFixedPrefix,
            editFixedPrefix: editFixedPrefix,
            deleteFixedPrefix: deleteFixedPrefix

        });

        function addNewFixedPrefix(onFixedPrefixAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onFixedPrefixAdded = onFixedPrefixAdded;
            };
            var parameters = {
            };
            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/FixedPrefixes/DefineFixedPrefixesEditor.html', parameters, settings);
        }

        function editFixedPrefix(obj, onFixedPrefixUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onFixedPrefixUpdated = onFixedPrefixUpdated;
            };
            var parameters = {
                ID: obj.ID,
            };
            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/FixedPrefixes/DefineFixedPrefixesEditor.html', parameters, settings);
        }

        function deleteFixedPrefix($scope, obj, onFixedPrefixObjDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return Fzero_FraudAnalysis_DefineFixedPrefixesAPIService.DeleteFixedPrefix(obj.Entity.ID)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Fixed Prefix", deletionResponse);
                                onFixedPrefixObjDeleted(obj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }

    }]);
