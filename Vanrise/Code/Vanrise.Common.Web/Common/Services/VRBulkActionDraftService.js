(function (appControllers) {

    "use strict";

    VRBulkActionDraftService.$inject = ['VRModalService', 'UtilsService','VRCommon_VRBulkActionDraftAPIService'];

    function VRBulkActionDraftService(VRModalService, UtilsService, VRCommon_VRBulkActionDraftAPIService) {

        function createBulkActionDraft(context) {
            return new BulkActionDraft(context.triggerRetrieveData, context.setSelectAllEnablity, context.setDeselectAllEnablity, context.setActionsEnablity, context.hasItems, UtilsService, VRCommon_VRBulkActionDraftAPIService);
        }

        return {
            createBulkActionDraft: createBulkActionDraft,
        };
    }

    function BulkActionDraft(triggerRetrieveData, setSelectAllEnablity, setDeselectAllEnablity, setActionsEnablity, hasItems, UtilsService, VRCommon_VRBulkActionDraftAPIService) {
        var isAllSelected;
        var targetItems = [];
        var bulkActionDraftIdentifier;
        var reflectedToDB = false;
        bulkActionDraftIdentifier = UtilsService.guid();
        reEvaluateButtonsStatus();

        function onSelectItem(item, isSelected) {
            if (isSelected) {
                if (isAllSelected) {
                    var index = UtilsService.getItemIndexByVal(targetItems, item.ItemId, "ItemId");
                    if (index > -1)
                        targetItems.splice(index, 1);
                }
                else {
                    if (UtilsService.getItemByVal(targetItems, item.ItemId, "ItemId") == undefined)
                        targetItems.push(item);
                }

            } else {
                if (isAllSelected) {
                    if (UtilsService.getItemByVal(targetItems, item.ItemId, "ItemId") == undefined)
                        targetItems.push(item);

                }
                else {
                    var index = UtilsService.getItemIndexByVal(targetItems, item.ItemId, "ItemId");
                    if (index > -1)
                        targetItems.splice(index, 1);
                }
            }
            reEvaluateButtonsStatus();
        }

        function setItemSelected(item) {
            if (isAllSelected) {
                if (UtilsService.getItemByVal(targetItems, item.ItemId, "ItemId") == undefined)
                    item.isSelected = true;

            } else {
                if (UtilsService.getItemByVal(targetItems, item.ItemId, "ItemId") != undefined)
                    item.isSelected = true;
            }
        }

        function selectAllItems() {
            reflectedToDB = true;
            isAllSelected = true;
            targetItems.length = 0;
            reEvaluateButtonsStatus();
            return triggerRetrieveData();
        }

        function deselectAllItems() {
            isAllSelected = false;
            targetItems.length = 0;
            reEvaluateButtonsStatus();
            return triggerRetrieveData();
        }

        function isItemSelected(itemId) {
            if (isAllSelected) {
                if (UtilsService.getItemByVal(targetItems, itemId, "ItemId") == undefined)
                    return true;
                return false;
            }

            if (UtilsService.getItemByVal(targetItems, itemId, "ItemId") != undefined)
                return true;
            return false;
        }

        function getBulkActionIdentifier() {
            return bulkActionDraftIdentifier;
        }

        function getBulkActionState() {
            return {
                IsAllSelected: isAllSelected,
                BulkActionDraftIdentifier: bulkActionDraftIdentifier,
                ReflectedToDB: reflectedToDB
            };
        }

        function finalizeBulkActionDraft() {
            var promise = UtilsService.createPromiseDeferred();
            promise.resolve({
                IsAllSelected: isAllSelected,
                TargetItems: targetItems,
                BulkActionDraftIdentifier: bulkActionDraftIdentifier
            });
            return promise.promise;
        }
        function anyItemSelected() {
            return isAllSelected || targetItems.length > 0;
        }

        function allItemsSelected() {
            return isAllSelected && targetItems.length == 0;
        }
        function hasSelectedItems() {
            if (isAllSelected || targetItems.length > 0)
                return true;
            return false;
        }
        function reEvaluateButtonsStatus() {
            var hasInvoices = hasItems();
            if (hasInvoices) {
                var isAnyItemSelected = anyItemSelected();
                var allSelected = allItemsSelected();

                if (allSelected) {
                    setSelectAllEnablity(false);
                    setDeselectAllEnablity(true);
                    setActionsEnablity(true);
                }
                else {
                    if (isAnyItemSelected) {
                        setDeselectAllEnablity(true);
                        setActionsEnablity(true);
                        setSelectAllEnablity(true);
                    }
                    else {
                        setDeselectAllEnablity(false);
                        setActionsEnablity(false);
                        setSelectAllEnablity(true);
                    }

                }
            } else {
                setDeselectAllEnablity(false);
                setActionsEnablity(false);
                setSelectAllEnablity(false);
            }

        }
        function GetVRBulkActionDrafts() {
            var bulkActionsDraftPromise = UtilsService.createPromiseDeferred();
            var bulkActionDrafts;
            finalizeBulkActionDraft().then(function (response) {
                var finaleStateObject = response;
                VRCommon_VRBulkActionDraftAPIService.GetVRBulkActionDrafts(finaleStateObject).then(function (response) {
                    console.log(response);
                    bulkActionDrafts = response;
                    bulkActionsDraftPromise.resolve({
                        bulkActionDrafts: bulkActionDrafts
                    });
                });
            });
            return bulkActionsDraftPromise.promise;
        }
    


        return {
            onSelectItem: onSelectItem,
            setItemSelected: setItemSelected,
            selectAllItems: selectAllItems,
            deselectAllItems: deselectAllItems,
            isItemSelected: isItemSelected,
            getBulkActionState: getBulkActionState,
            finalizeBulkActionDraft: finalizeBulkActionDraft,
            getBulkActionIdentifier: getBulkActionIdentifier,
            reEvaluateButtonsStatus: reEvaluateButtonsStatus,
            hasSelectedItems: hasSelectedItems,
            GetVRBulkActionDrafts: GetVRBulkActionDrafts
        };


    }

    appControllers.service('VRCommon_VRBulkActionDraftService', VRBulkActionDraftService);

})(appControllers);


