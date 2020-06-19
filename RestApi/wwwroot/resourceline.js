import { DescriptionEditorViewModel } from "./descriptioneditor.js";

function ResourceLineViewModel(backend, messageManagerVm, updateButtonsState) {
    let self = this;
    this.backend = backend;
    this.messageManagerVm = messageManagerVm;
    this.updateButtonsState = updateButtonsState;
    this.shortName = ko.observable("TODO: shortName");
    this.address = ko.observable("TODO: address");
    this.descriptionEditorVm = new DescriptionEditorViewModel(backend, messageManagerVm, updateButtonsState);
    this.isAvailable = ko.observable("TODO: isAvailable");
    this.lockedBy = ko.observable("TODO: lockedBy");
    this.actions = ko.observableArray([]);
    this.messageManagerVm = null;
    this.actionsEnabled = ko.observable(true);

    this.updateActions = function (refresh) {
        let resource = this;
        let label = resource.isAvailable() ? "Lock" : "Release";
        let backendAction = resource.isAvailable() ? "lock" : "release";
        let execute = function () {
            self.backend.sendResourceAction(backendAction, resource.shortName(), function (data) {
                refresh();
                if (data == true) {
                    self.messageManagerVm.showDisappearingMessage("OK");
                } else {
                    self.messageManagerVm.showMessageWithOk("Failed!");
                };
            }, refresh);
        };
        resource.actions([{
            label: label,
            backendAction: backendAction,
            execute: execute,
        }]);
    };
}

ResourceLineViewModel.create = function (resourceInfo, backend, messageManagerVm, updateButtonsState) {
    let vm = new ResourceLineViewModel(backend, messageManagerVm, updateButtonsState);
    vm.shortName(resourceInfo.resource.shortName);
    vm.address(resourceInfo.resource.address);
    vm.descriptionEditorVm.resourceName = resourceInfo.resource.shortName;
    vm.descriptionEditorVm.oldDescription(resourceInfo.resource.description);
    vm.descriptionEditorVm.newDescription(resourceInfo.resource.description);
    vm.isAvailable(resourceInfo.isAvailable);
    vm.lockedBy(resourceInfo.lockedBy);
    // vm.actions(source.actions);
    // vm.actions().forEach(function (action) { action.execute = action.execute.bind(vm); })
    return vm;
}

function resourceLineRegisterWidget(template) {
    let widgetName = "resourceline-widget";
    ko.components.register(widgetName, {
        viewModel: function (params) { return params; },
        template: template,
    });
    return widgetName;
}

export { ResourceLineViewModel, resourceLineRegisterWidget }