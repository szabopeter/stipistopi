import { ResourceLineViewModel } from "./resourceline.js";
import { MessageManagerViewModel } from "./messagemanager.js";


function ResourcesPageViewModel(backend) {
    let self = this;
    this.backend = backend;
    this.resources = ko.observableArray([]);
    this.messageManagerVm = new MessageManagerViewModel();
    this.buttonsEnabled = ko.observable(true);

    this.updateButtonsState = function () {
        let isEditing = false;
        self.resources().forEach(resource => {
            if (resource.descriptionEditorVm.isEditing())
                isEditing = true;
        });
        self.resources().forEach(resource => resource.actionsEnabled(!isEditing));
    };

    function UpdateResourceList(resourceInfos) {
        self.resources(resourceInfos.map(function (resourceInfo) {
            let vm = ResourceLineViewModel.create(resourceInfo, self.backend, self.messageManagerVm, self.updateButtonsState);
            vm.messageManagerVm = self.messageManagerVm;
            vm.updateActions(self.refresh);
            return vm;
        }));
    };

    this.refresh = function () {
        self.backend.loadResources(UpdateResourceList, function () { self.resources([]) });
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