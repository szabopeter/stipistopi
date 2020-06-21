import { ResourceLineViewModel } from "./resourceline.js";
import { MessageManagerViewModel } from "./messagemanager.js";


function ResourcesPageViewModel(backend) {
    let self = this;
    this.backend = backend;
    this.resources = ko.observableArray([]);
    this.messageManagerVm = new MessageManagerViewModel();
    this.buttonsEnabled = ko.observable(true);
    this.refreshEnabled = ko.observable(true);

    this.updateButtonsState = function () {
        let isEditing = false;
        self.resources().forEach(resource => {
            if (resource.descriptionEditorVm.isEditing())
                isEditing = true;
        });
        self.resources().forEach(resource => resource.actionsEnabled(!isEditing));
    };

    function UpdateResourceList(ssResources) {
        self.resources(ssResources.map(function (ssResource) {
            let vm = ResourceLineViewModel.create(ssResource, self.backend, self.messageManagerVm, self.updateButtonsState);
            vm.messageManagerVm = self.messageManagerVm;
            vm.updateActions(self.refresh);
            return vm;
        }));
        self.refreshEnabled(true);
    };

    this.refresh = function () {
        self.refreshEnabled(false);
        self.backend.loadResources(UpdateResourceList, function () {
            self.resources([]);
            self.refreshEnabled(true);
        });
    };

    this.pageActivated = this.refresh;
}

function resourcesPageRegisterWidget(template) {
    let widgetName = "resources-page-widget";
    ko.components.register(widgetName, {
        viewModel: function (params) { return params; },
        template: template,
    });
    return widgetName;
}

export { ResourcesPageViewModel, resourcesPageRegisterWidget };