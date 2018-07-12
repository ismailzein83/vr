'use strict';

app.directive('vrWhsBeAutoimporttemplatesettingsEditor', ['WhS_BE_SupplierAutoImportEmailTypeEnum', 'VRUIUtilsService', 'UtilsService',
	function (WhS_BE_SupplierAutoImportEmailTypeEnum, VRUIUtilsService, UtilsService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
				isrequired: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var autoImportTemplateSettingsEditor = new AutoImportTemplateSettingsEditor($scope, ctrl, $attrs);
				autoImportTemplateSettingsEditor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/AutoImportTemplateSettingsTemplate.html'
		};

		function AutoImportTemplateSettingsEditor($scope, ctrl, $attrs) {

			this.initializeController = initializeController;

			var gridAPI;

			function initializeController() {

				$scope.scopeModel = {};
				$scope.scopeModel.gridData = [];
				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					var autoImportTemplateSettingsList = [];
					var autoImportTemplateSettingsEnumList = UtilsService.getArrayEnum(WhS_BE_SupplierAutoImportEmailTypeEnum);
					for (var i = 0, l = autoImportTemplateSettingsEnumList.length; i < l; i++) {

						var autoImportTemplateSetting = autoImportTemplateSettingsEnumList[i];
						var storedSettings = getStoredSettings(autoImportTemplateSetting, dataRetrievalInput.Query);
						var dataItem = {
							EmailType: autoImportTemplateSetting,
							EmailTemplateId: (storedSettings != null) ? storedSettings.EmailTemplateId : null,
							AttachPricelist: (storedSettings != null) ? storedSettings.AttachPricelist : false,
							TemplateSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
						};
						extendDataItem(dataItem);
						autoImportTemplateSettingsList.push(dataItem);
					}
					onResponseReady({
						Data: autoImportTemplateSettingsList
					});
				};
			}

			function defineAPI() {
				var api = {};
				api.load = function (payload) {
					return gridAPI.retrieveData(payload.AutoImportTemplateSettings);
				};

				api.getData = function () {
					var items = [];
					if ($scope.scopeModel.gridData != null && $scope.scopeModel.gridData.length > 0) {
						for (var i = 0; i < $scope.scopeModel.gridData.length; i++) {
							var item = $scope.scopeModel.gridData[i];
							items.push({
								TemplateType: item.EmailType.value,
								EmailTemplateId: (item.EmailTemplate != null) ? item.EmailTemplate.VRMailMessageTemplateId : null,
								AttachPricelist: item.AttachPricelist,
							});
						}
					}
					return items;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function extendDataItem(dataItem) {
				dataItem.OnMailMessagetemplateSelectorReady = function (api) {
					dataItem.TemplateSelectorReadyDeferred.resolve();
					dataItem.selectorAPI = api;
					loadMailMsgTemplateSelector(dataItem);
				};
			}

			function getStoredSettings(autoImportTemplateSetting, autoImportTemplateSettings) {
				if (autoImportTemplateSettings != null && autoImportTemplateSettings.length >= 0) {
					for (var i = 0; i < autoImportTemplateSettings.length; i++) {
						if (autoImportTemplateSettings[i].TemplateType == autoImportTemplateSetting.value)
							return autoImportTemplateSettings[i];
					}
				}
			}

			function loadMailMsgTemplateSelector(dataItem) {
				var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
				dataItem.TemplateSelectorReadyDeferred.promise.then(function () {
					var mailMsgTemplateSelectorPayload = {
						selectedIds: dataItem.EmailTemplateId,
						filter: {
							VRMailMessageTypeId: "757f93c4-1886-4002-82ae-2fda0e529a3c"
						}
					};
					VRUIUtilsService.callDirectiveLoad(dataItem.selectorAPI, mailMsgTemplateSelectorPayload, mailMsgTemplateSelectorLoadDeferred);
				});
				return mailMsgTemplateSelectorLoadDeferred.promise;
			}
		}
	}]);